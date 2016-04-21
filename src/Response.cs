// Copyright (c) Yamool. All rights reserved.
// Licensed under the MIT license. See License.txt file in the project root for full license information.

namespace Yamool.Net.DNS
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using Yamool.Net.DNS.Records;

    #region RFC 1035
    /*
     4.1. Format

    All communications inside of the domain protocol are carried in a single
    format called a message.  The top level format of message is divided
    into 5 sections (some of which are empty in certain cases) shown below:

        +---------------------+
        |        Header       |
        +---------------------+
        |       Question      | the question for the name server
        +---------------------+
        |        Answer       | RRs answering the question
        +---------------------+
        |      Authority      | RRs pointing toward an authority
        +---------------------+
        |      Additional     | RRs holding additional information
        +---------------------+

    The header section is always present.  The header includes fields that
    specify which of the remaining sections are present, and also specify
    whether the message is a query or a response, a standard query or some
    other opcode, etc.

    The names of the sections after the header are derived from their use in
    standard queries.  The question section contains fields that describe a
    question to a name server.  These fields are a query type (QTYPE), a
    query class (QCLASS), and a query domain name (QNAME).  The last three
    sections have the same format: a possibly empty list of concatenated
    resource records (RRs).  The answer section contains RRs that answer the
    question; the authority section contains RRs that point toward an
    authoritative name server; the additional records section contains RRs
    which relate to the query, but are not strictly answers for the
    question.
     */
    #endregion

    public interface IResponse
    {
        /// <summary>
        /// Return a specified TYPE of Record list.
        /// </summary>
        /// <typeparam name="T">Return <see cref="Record"/> type.</typeparam>
        /// <param name="recordType">The record TYPE</param>
        /// <returns>Return a list of Record</returns>
        T[] GetAnswerRecords<T>(TYPE recordType) where T : IRecord;

        /// <summary>
        /// The Header of response.
        /// </summary>
        Header Header { get; }

        /// <summary>
        /// Return a response code for the dns query.
        /// </summary>
        RCODE ResponseCode { get; }

        /// <summary>
        /// The Server which delivered this response
        /// </summary>
        EndPoint Server { get; }

        /// <summary>
        /// List of Question records
        /// </summary>
        List<IQuestion> Questions { get; }

        /// <summary>
        /// List of AnswerRR records
        /// </summary>
        List<IAnswerRR> Answers { get; }

        /// <summary>
        /// List of AuthorityRR records
        /// </summary>
        List<IAuthorityRR> Authorities { get; }

        /// <summary>
        /// List of AdditionalRR records
        /// </summary>
        List<IAdditionalRR> Additionals { get; }

        /// <summary>
        /// TimeStamp when inited.
        /// </summary>
        DateTime TimeStamp { get; }

        /// <summary>
        /// The message length of response.
        /// </summary>
        int MessageSize { get; }
    }

    /// <summary>
    /// The response of the dns question from server
    /// </summary>
    public class Response : IResponse
    {
        public Response()
        {
            this.Server = new IPEndPoint(0, 0);
            this.Questions = new List<IQuestion>();
            this.Answers = new List<IAnswerRR>();
            this.Authorities = new List<IAuthorityRR>();
            this.Additionals = new List<IAdditionalRR>();
            this.Header = new Header();
            this.TimeStamp = DateTime.Now;
        }

        public Response(EndPoint server, byte[] data)
        {
            this.TimeStamp = DateTime.Now;
            this.Server = server;         
            var rr = new RecordReader(data);
            this.Header = new Header(rr);
            this.MessageSize = data.Length;

            this.Questions = new List<IQuestion>(this.Header.QDCOUNT);
            for (var i = 0; i < this.Header.QDCOUNT; i++)
            {
                this.Questions.Add(new Question(rr));
            }

            this.Answers = new List<IAnswerRR>(this.Header.ANCOUNT);
            for (var i = 0; i < this.Header.ANCOUNT; i++)
            {
                this.Answers.Add(new AnswerRR(rr));
            }
            this.Authorities = new List<IAuthorityRR>(this.Header.NSCOUNT);
            for (var i = 0; i < this.Header.NSCOUNT; i++)
            {
                this.Authorities.Add(new AuthorityRR(rr));
            }
            this.Additionals = new List<IAdditionalRR>(this.Header.ARCOUNT);
            for (var i = 0; i < this.Header.ARCOUNT; i++)
            {
                this.Additionals.Add(new AdditionalRR(rr));
            }
        }
        
        /// <summary>
        /// Return a specified TYPE of Record list.
        /// </summary>
        /// <typeparam name="T">Return <see cref="Record"/> type.</typeparam>
        /// <param name="recordType">The record TYPE</param>
        /// <returns>Return a list of Record</returns>
        public T[] GetAnswerRecords<T>(TYPE recordType) where T : IRecord
        {
            var list = new List<T>();
            foreach (var answerRR in this.Answers)
            {
                if (answerRR.TYPE == recordType)
                {
                    list.Add((T)answerRR.RECORD);
                }
            }
            return list.ToArray();
        }

        /// <summary>
        /// The Header of response.
        /// </summary>
        public Header Header
        {
            get;
            private set;
        }

        /// <summary>
        /// Return a response code for the dns query.
        /// </summary>
        public RCODE ResponseCode
        {
            get
            {
                return this.Header.RCODE;
            }
        }

        /// <summary>
        /// The Server which delivered this response
        /// </summary>
        public EndPoint Server
        {
            get;
            private set;
        }

        /// <summary>
        /// List of Question records
        /// </summary>
        public List<IQuestion> Questions
        {
            get;
            private set;
        }

        /// <summary>
        /// List of AnswerRR records
        /// </summary>
        public List<IAnswerRR> Answers
        {
            get;
            private set;
        }

        /// <summary>
        /// List of AuthorityRR records
        /// </summary>
        public List<IAuthorityRR> Authorities
        {
            get;
            private set;
        }

        /// <summary>
        /// List of AdditionalRR records
        /// </summary>
        public List<IAdditionalRR> Additionals
        {
            get;
            private set;
        }

        /// <summary>
        /// TimeStamp when inited.
        /// </summary>
        public DateTime TimeStamp
        {
            get;
            private set;
        }

        /// <summary>
        /// The message length of response.
        /// </summary>
        public int MessageSize
        {
            get;
            private set;
        }
    }    
}
