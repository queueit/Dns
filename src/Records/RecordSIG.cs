/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

#region Rfc info
/*
 * http://www.ietf.org/rfc/rfc2535.txt
 * 4.1 SIG RDATA Format

   The RDATA portion of a SIG RR is as shown below.  The integrity of
   the RDATA information is protected by the signature field.

                           1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
       0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |        type covered           |  algorithm    |     labels    |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                         original TTL                          |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                      signature expiration                     |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |                      signature inception                      |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
      |            key  tag           |                               |
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+         signer's name         +
      |                                                               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-/
      /                                                               /
      /                            signature                          /
      /                                                               /
      +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+


*/
#endregion

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordSIG : IRecord
    {
        UInt16 TYPECOVERED { get; }
        byte ALGORITHM { get; }
        byte LABELS { get; }
        UInt32 ORIGINALTTL { get; }
        UInt32 SIGNATUREEXPIRATION { get; }
        UInt32 SIGNATUREINCEPTION { get; }
        UInt16 KEYTAG { get; }
        string SIGNERSNAME { get; }
        string SIGNATURE { get; }
    }

    public class RecordSIG : Record, IRecordSIG
    {
		public UInt16 TYPECOVERED { get; private set; }
        public byte ALGORITHM { get; private set; }
        public byte LABELS { get; private set; }
        public UInt32 ORIGINALTTL { get; private set; }
        public UInt32 SIGNATUREEXPIRATION { get; private set; }
        public UInt32 SIGNATUREINCEPTION { get; private set; }
        public UInt16 KEYTAG { get; private set; }
        public string SIGNERSNAME { get; private set; }
        public string SIGNATURE { get; private set; }

		public RecordSIG(RecordReader rr)
		{
			TYPECOVERED = rr.ReadUInt16();
			ALGORITHM = rr.ReadByte();
			LABELS = rr.ReadByte();
			ORIGINALTTL = rr.ReadUInt32();
			SIGNATUREEXPIRATION = rr.ReadUInt32();
			SIGNATUREINCEPTION = rr.ReadUInt32();
			KEYTAG = rr.ReadUInt16();
			SIGNERSNAME = rr.ReadDomainName();
			SIGNATURE = rr.ReadString();
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} \"{8}\"",
				TYPECOVERED,
				ALGORITHM,
				LABELS,
				ORIGINALTTL,
				SIGNATUREEXPIRATION,
				SIGNATUREINCEPTION,
				KEYTAG,
				SIGNERSNAME,
				SIGNATURE);
		}

	}
}
