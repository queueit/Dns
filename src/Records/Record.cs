/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

namespace Yamool.Net.DNS.Records
{
    using System;

	public abstract class Record
	{
		/// <summary>
		/// The Resource Record this RDATA record belongs to
		/// </summary>
		public RR RR;
	}
}
