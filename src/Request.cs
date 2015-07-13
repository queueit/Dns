// Copyright (c) Yamool. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

namespace Yamool.Net.DNS
{
    using System;
    using System.Collections.Generic;
    using System.Net;

    /*
     All communications inside of the domain protocol are carried in a single
    format called a message.  The top level format of message is divided
    into 5 sections (some of which are empty in certain cases) shown below:

    +---------------------+
    |        Header       |
    +---------------------+
    |       Question      | the question for the name server
    +---------------------+

     * 
     */

    /// <summary>
    /// The dns request that container a message.
    /// </summary>
    public class Request
    {
        public readonly Header Header;

        private ICollection<Question> questions;

        public Request()
        {
            Header = new Header();
            Header.OPCODE = OPCODE.Query;
            Header.QDCOUNT = 0;
            questions = new List<Question>();
        }

        /// <summary>
        /// Add a <see cref="Question"/> to the question collection.
        /// </summary>
        /// <param name="question"></param>
        public void AddQuestion(Question question)
        {
            questions.Add(question);
        }

        /// <summary>
        /// Represents the request as a byte array.
        /// </summary>
        public byte[] Data
        {
            get
            {
                var data = new List<byte>();
                Header.QDCOUNT = (ushort)questions.Count;
                data.AddRange(Header.Data);
                foreach (Question q in questions)
                {
                    data.AddRange(q.Data);
                }
                return data.ToArray();
            }
        }

        internal int Retries
        {
            get;
            set;
        }

        internal EndPoint[] DnsServers
        {
            get;
            set;
        }

        internal int Timeout
        {
            get;
            set;
        }

    }
}
