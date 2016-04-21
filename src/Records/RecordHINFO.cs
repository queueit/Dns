/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

/*
 3.3.2. HINFO RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                      CPU                      /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                       OS                      /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

CPU             A <character-string> which specifies the CPU type.

OS              A <character-string> which specifies the operating
                system type.

Standard values for CPU and OS can be found in [RFC-1010].

HINFO records are used to acquire general information about a host.  The
main use is for protocols such as FTP that can use special procedures
when talking between machines or operating systems of the same type.
 */

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordHINFO : IRecord
    {
        string CPU { get; }
        string OS { get; }
    }

    public class RecordHINFO : Record, IRecordHINFO
    {
		public string CPU { get; private set; }
        public string OS { get; private set; }

        public RecordHINFO(RecordReader rr)
		{
			CPU = rr.ReadString();
			OS = rr.ReadString();
		}

		public override string ToString()
		{
			return string.Format("CPU={0} OS={1}",CPU,OS);
		}

	}
}
