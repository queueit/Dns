/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
3.3.8. MR RDATA format (EXPERIMENTAL)

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   NEWNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

NEWNAME         A <domain-name> which specifies a mailbox which is the
                proper rename of the specified mailbox.

MR records cause no additional section processing.  The main use for MR
is as a forwarding entry for a user who has moved to a different
mailbox.
*/
namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordMR : IRecord
    {
        string NEWNAME { get; }
    }

    public class RecordMR : Record, IRecordMR
    {
		public string NEWNAME { get; private set; }

        public RecordMR(RecordReader rr)
		{
			NEWNAME = rr.ReadDomainName();
		}

		public override string ToString()
		{
			return NEWNAME;
		}

	}
}
