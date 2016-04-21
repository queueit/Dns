/* Copyright (C) Alphons van der Heijden
 * http://www.codeproject.com/Articles/23673/DNS-NET-Resolver-C
 * Licensed under the Code Project Open License (CPOL)
 */

namespace Yamool.Net.DNS
{
    using System;
    using System.Text;
    using Yamool.Net.DNS.Records;

    /// <summary>
    /// The record reader that read record from a byte stream.
    /// </summary>
    public class RecordReader
    {
        private byte[] _buffer;
        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordReader"/> class.
        /// </summary>
        /// <param name="data">The buffer to be read.</param>
        public RecordReader(byte[] data) : this(data, 0) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="RecordReader"/> class.
        /// </summary>
        /// <param name="data">The buffer to be read.</param>
        /// <param name="position">The position which to begin read.</param>
        public RecordReader(byte[] data, int position)
        {
            _buffer = data;
            _position = position;
        }

        /// <summary>
        /// Gets or sets the current position within the record reader.
        /// </summary>
        public int Position
        {
            get
            {
                return _position;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException("value", "Non-negative number required.");
                }
                if (value >= this.Length)
                {
                    throw new ArgumentOutOfRangeException("value", "The Position value must be less than length of the record reader.");
                }
                _position = value;
            }
        }

        private int Length
        {
            get
            {
                return _buffer.Length;
            }
        }

        public IRecord ReadRecord(TYPE type, int Length)
        {
            switch (type)
            {
                case TYPE.A:
                    return new RecordA(this);
                case TYPE.NS:
                    return new RecordNS(this);
                case TYPE.MD:
                    return new RecordMD(this);
                case TYPE.MF:
                    return new RecordMF(this);
                case TYPE.CNAME:
                    return new RecordCNAME(this);
                case TYPE.SOA:
                    return new RecordSOA(this);
                case TYPE.MB:
                    return new RecordMB(this);
                case TYPE.MG:
                    return new RecordMG(this);
                case TYPE.MR:
                    return new RecordMR(this);
                case TYPE.NULL:
                    return new RecordNULL(this);
                case TYPE.WKS:
                    return new RecordWKS(this);
                case TYPE.PTR:
                    return new RecordPTR(this);
                case TYPE.HINFO:
                    return new RecordHINFO(this);
                case TYPE.MINFO:
                    return new RecordMINFO(this);
                case TYPE.MX:
                    return new RecordMX(this);
                case TYPE.TXT:
                    return new RecordTXT(this);
                case TYPE.RP:
                    return new RecordRP(this);
                case TYPE.AFSDB:
                    return new RecordAFSDB(this);
                case TYPE.X25:
                    return new RecordX25(this);
                case TYPE.ISDN:
                    return new RecordISDN(this);
                case TYPE.RT:
                    return new RecordRT(this);
                case TYPE.NSAP:
                    return new RecordNSAP(this);
                case TYPE.NSAPPTR:
                    return new RecordNSAPPTR(this);
                case TYPE.SIG:
                    return new RecordSIG(this);
                case TYPE.KEY:
                    return new RecordKEY(this);
                case TYPE.PX:
                    return new RecordPX(this);
                case TYPE.GPOS:
                    return new RecordGPOS(this);
                case TYPE.AAAA:
                    return new RecordAAAA(this);
                case TYPE.LOC:
                    return new RecordLOC(this);
                case TYPE.NXT:
                    return new RecordNXT(this);
                case TYPE.EID:
                    return new RecordEID(this);
                case TYPE.NIMLOC:
                    return new RecordNIMLOC(this);
                case TYPE.SRV:
                    return new RecordSRV(this);
                case TYPE.ATMA:
                    return new RecordATMA(this);
                case TYPE.NAPTR:
                    return new RecordNAPTR(this);
                case TYPE.KX:
                    return new RecordKX(this);
                case TYPE.CERT:
                    return new RecordCERT(this);
                case TYPE.A6:
                    return new RecordA6(this);
                case TYPE.DNAME:
                    return new RecordDNAME(this);
                case TYPE.SINK:
                    return new RecordSINK(this);
                case TYPE.OPT:
                    return new RecordOPT(this);
                case TYPE.APL:
                    return new RecordAPL(this);
                case TYPE.DS:
                    return new RecordDS(this);
                case TYPE.SSHFP:
                    return new RecordSSHFP(this);
                case TYPE.IPSECKEY:
                    return new RecordIPSECKEY(this);
                case TYPE.RRSIG:
                    return new RecordRRSIG(this);
                case TYPE.NSEC:
                    return new RecordNSEC(this);
                case TYPE.DNSKEY:
                    return new RecordDNSKEY(this);
                case TYPE.DHCID:
                    return new RecordDHCID(this);
                case TYPE.NSEC3:
                    return new RecordNSEC3(this);
                case TYPE.NSEC3PARAM:
                    return new RecordNSEC3PARAM(this);
                case TYPE.HIP:
                    return new RecordHIP(this);
                case TYPE.SPF:
                    return new RecordSPF(this);
                case TYPE.UINFO:
                    return new RecordUINFO(this);
                case TYPE.UID:
                    return new RecordUID(this);
                case TYPE.GID:
                    return new RecordGID(this);
                case TYPE.UNSPEC:
                    return new RecordUNSPEC(this);
                case TYPE.TKEY:
                    return new RecordTKEY(this);
                case TYPE.TSIG:
                    return new RecordTSIG(this);
                default:
                    return new RecordUnknown(this);
            }
        }

