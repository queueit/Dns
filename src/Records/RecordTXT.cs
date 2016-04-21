/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

#region Rfc info
/*
3.3.14. TXT RDATA format

    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
    /                   TXT-DATA                    /
    +--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

where:

TXT-DATA        One or more <character-string>s.

TXT RRs are used to hold descriptive text.  The semantics of the text
depends on the domain where it is found.
 * 
*/
#endregion

namespace Yamool.Net.DNS.Records
{
    using System;

    public interface IRecordTXT : IRecord
    {
        string TXT { get; }
    }

    public class RecordTXT : Record, IRecordTXT
    {
		public string TXT { get; private set; }

		public RecordTXT(RecordReader rr)
		{
			TXT = rr.ReadString();
		}

		public override string ToString()
		{
			return string.Format("\"{0}\"",TXT);
		}

	}
}
