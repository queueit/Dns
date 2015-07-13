/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

namespace Yamool.Net.DNS
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Net;

    #region Rfc 1034/1035
    /*
	4.1.2. Question section format

	The question section is used to carry the "question" in most queries,
	i.e., the parameters that define what is being asked.  The section
	contains QDCOUNT (usually 1) entries, each of the following format:

										1  1  1  1  1  1
		  0  1  2  3  4  5  6  7  8  9  0  1  2  3  4  5
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                                               |
		/                     QNAME                     /
		/                                               /
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                     QTYPE                     |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+
		|                     QCLASS                    |
		+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+--+

	where:

	QNAME           a domain name represented as a sequence of labels, where
					each label consists of a length octet followed by that
					number of octets.  The domain name terminates with the
					zero length octet for the null label of the root.  Note
					that this field may be an odd number of octets; no
					padding is used.

	QTYPE           a two octet code which specifies the type of the query.
					The values for this field include all codes valid for a
					TYPE field, together with some more general codes which
					can match more than one type of RR.


	QCLASS          a two octet code that specifies the class of the query.
					For example, the QCLASS field is IN for the Internet.
	*/
    #endregion
    
    /// <summary>
    /// The question section of message.
    /// </summary>
    public class Question
    {      
        private Question() { }

        public Question(string qName, QTYPE qType, QCLASS qClass)
        {
            this.QName = qName;
            this.QType = qType;
            this.QClass = qClass;
        }

        public Question(RecordReader rr)
        {
            this.QName = rr.ReadDomainName();
            this.QType = (QTYPE)rr.ReadUInt16();
            this.QClass = (QCLASS)rr.ReadUInt16();
        }

      
        /// <summary>
        /// The domain name represented as a sequence of labels.
        /// </summary>
        public string QName
        {
            get;
            set;
        }

        /// <summary>
        /// The type of the query.
        /// </summary>
        public QTYPE QType
        {
            get;
            set;
        }

        /// <summary>
        /// The class of the query
        /// </summary>
        public QCLASS QClass
        {
            get;
            set;
        }

        /// <summary>
        /// Represents the question as a byte array
        /// </summary>
        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                data.AddRange(WriteName(QName));
                data.AddRange(WriteShort((ushort)QType));
                data.AddRange(WriteShort((ushort)QClass));
                return data.ToArray();
            }
        }

        private byte[] WriteName(string src)
        {
            if (!src.EndsWith("."))
            {
                src += ".";
            }
            if (src == ".")
            {
                return new byte[1];
            }

            var sb = new StringBuilder();
            int i, j, length = src.Length;
            sb.Append('\0');
            for (i = 0, j = 0; i < length; i++, j++)
            {
                sb.Append(src[i]);
                if (src[i] == '.')
                {
                    sb[i - j] = (char)(j & 0xff);
                    j = -1;
                }
            }
            sb[sb.Length - 1] = '\0';
            return Encoding.ASCII.GetBytes(sb.ToString());
        }

        private byte[] WriteShort(ushort value)
        {
            return BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)value));
        }

        public override string ToString()
        {
            return string.Format("{0,-32}\t{1}\t{2}", this.QName, this.QClass, this.QType);
        }
    }
}
