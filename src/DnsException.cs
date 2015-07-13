// Copyright (c) Yamool. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

namespace Yamool.Net.DNS
{
    using System;
    using System.Collections.Generic;

    public class DnsException : Exception
    {
        public DnsException(RCODE responseCode, string message)
            : base(message)
        {
            this.ResponseCode = responseCode;
        }

        public RCODE ResponseCode
        {
            get;
            private set;
        }
    }
}
