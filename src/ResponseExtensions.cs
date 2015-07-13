//----------------------------------------------------------------
// Copyright (c) Yamool Inc.  All rights reserved.
//----------------------------------------------------------------

namespace Yamool.Net.DNS
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Yamool.Net.DNS.Records;

    public static class ResponseExtensions
    {
        /// <summary>
        /// List of PTRRecord in Response.Answers
        /// </summary>
        public static RecordPTR[] RecordsPTR(this Response response)
        {
            return response.GetAnswerRecords<RecordPTR>(TYPE.PTR);
        }

        /// <summary>
        /// List of ARecord in Response.Answers
        /// </summary>
        public static RecordA[] RecordsA(this Response response)
        {
            return response.GetAnswerRecords<RecordA>(TYPE.A);
        }


        /// <summary>
        /// List of TXTRecord in Response.Answers
        /// </summary>
        public static RecordTXT[] RecordsTXT(this Response response)
        {
            return response.GetAnswerRecords<RecordTXT>(TYPE.TXT);                
        }


        /// <summary>
        /// List of CNAMERecord in Response.Answers
        /// </summary>
        public static RecordCNAME[] RecordsCNAME(this Response response)
        {
            return response.GetAnswerRecords<RecordCNAME>(TYPE.CNAME);                
        }

        /// <summary>
        /// List of AAAARecord in Response.Answers
        /// </summary>
        public static RecordAAAA[] RecordsAAAA(this Response response)
        {
            return response.GetAnswerRecords<RecordAAAA>(TYPE.AAAA);             
        }

        /// <summary>
        /// List of NSRecord in Response.Answers
        /// </summary>
        public static RecordNS[] RecordsNS(this Response response)
        {
            return response.GetAnswerRecords<RecordNS>(TYPE.NS);  
        }

        /// <summary>
        /// List of SOARecord in Response.Answers
        /// </summary>
        public static RecordSOA[] RecordsSOA(this Response response)
        {
            return response.GetAnswerRecords<RecordSOA>(TYPE.SOA);  
        }

        public static RR[] RecordsRR(this Response response)
        {
            var list = new List<RR>();
            foreach (RR rr in response.Answers)
            {
                list.Add(rr);
            }
            foreach (RR rr in response.Authorities)
            {
                list.Add(rr);
            }
            foreach (RR rr in response.Additionals)
            {
                list.Add(rr);
            }
            return list.ToArray();
        }
    }
}
