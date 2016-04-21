/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordRRSIG : IRecord
    {
        byte[] RDATA { get; }
    }

    public class RecordRRSIG : Record, IRecordRRSIG
    {
		public byte[] RDATA { get; private set; }

        public RecordRRSIG(RecordReader rr)
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
