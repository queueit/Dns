/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
3.3.6. MG RDATA format (EXPERIMENTAL)

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   MGMNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

MGMNAME         A <domain-name> which specifies a mailbox which is a
                member of the mail group specified by the domain name.

MG records cause no additional section processing.
*/
namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordMG : IRecord
    {
        string MGMNAME { get; }
    }

    public class RecordMG : Record, IRecordMG
    {
		public string MGMNAME { get; private set; }

        public RecordMG(RecordReader rr)
		{
			MGMNAME = rr.ReadDomainName();
		}

		public override string ToString()
		{
			return MGMNAME;
		}

	}
}
