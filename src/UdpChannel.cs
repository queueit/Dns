// Copyright (c) Yamool. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

namespace Yamool.Net.DNS
{
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;

    internal class UdpChannel : DnsChannel
    {
        public UdpChannel(Request request)
            : base(request, ProtocolType.Udp)
        {
        }

        protected override byte[] PrepareRequestData(byte[] data)
        {
            return data;
        }

        protected override void OnReceived(byte[] buffer, int offset, int count)
        {
            var response = new Response(this.EndPoint, buffer);
            /*
                TC(TrunCation)- specifies that this message was truncated due to length greater than that permitted on the transmission channel
             */
            if (response.Header.TC)
            {
                throw new DnsException(RCODE.BADTRUNC, "The message length greater than that permitted.");
            }
            this.OnResponseCompleted(response);
        }
    }
}