        /// <summary>        
        /// Reads a byte and advances the position by one byte,or returns 0 if at the end of the current record reader.
        /// </summary>
        public byte ReadByte()
        {
            if (this.Position >= this.Length)
            {
                return 0;
            }
            return this._buffer[_position++];
        }

        /// <summary>
        /// Reads a byte and case to a <see cref="char"/>.
        /// </summary>
        /// <returns></returns>
        private char ReadChar()
        {
            return (char)this.ReadByte();
        }

        /// <summary>
        /// Read a 16-bit unsigned integer.
        /// </summary>
        /// <returns>The unsigned integer.</returns>
        public UInt16 ReadUInt16()
        {
            return (UInt16)(this.ReadByte() << 8 | this.ReadByte());
        }

        /// <summary>
        /// Read a 16-bit unsigned integer at specified offset to begin.
        /// </summary>
        /// <param name="offset">The offset to begin read.</param>
        /// <returns>The unsigned integer.</returns>
        public UInt16 ReadUInt16(int offset)
        {
            this.Position += offset;
            return this.ReadUInt16();
        }

        /// <summary>
        /// Read a 32-bit unsigned integer.
        /// </summary>
        /// <returns>32-bit unsigned integer.</returns>
        public UInt32 ReadUInt32()
        {
            return (UInt32)(this.ReadUInt16() << 16 | this.ReadUInt16());
        }

        /// <summary>
        /// Reads a specified maximum number of bytes and advances the position by specified number of bytes.
        /// </summary>
        /// <param name="length">The maximum number of bytes to read.</param>
        /// <returns>The array of bytes.</returns>
        public byte[] ReadBytes(int length)
        {
            if (this.Position + length > this.Length)
            {
                throw new InvalidOperationException("The read bytes excetted the length of record reader");
            }
            var destBuff = new byte[length];
            for (var i = 0; i < length; i++)
            {
                destBuff[i] = this.ReadByte();
            }
            return destBuff;
        }

        /// <summary>
        /// Read a next block bytes and return as string.
        /// </summary>
        /// <returns>A string.</returns>
        public string ReadString()
        {
            short length = this.ReadByte();
            var sb = new StringBuilder(length);
            for (var i = 0; i < length; i++)
            {
                sb.Append(this.ReadChar());
            }
            return sb.ToString();
        }

        /// <summary>
        /// Read a Domain name.
        /// </summary>
        /// <returns></returns>
        public string ReadDomainName()
        {
            var name = new StringBuilder();
            int length = 0;           

            // get  the length of the first label
            while ((length = this.ReadByte()) != 0)
            {
                // top 2 bits set denotes domain name compression and to reference elsewhere
                //0xc0 -> Name is a pointer.
                if ((length & 0xc0) == 0xc0) 
                {
                    // work out the existing domain name, copy this pointer
                    var newRecordReader = new RecordReader(_buffer, (length & 0x3f) << 8 | this.ReadByte());

                    name.Append(newRecordReader.ReadDomainName());
                    return name.ToString();
                }

                // if not using compression, copy a char at a time to the domain name
                while (length > 0)
                {
                    name.Append(this.ReadChar());
                    length--;
                }
                name.Append('.');
            }
            if (name.Length == 0)
                return ".";
            else
                return name.ToString();
        }        
    }
}
