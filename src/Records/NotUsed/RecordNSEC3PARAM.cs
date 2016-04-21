/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordNSEC3PARAM : IRecord
    {
        byte[] RDATA { get; }
    }

    public class  RecordNSEC3PARAM : Record, IRecordNSEC3PARAM
    {
		public byte[] RDATA { get; private set; }

        public RecordNSEC3PARAM(RecordReader rr)
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
