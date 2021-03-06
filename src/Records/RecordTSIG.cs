/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
 * http://www.ietf.org/rfc/rfc2845.txt
 * 
 * Field Name       Data Type      Notes
      --------------------------------------------------------------
      Algorithm Name   domain-name    Name of the algorithm
                                      in domain name syntax.
      Time Signed      u_int48_t      seconds since 1-Jan-70 UTC.
      Fudge            u_int16_t      seconds of error permitted
                                      in Time Signed.
      MAC Size         u_int16_t      number of octets in MAC.
      MAC              octet stream   defined by Algorithm Name.
      Original ID      u_int16_t      original message ID
      Error            u_int16_t      expanded RCODE covering
                                      TSIG processing.
      Other Len        u_int16_t      length, in octets, of
                                      Other Data.
      Other Data       octet stream   empty unless Error == BADTIME

 */

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordTSIG : IRecord
    {
        string ALGORITHMNAME { get; }
        long TIMESIGNED { get; }
        UInt16 FUDGE { get; }
        UInt16 MACSIZE { get; }
        byte[] MAC { get; }
        UInt16 ORIGINALID { get; }
        UInt16 ERROR { get; }
        UInt16 OTHERLEN { get; }
        byte[] OTHERDATA { get; }
    }

    public class RecordTSIG : Record, IRecordTSIG
    {
		public string ALGORITHMNAME { get; private set; }
        public long TIMESIGNED { get; private set; }
        public UInt16 FUDGE { get; private set; }
        public UInt16 MACSIZE { get; private set; }
        public byte[] MAC { get; private set; }
        public UInt16 ORIGINALID { get; private set; }
        public UInt16 ERROR { get; private set; }
        public UInt16 OTHERLEN { get; private set; }
        public byte[] OTHERDATA { get; private set; }

        public RecordTSIG(RecordReader rr)
		{
			ALGORITHMNAME = rr.ReadDomainName();
			TIMESIGNED = rr.ReadUInt32() << 32 | rr.ReadUInt32();
			FUDGE = rr.ReadUInt16();
			MACSIZE = rr.ReadUInt16();
			MAC = rr.ReadBytes(MACSIZE);
			ORIGINALID = rr.ReadUInt16();
			ERROR = rr.ReadUInt16();
			OTHERLEN = rr.ReadUInt16();
			OTHERDATA = rr.ReadBytes(OTHERLEN);
		}

		public override string ToString()
		{
			DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
			dateTime = dateTime.AddSeconds(TIMESIGNED);
			string printDate = dateTime.ToShortDateString() + " " + dateTime.ToShortTimeString();
			return string.Format("{0} {1} {2} {3} {4}",
				ALGORITHMNAME,
				printDate,
				FUDGE,
				ORIGINALID,
				ERROR);
		}

	}
}
