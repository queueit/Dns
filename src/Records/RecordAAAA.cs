/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

#region Rfc info
/*
2.2 AAAA data format

   A 128 bit IPv6 address is encoded in the data portion of an AAAA
   resource record in network byte order (high-order byte first).
 */
#endregion

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordAAAA : IRecord
    {
        System.Net.IPAddress Address { get; }
    }

    public class RecordAAAA : Record, IRecordAAAA
    {
		public System.Net.IPAddress Address { get; private set; }

		public RecordAAAA(RecordReader rr)
		{
		    System.Net.IPAddress ipAddress = null;

		    if (System.Net.IPAddress.TryParse(
		        string.Format("{0:x}:{1:x}:{2:x}:{3:x}:{4:x}:{5:x}:{6:x}:{7:x}",
		            rr.ReadUInt16(),
		            rr.ReadUInt16(),
		            rr.ReadUInt16(),
		            rr.ReadUInt16(),
		            rr.ReadUInt16(),
		            rr.ReadUInt16(),
		            rr.ReadUInt16(),
		            rr.ReadUInt16()), out ipAddress))
		    {
		        this.Address = ipAddress;
		    }
		}

		public override string ToString()
		{
			return Address.ToString();
		}

	}
}
