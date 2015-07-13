// Copyright (c) Yamool. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

namespace Yamool.Net.DNS
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;

    //http://stackoverflow.com/questions/5970383/difference-between-tcp-and-udp

    //http://stackoverflow.com/questions/1099672/when-is-it-appropriate-to-use-udp-instead-of-tcp When is it appropriate to use UDP instead of TCP?

    /// <summary>
    /// The socket channel connect to DNS server and get received data.
    /// </summary>
    /// <remarks>
    /// 
    /// http://en.wikipedia.org/wiki/Domain_Name_System#Protocol_details
    /// </remarks>
    internal abstract class DnsChannel
    {        
        private Socket _socket;
        private byte[] _buffer;
        private bool _disposed;

        private ProtocolType _protocolType;
        private TaskCompletionSource<Response> _tcs;
        private Request _request;
        private Timer _timeoutTimer = null;
        private int _state;
        private int _retryCount;

        public DnsChannel(Request request,ProtocolType protocolType)
        {
            _state = 1;
            if (protocolType == ProtocolType.Tcp)
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            }
            else
            {
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            }
            //RFC1035.size of a UDP datagram is 512 bytes
            //The Transmission Control Protocol (TCP) is used when the response data size exceeds 512 bytes, or for tasks such as zone transfers. 
            //Some resolver implementations use TCP for all queries.
            _buffer = new byte[512];
            _protocolType = protocolType;
            _request = request;
        }

        public Task<Response> GetResponseAsync(EndPoint remoteEP)
        {
            _tcs = new TaskCompletionSource<Response>(this);
            if (_request.Timeout > 0)
            {
                _timeoutTimer = new Timer(HandleTimeout, null, _request.Timeout, Timeout.Infinite);
                this.Connect(remoteEP);
            }
            return _tcs.Task;
        }
       
        public void Connect(EndPoint remoteEP)
        {            
            this.EndPoint = remoteEP;
            try
            {
                if (this.IsCancellationRequested)
                {
                    return;
                }
                _socket.BeginConnect(remoteEP, (ar) =>
                {
                    try
                    {
                        _socket.EndConnect(ar);
                        var data = this.PrepareRequestData(_request.Data);
                        _socket.BeginSend(data, 0, data.Length, SocketFlags.None, SendCallback, null);
                    }
                    catch (Exception ex)
                    {
                        this.OnError(ex);
                    }
                }, null);
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }
        
        private void SendCallback(IAsyncResult ar)
        {
            try
            {                
                var count = _socket.EndSend(ar);
                this.Receive();
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        public void Receive()
        {
            try
            {
                if (this.IsCancellationRequested)
                {
                    return;
                }
                var ar = _socket.BeginReceive(_buffer, 0, _buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {              
                var count = _socket.EndReceive(ar);
                if (this.IsCancellationRequested)
                {
                    return;
                }
                this.OnReceived(_buffer, 0, count);
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void DisConnect()
        {
            try
            {              
                if (_socket.Connected)
                {
                    _socket.Disconnect(true);
                }
            }
            catch { }
        }

        protected abstract byte[] PrepareRequestData(byte[] data);
        protected abstract void OnReceived(byte[] buffer, int offset, int count);
    
        private void SafeDispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                CloseSocket(_socket);             
                if (_timeoutTimer != null)
                {
                    _timeoutTimer.Dispose();
                }
            }           
        }       

        private static void CloseSocket(Socket socket)
        {
            try
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }
            }
            finally
            {
                try
                {
                    socket.Close();
                }
                catch { }
            }
        }

        protected EndPoint EndPoint
        {
            get;
            private set;
        }

        protected void OnResponseCompleted(Response response)
        {
            if (Interlocked.CompareExchange(ref _state, 2, 1) == 1)
            {
                _tcs.TrySetResult(response);
                this.SafeDispose();
            }
        }

        protected void OnError(Exception ex)
        {
            if (_request.Retries > 0 && (_retryCount++) <= _request.Retries && ex is SocketException)
            {
                //close a current socket.
                this.DisConnect();
                this.Connect(_request.DnsServers[_retryCount++ % _request.DnsServers.Length]);
            }
            else
            {
                if (Interlocked.CompareExchange(ref _state, 2, 1) == 1)
                {
                    _tcs.TrySetException(ex);
                    this.SafeDispose();
                }
            }
        }

        private void HandleTimeout(object state)
        {   
            _tcs.TrySetCanceled();
            if (Interlocked.CompareExchange(ref _state, 2, 1) == 1)
            {             
                this.SafeDispose();
            }
        }

        protected bool IsCancellationRequested
        {
            get
            {
                return _state > 1;
            }
        }
    }    
}
