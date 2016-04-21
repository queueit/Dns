/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */


/*
 * http://tools.ietf.org/rfc/rfc2230.txt
 * 
 * 3.1 KX RDATA format

   The KX DNS record has the following RDATA format:

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    |                  PREFERENCE                   |
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   EXCHANGER                   /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

   where:

   PREFERENCE      A 16 bit non-negative integer which specifies the
                   preference given to this RR among other KX records
                   at the same owner.  Lower values are preferred.

   EXCHANGER       A <domain-name> which specifies a host willing to
                   act as a mail exchange for the owner name.

   KX records MUST cause type A additional section processing for the
   host specified by EXCHANGER.  In the event that the host processing
   the DNS transaction supports IPv6, KX records MUST also cause type
   AAAA additional section processing.

   The KX RDATA field MUST NOT be compressed.

 */
namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordKX : IRecord
    {
        ushort PREFERENCE { get; }
        string EXCHANGER { get; }
    }

    public class RecordKX : Record, IComparable, IRecordKX
    {
		public ushort PREFERENCE { get; private set; }
        public string EXCHANGER { get; private set; }

        public RecordKX(RecordReader rr)
		{
			PREFERENCE = rr.ReadUInt16();
			EXCHANGER = rr.ReadDomainName();
		}

		public override string ToString()
		{
			return string.Format("{0} {1}", PREFERENCE, EXCHANGER);
		}

		public int CompareTo(object objA)
		{
			RecordKX recordKX = objA as RecordKX;
			if (recordKX == null)
				return -1;
			else if (this.PREFERENCE > recordKX.PREFERENCE)
				return 1;
			else if (this.PREFERENCE < recordKX.PREFERENCE)
				return -1;
			else // they are the same, now compare case insensitive names
				return string.Compare(this.EXCHANGER, recordKX.EXCHANGER, true);
		}

	}
}
