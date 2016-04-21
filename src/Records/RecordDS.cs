/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
 * http://tools.ietf.org/rfc/rfc3658.txt
 * 
2.4.  Wire Format of the DS record

   The DS (type=43) record contains these fields: key tag, algorithm,
   digest type, and the digest of a public key KEY record that is
   allowed and/or used to sign the child's apex KEY RRset.  Other keys
   MAY sign the child's apex KEY RRset.

                        1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |           key tag             |  algorithm    |  Digest type  |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                digest  (length depends on type)               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                (SHA-1 digest is 20 bytes)                     |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|
   |                                                               |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+

 */

namespace Yamool.Net.DNS.Records
{
    using System;
    using System.Text;

    public interface IRecordDS : IRecord
    {
        UInt16 KEYTAG { get; }
        byte ALGORITHM { get; }
        byte DIGESTTYPE { get; }
        byte[] DIGEST { get; }
    }

    public class RecordDS : Record, IRecordDS
    {
		public UInt16 KEYTAG { get; private set; }
        public byte ALGORITHM { get; private set; }
        public byte DIGESTTYPE { get; private set; }
        public byte[] DIGEST { get; private set; }

        public RecordDS(RecordReader rr)
		{
			ushort length = rr.ReadUInt16(-2);
			KEYTAG = rr.ReadUInt16();
			ALGORITHM = rr.ReadByte();
			DIGESTTYPE = rr.ReadByte();
			length -= 4;
			DIGEST = new byte[length];
			DIGEST = rr.ReadBytes(length);
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for (int intI = 0; intI < DIGEST.Length; intI++)
				sb.AppendFormat("{0:x2}", DIGEST[intI]);
			return string.Format("{0} {1} {2} {3}",
				KEYTAG,
				ALGORITHM,
				DIGESTTYPE,
				sb.ToString());
		}

	}
}
