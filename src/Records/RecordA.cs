/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
 3.4.1. A RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                    ADDRESS                    |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

ADDRESS         A 32 bit Internet address.

Hosts that have multiple Internet addresses will have multiple A
records.
 * 
 */
namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordA : IRecord
    {
        System.Net.IPAddress Address { get; }
    }

    public class RecordA : Record, IRecordA
    {
		public System.Net.IPAddress Address { get; private set; }

		public RecordA(RecordReader rr)
		{
		    System.Net.IPAddress ipAddress = null;

            System.Net.IPAddress.TryParse(string.Format("{0}.{1}.{2}.{3}",
				rr.ReadByte(),
				rr.ReadByte(),
				rr.ReadByte(),
				rr.ReadByte()), out ipAddress);

		    this.Address = ipAddress;
		}

		public override string ToString()
		{
			return Address.ToString();
		}

	}
}
