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

	public class RecordMG : Record
	{
		public string MGMNAME;

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
