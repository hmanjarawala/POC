using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace EmailAutomation.Test
{
    internal enum TcpCommand { LOGN, LOGT, STAT, TOP, RETR, LIST, CLOS, USER, PASS, DELE, SELE, FETC };

    [ExcludeFromCodeCoverage]
    internal abstract class MailClientStream : Stream
    {
        protected readonly IDictionary<TcpCommand, string> dict;
        protected Stream _innerStream;

        public MailClientStream(IDictionary<TcpCommand, string> dict)
            : base()
        {
            this.dict = dict;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public override long Length
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _innerStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override int ReadByte()
        {
            return _innerStream.ReadByte();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            write(buffer, offset, count);
        }

        protected abstract void write(byte[] buffer, int offset, int count);
    }
}