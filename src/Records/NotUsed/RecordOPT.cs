/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

namespace Yamool.Net.DNS.Records
{
    using System;

	public class RecordOPT : Record
	{
		public byte[] RDATA;

		public RecordOPT(RecordReader rr)
		{
			// re-read length
			ushort RDLENGTH = rr.ReadUInt16(-2);
			RDATA = rr.ReadBytes(RDLENGTH);
		}

		public override string ToString()
		{
			return string.Format("not-used");
		}

	}
}
