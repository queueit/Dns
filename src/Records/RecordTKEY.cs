/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
 * http://tools.ietf.org/rfc/rfc2930.txt
 * 
2. The TKEY Resource Record

   The TKEY resource record (RR) has the structure given below.  Its RR
   type code is 249.

      Field       Type         Comment
      -----       ----         -------
       Algorithm:   domain
       Inception:   u_int32_t
       Expiration:  u_int32_t
       Mode:        u_int16_t
       Error:       u_int16_t
       Key Size:    u_int16_t
       Key Data:    octet-stream
       Other Size:  u_int16_t
       Other Data:  octet-stream  undefined by this specification

 */

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordTKEY : IRecord
    {
        string ALGORITHM { get; }
        UInt32 INCEPTION { get; }
        UInt32 EXPIRATION { get; }
        UInt16 MODE { get; }
        UInt16 ERROR { get; }
        UInt16 KEYSIZE { get; }
        byte[] KEYDATA { get; }
        UInt16 OTHERSIZE { get; }
        byte[] OTHERDATA { get; }
    }

    public class RecordTKEY : Record, IRecordTKEY
    {
		public string ALGORITHM { get; private set; }
        public UInt32 INCEPTION { get; private set; }
        public UInt32 EXPIRATION { get; private set; }
        public UInt16 MODE { get; private set; }
        public UInt16 ERROR { get; private set; }
        public UInt16 KEYSIZE { get; private set; }
        public byte[] KEYDATA { get; private set; }
        public UInt16 OTHERSIZE { get; private set; }
        public byte[] OTHERDATA { get; private set; }

        public RecordTKEY(RecordReader rr)
		{
			ALGORITHM = rr.ReadDomainName();
			INCEPTION = rr.ReadUInt32();
			EXPIRATION = rr.ReadUInt32();
			MODE = rr.ReadUInt16();
			ERROR = rr.ReadUInt16();
			KEYSIZE = rr.ReadUInt16();
			KEYDATA = rr.ReadBytes(KEYSIZE);
			OTHERSIZE = rr.ReadUInt16();
			OTHERDATA = rr.ReadBytes(OTHERSIZE);
		}

		public override string ToString()
		{
			return string.Format("{0} {1} {2} {3} {4}",
				ALGORITHM,
				INCEPTION,
				EXPIRATION,
				MODE,
				ERROR);
		}

	}
}
