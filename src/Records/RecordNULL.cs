/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
3.3.10. NULL RDATA format (EXPERIMENTAL)

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                  <anything>                   /
    /                                               /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

Anything at all may be in the RDATA field so long as it is 65535 octets
or less.

NULL records cause no additional section processing.  NULL RRs are not
allowed in master files.  NULLs are used as placeholders in some
experimental extensions of the DNS.
*/
namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordNULL : IRecord
    {
        byte[] ANYTHING { get; }
    }

    public class RecordNULL : Record, IRecordNULL
    {
		public byte[] ANYTHING { get; private set; }

        public RecordNULL(RecordReader rr)
		{
			rr.Position -= 2;
			// re-read length
			ushort RDLENGTH = rr.ReadUInt16();
			ANYTHING = new byte[RDLENGTH];
			ANYTHING = rr.ReadBytes(RDLENGTH);
		}

		public override string ToString()
		{
			return string.Format("...binary data... ({0}) bytes",ANYTHING.Length);
		}

	}
}
