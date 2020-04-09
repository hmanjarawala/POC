using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace EmailAutomation.Test
{
    [ExcludeFromCodeCoverage]
    internal class ImapStream : MailClientStream
    {
        public ImapStream(IDictionary<TcpCommand, string> dict) : base(dict) { }

        protected override void write(byte[] buffer, int offset, int count)
        {
            var command = Encoding.ASCII.GetString(buffer, offset, count);

            if (command.StartsWith("$ LOGIN "))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.LOGN]));
            }
            else if (command.StartsWith("$ LOGOUT"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.LOGT]));
            }
            else if (command.StartsWith("$ STATUS"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.STAT]));
            }
            else if (command.StartsWith("$ LIST"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.LIST]));
            }
            else if (command.StartsWith("$ CLOSE"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.CLOS]));
            }
            else if (command.StartsWith("$ STORE"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.DELE]));
            }
            else if (command.StartsWith("$ SELECT"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.SELE]));
            }
            else if (command.StartsWith("$ FETCH"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.FETC]));
            }
        }
    }
}
