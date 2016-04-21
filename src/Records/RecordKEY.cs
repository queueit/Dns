/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

#region Rfc info
/* http://www.ietf.org/rfc/rfc2535.txt
 * 
3.1 KEY RDATA format

   The RDATA for a KEY RR consists of flags, a protocol octet, the
   algorithm number octet, and the public key itself.  The format is as
   follows:
                        1 1 1 1 1 1 1 1 1 1 2 2 2 2 2 2 2 2 2 2 3 3
    0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1 2 3 4 5 6 7 8 9 0 1
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |             flags             |    protocol   |   algorithm   |
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
   |                                                               /
   /                          public key                           /
   /                                                               /
   +-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-|

   The KEY RR is not intended for storage of certificates and a separate
   certificate RR has been developed for that purpose, defined in [RFC
   2538].

   The meaning of the KEY RR owner name, flags, and protocol octet are
   described in Sections 3.1.1 through 3.1.5 below.  The flags and
   algorithm must be examined before any data following the algorithm
   octet as they control the existence and format of any following data.
   The algorithm and public key fields are described in Section 3.2.
   The format of the public key is algorithm dependent.

   KEY RRs do not specify their validity period but their authenticating
   SIG RR(s) do as described in Section 4 below.

*/
#endregion

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordKEY : IRecord
    {
        UInt16 FLAGS { get; }
        byte PROTOCOL { get; }
        byte ALGORITHM { get; }
        string PUBLICKEY { get; }
    }

    public class RecordKEY : Record, IRecordKEY
    {
		public UInt16 FLAGS { get; private set; }
        public byte PROTOCOL { get; private set; }
        public byte ALGORITHM { get; private set; }
        public string PUBLICKEY { get; private set; }

        public RecordKEY(RecordReader rr)
		{
			FLAGS = rr.ReadUInt16();
			PROTOCOL = rr.ReadByte();
			ALGORITHM = rr.ReadByte();
			PUBLICKEY = rr.ReadString();
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} \"{3}\"",
				FLAGS,
				PROTOCOL,
				ALGORITHM,
				PUBLICKEY);
		}

	}
}
