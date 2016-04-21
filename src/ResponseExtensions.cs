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
        public static IRecordPTR[] RecordsPTR(this IResponse response)
        {
            return response.GetAnswerRecords<IRecordPTR>(TYPE.PTR);
        }

        /// <summary>
        /// List of ARecord in Response.Answers
        /// </summary>
        public static IRecordA[] RecordsA(this IResponse response)
        {
            return response.GetAnswerRecords<IRecordA>(TYPE.A);
        }


        /// <summary>
        /// List of TXTRecord in Response.Answers
        /// </summary>
        public static IRecordTXT[] RecordsTXT(this IResponse response)
        {
            return response.GetAnswerRecords<IRecordTXT>(TYPE.TXT);                
        }


        /// <summary>
        /// List of CNAMERecord in Response.Answers
        /// </summary>
        public static IRecordCNAME[] RecordsCNAME(this IResponse response)
        {
            return response.GetAnswerRecords<IRecordCNAME>(TYPE.CNAME);                
        }

        /// <summary>
        /// List of AAAARecord in Response.Answers
        /// </summary>
        public static IRecordAAAA[] RecordsAAAA(this IResponse response)
        {
            return response.GetAnswerRecords<IRecordAAAA>(TYPE.AAAA);             
        }

        /// <summary>
        /// List of NSRecord in Response.Answers
        /// </summary>
        public static IRecordNS[] RecordsNS(this IResponse response)
        {
            return response.GetAnswerRecords<IRecordNS>(TYPE.NS);  
        }

        /// <summary>
        /// List of SOARecord in Response.Answers
        /// </summary>
        public static IRecordSOA[] RecordsSOA(this IResponse response)
        {
            return response.GetAnswerRecords<IRecordSOA>(TYPE.SOA);  
        }

        public static IRR[] RecordsRR(this IResponse response)
        {
            var list = new List<IRR>();
            foreach (IRR rr in response.Answers)
            {
                list.Add(rr);
            }
            foreach (IRR rr in response.Authorities)
            {
                list.Add(rr);
            }
            foreach (IRR rr in response.Additionals)
            {
                list.Add(rr);
            }
            return list.ToArray();
        }
    }
}
