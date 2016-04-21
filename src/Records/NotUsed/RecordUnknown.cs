/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordUnknown : IRecord
    {
        byte[] RDATA { get; }
    }

    public class RecordUnknown : Record, IRecordUnknown
    {
		public byte[] RDATA { get; private set; }
        public RecordUnknown(RecordReader rr)
		{
			// re-read length
			ushort RDLENGTH = rr.ReadUInt16(-2);
			RDATA = rr.ReadBytes(RDLENGTH);
		}
	}
}
