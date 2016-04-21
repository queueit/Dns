/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
3.3.3. MB RDATA format (EXPERIMENTAL)

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   MADNAME                     /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

MADNAME         A <domain-name> which specifies a host which has the
                specified mailbox.

MB records cause additional section processing which looks up an A type
RRs corresponding to MADNAME.
*/
namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordMB : IRecord
    {
        string MADNAME { get; }
    }

    public class RecordMB : Record, IRecordMB
    {
		public string MADNAME { get; private set; }

		public RecordMB(RecordReader rr)
		{
			MADNAME = rr.ReadDomainName();
		}

		public override string ToString()
		{
			return MADNAME;
		}

	}
}
