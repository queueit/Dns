// Copyright (c) Yamool. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

namespace Yamool.Net.DNS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Sockets;
    using System.Net.NetworkInformation;

    /// <summary>
    /// The Resolver class lets lookup Domain Name System (DNS) resource records.
    /// </summary>
    public class Resolver
    {
        private static int _uid = new Random().Next();
        private static EndPoint[] _dnsServerDefault = null;
        private EndPoint[] _dnsServers;
        private TransportType _transportType;
        private int _retries;
        private int _timeout;        

        #region ctors
        /// <summary>
        /// Constructor of Resolver using default dns server on local.
        /// </summary>
        public Resolver():this(GetLocalMachineDnsServers())
        {
        }

        public Resolver(IPEndPoint dnsServer)
            : this(new EndPoint[] { dnsServer })
        {
        }

        /// <summary>
        /// Constructor of Resolver using DNS server specified.
        /// </summary>
        /// <param name="dnsServer">The DNS server to use</param>
        public Resolver(EndPoint[] dnsServer)
        {
            _dnsServers = dnsServer;            
            _transportType = TransportType.Udp;
            _retries = 5;
            _timeout = 3000;
            this.Recursion = true;
        }
        #endregion

        /// <summary>
        /// Query and get a resource record on specified DNS Server.
        /// </summary>
        /// <param name="host">The name of host to query.</param>
        /// <param name="qType">The QTYPE to use</param>
        /// <returns>Return an asynchronsous operation</returns>
        public Task<Response> QueryAsync(string host, QTYPE qType)
        {
            return this.QueryAsync(host, qType, QCLASS.IN);
        }

        /// <summary>
        /// Query and get a resource record on specified DNS Server.
        /// </summary>
        /// <param name="host">The name of host to query.</param>
        /// <param name="qType">The QTYPE to use</param>
        /// <param name="qClass">The QCLASS to use</param>
        /// <returns>Return an asynchronsous operation.</returns>
        public Task<Response> QueryAsync(string host, QTYPE qType, QCLASS qClass)
        {
            var question = new Question(host, qType, qClass);
            var request = new Request()
            {
                Retries = this.Retries,
                DnsServers = this._dnsServers,
                Timeout = this.Timeout
            };
            var id = (ushort)Interlocked.Increment(ref _uid);
            request.Header.ID = id;
            request.Header.RD = this.Recursion;
            request.AddQuestion(question);
            DnsChannel channel = null;
            if (_transportType == TransportType.Tcp)
            {
                channel = new TcpChannel(request);
            }
            else
            {
                channel = new UdpChannel(request);
            }
            return channel.GetResponseAsync(_dnsServers[0]);
        }

        /// <summary>
        ///	Resolves a host name or IP address to an <see cref="System.Net.IPHostEntry"/> instance.
        /// </summary>
        /// <param name="hostNameOrAddress">The host name or IP address to resolve.</param>
        /// <returns>
        ///		An System.Net.IPHostEntry instance that contains address information about
        ///		the host specified in hostNameOrAddress. 
        ///</returns>
        public Task<IPHostEntry> GetHostEntryAsync(string hostNameOrAddress)
        {
            IPAddress iPAddress;            
            if (IPAddress.TryParse(hostNameOrAddress, out iPAddress))
            {
                return GetHostEntryAsync(iPAddress);
            }
            else
            {
                return MakeEntryAsync(hostNameOrAddress);
            }
        }

        /// <summary>
        ///		Resolves an IP address to an <see cref="System.Net.IPHostEntry"/> instance.
        /// </summary>
        /// <param name="ip">An IP address.</param>
        /// <returns>
        ///		An System.Net.IPHostEntry instance that contains address information about
        ///		the host specified in address.
        ///</returns>
        public Task<IPHostEntry> GetHostEntryAsync(IPAddress ip)
        {
            var tcs = new TaskCompletionSource<IPHostEntry>();
            this.QueryAsync(GetArpaFromIp(ip), QTYPE.PTR, QCLASS.IN).ContinueWith((Task<Response> requestTask) =>
            {
                if (requestTask.IsCanceled)
                {
                    tcs.SetCanceled();
                    return;
                }
                if (requestTask.IsFaulted)
                {
                    tcs.SetException(requestTask.Exception.GetBaseException());
                    return;
                }
                var response = requestTask.Result;
                var ptrs = response.RecordsPTR();
                if (ptrs.Length > 0)
                {
                    this.MakeEntryAsync(ptrs[0].PTRDNAME).ContinueWith((Task<IPHostEntry> entryTask) =>
                    {
                        if (entryTask.IsCanceled)
                        {
                            tcs.SetCanceled();
                            return;
                        }
                        if (entryTask.IsFaulted)
                        {
                            tcs.SetException(entryTask.Exception.GetBaseException());
                            return;
                        }
                        tcs.SetResult(entryTask.Result);
                    }, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously,
                    TaskScheduler.Default);
                }
                else
                {
                    tcs.SetResult(new IPHostEntry());
                }
            },
            CancellationToken.None,
            TaskContinuationOptions.ExecuteSynchronously,
            TaskScheduler.Default);
            return tcs.Task;
        }

        private Task<IPHostEntry> MakeEntryAsync(string hostName)
        {
            var tcs = new TaskCompletionSource<IPHostEntry>();
            this.QueryAsync(hostName, QTYPE.A, QCLASS.IN).ContinueWith((Task<Response> requestTask) =>
            {
                if (requestTask.IsCanceled)
                {
                    tcs.SetCanceled();
                    return;
                }
                if (requestTask.IsFaulted)
                {
                    tcs.SetException(requestTask.Exception.GetBaseException());
                    return;
                }
                var entry = new IPHostEntry()
                {
                    HostName = hostName
                };
                var response = requestTask.Result;
                var addressList = new List<IPAddress>();
                var aliases = new List<string>();
                foreach (var answerRR in response.Answers)
                {
                    if (answerRR.TYPE == TYPE.A)
                    {
                        addressList.Add(IPAddress.Parse((answerRR.RECORD.ToString())));
                        entry.HostName = answerRR.NAME;
                    }
                    else
                    {
                        if (answerRR.TYPE == TYPE.CNAME)
                        {
                            aliases.Add(answerRR.NAME);
                        }
                    }
                }
                entry.AddressList = addressList.ToArray();
                entry.Aliases = aliases.ToArray();
                tcs.SetResult(entry);
            });
            return tcs.Task;
        }

        /// <summary>
        /// Translates the IPV4 or IPV6 address into an arpa address
        /// </summary>
        /// <param name="ip">IP address to get the arpa address form</param>
        /// <returns>The 'mirrored' IPV4 or IPV6 arpa address</returns>
        private static string GetArpaFromIp(IPAddress ip)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                var sb = new StringBuilder();
                sb.Append("in-addr.arpa.");
                foreach (byte b in ip.GetAddressBytes())
                {
                    sb.Insert(0, string.Format("{0}.", b));
                }
                return sb.ToString();
            }
            if (ip.AddressFamily == AddressFamily.InterNetworkV6)
            {
                var sb = new StringBuilder();
                sb.Append("ip6.arpa.");
                foreach (byte b in ip.GetAddressBytes())
                {
                    sb.Insert(0, string.Format("{0:x}.", (b >> 4) & 0xf));
                    sb.Insert(0, string.Format("{0:x}.", (b >> 0) & 0xf));
                }
                return sb.ToString();
            }
            return "?";
        }

        private static string GetArpaFromEnum(string strEnum)
        {
            var sb = new StringBuilder();
            var Number = System.Text.RegularExpressions.Regex.Replace(strEnum, "[^0-9]", "");
            sb.Append("e164.arpa.");
            foreach (char c in Number)
            {
                sb.Insert(0, string.Format("{0}.", c));
            }
            return sb.ToString();
        }

        /// <summary>
        /// Gets or sets protocol to use.Default use UDP protocol.
        /// </summary>
        public TransportType TransportType
        {
            get
            {
                return _transportType;
            }
            set
            {
                if (value == TransportType.Udp || value == TransportType.Tcp)
                {
                    _transportType = value;
                }
                else
                {
                    throw new NotSupportedException("Only support a TCP or UDP methods.");
                }              
            }
        }

        /// <summary>
        /// Gets or sets timeout in milliseconds.
        /// Default value is 3000 milliseconds.
        /// </summary>
        public int Timeout
        {
            get
            {
                return _timeout;
            }
            set
            {
                _timeout = value;
            }
        }

        /// <summary>
        /// Gets or sets number of retries before giving up.Default value is 5.
        /// </summary>
        public int Retries
        {
            get
            {
                return _retries;
            }
            set
            {
                if (value >= 1)
                {
                    _retries = value;
                }
            }
        }

        /// <summary>
        /// Indicates the dns server whether use a recursion query.
        /// </summary>
        private bool Recursion
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets list of DNS servers to use
        /// </summary>
        public EndPoint[] DnsServers
        {
            get
            {
                return _dnsServers;
            }
            set
            {
                _dnsServers = value;
            }
        }

        private static EndPoint[] GetLocalMachineDnsServers()
        {
            if (_dnsServerDefault == null)
            {
                var list = new HashSet<IPEndPoint>();
                foreach (NetworkInterface n in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (n.OperationalStatus == OperationalStatus.Up)
                    {
                        var ipProps = n.GetIPProperties();
                        foreach (var ipAddr in ipProps.DnsAddresses)
                        {
                            IPEndPoint entry = new IPEndPoint(ipAddr, 53);
                            if (!list.Contains(entry))
                            {
                                list.Add(entry);
                            }
                        }
                    }
                }
                _dnsServerDefault = list.ToArray();
            }
            return _dnsServerDefault;            
        }        
    }
}
