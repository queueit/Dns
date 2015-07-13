// Copyright (c) Yamool. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

namespace Yamool.Net.DNS
{
    using System;
    using System.Net.Sockets;

    internal class TcpChannel : DnsChannel
    {
        private byte[] _messageData;
        private int _messageSize;

        public TcpChannel(Request request) : base(request, ProtocolType.Tcp) { }

        protected override byte[] PrepareRequestData(byte[] data)
        {
            var buffer = new byte[data.Length + 2];
            buffer[0] = (byte)((data.Length >> 8) & 0xff);
            buffer[1] = (byte)(data.Length & 0xff);
            Buffer.BlockCopy(data, 0, buffer, 2, data.Length);
            return buffer;
        }

        protected override void OnReceived(byte[] buffer, int offset, int count)
        {
            if (count == 0)
            {
                this.OnResponseCompleted(null);
                return;
            }
            //The message is prefixed with a two byte length field which gives the message length, excluding the two byte length field. 
            if (_messageSize == 0)
            {
                _messageSize = buffer[0] << 8 | buffer[1];
                offset += 2;
            }
            if (_messageData == null)
            {
                _messageData = new byte[count - offset];
                Buffer.BlockCopy(buffer, offset, _messageData, 0, count - offset);
            }
            else
            {
                var newBuffer = new byte[count + _messageData.Length];
                Buffer.BlockCopy(_messageData, 0, newBuffer, 0, _messageData.Length);
                Buffer.BlockCopy(buffer, 0, newBuffer, _messageData.Length, count);
                _messageData = newBuffer;
            }
            if (_messageData.Length < _messageSize)
            {
                //keep to continue receive data
                this.Receive();
                return;
            }
            var response = new Response(this.EndPoint, _messageData);
            if (response.Header.RCODE != RCODE.NoError)
            {
                this.OnResponseCompleted(response);
                return;
            }
            if (response.Questions[0].QType != QTYPE.AXFR)
            {
                this.OnResponseCompleted(response);
                return;
            }
            //occur an exception?
        }
    }
}
