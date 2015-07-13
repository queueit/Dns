// Copyright (c) Yamool. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.


namespace Yamool.Net.DNS
{
    using System;
    using Yamool.Net.DNS.Records;

    #region RFC info
    /*
	3.2. RR definitions

	3.2.1. Format

	All RRs have the same top level format shown below:

										1  1  1  1  1  1
		  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                                               |
		/                                               /
		/                      NAME                     /
		|                                               |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                      TYPE                     |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                     CLASS                     |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                      TTL                      |
		|                                               |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                   RDLENGTH                    |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--|
		/                     RDATA                     /
		/                                               /
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+


	where:

	NAME            an owner name, i.e., the name of the node to which this
					resource record pertains.

	TYPE            two octets containing one of the RR TYPE codes.

	CLASS           two octets containing one of the RR CLASS codes.

	TTL             a 32 bit signed integer that specifies the time interval
					that the resource record may be cached before the source
					of the information should again be consulted.  Zero
					values are interpreted to mean that the RR can only be
					used for the transaction in progress, and should not be
					cached.  For example, SOA records are always distributed
					with a zero TTL to prohibit caching.  Zero values can
					also be used for extremely volatile data.

	RDLENGTH        an unsigned 16 bit integer that specifies the length in
					octets of the RDATA field.

	RDATA           a variable length string of octets that describes the
					resource.  The format of this information varies
					according to the TYPE and CLASS of the resource record.
	*/
    #endregion

    /// <summary>
    /// The Resource Record class.
    /// </summary>
    public abstract class RR
    {
        /// <summary>
        /// The domain name to which this resource record pertains.
        /// </summary>
        public string NAME;

        /// <summary>
        /// Specifies type of resource record
        /// </summary>
        /// <remarks>This field specifies the meaning of the data in the RDATA  field.</remarks>
        public TYPE TYPE;

        /// <summary>
        /// Specifies type class of resource record, mostly IN but can be CS, CH or HS 
        /// </summary>
        public CLASS CLASS;

        /// <summary>
        /// An unsigned 16 bit integer that specifies the length in octets of the RDATA field.
        /// </summary>
        public ushort RDLENGTH;

        /// <summary>
        /// One of the Record* classes
        /// </summary>
        public Record RECORD;

        /// <summary>
        /// Specifies the time interval (in seconds) that the resource record may be cached before it should be discarded.
        /// </summary>
        /// <remarks>Zero values are interpreted to mean that the RR can only be used for the 
        /// transaction in progress, and should not be cached.</remarks>
        public uint TTL;

        public RR(RecordReader rr)
		{			
			this.NAME = rr.ReadDomainName();
			this.TYPE = (TYPE)rr.ReadUInt16();
			this.CLASS = (CLASS)rr.ReadUInt16();
			this.TTL = rr.ReadUInt32();
			this.RDLENGTH = rr.ReadUInt16();
            //begin to read a RDATA 
            this.RECORD = rr.ReadRecord(TYPE, RDLENGTH);
		}

        public override string ToString()
        {
            return string.Format("{0,-32} {1}\t{2}\t{3}\t{4}",
                this.NAME,
                this.TTL,
                this.CLASS,
                this.TYPE,
                this.RECORD);
        }
    }

    /// <summary>
    /// The answering for the question.
    /// </summary>
    public class AnswerRR : RR
    {
        public AnswerRR(RecordReader rr) : base(rr) { }
    }

    /// <summary>
    /// The authoritative name server. 
    /// </summary>
    public class AuthorityRR : RR
    {
        public AuthorityRR(RecordReader rr) : base(rr) { }
    }

    /// <summary>
    /// The additional which relate to the query, but are not strictly answers for the question.
    /// </summary>
    public class AdditionalRR : RR
    {
        public AdditionalRR(RecordReader rr) : base(rr) { }
    }

}
