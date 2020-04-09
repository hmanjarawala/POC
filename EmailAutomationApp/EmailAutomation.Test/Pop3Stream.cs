using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace EmailAutomation.Test
{
    [ExcludeFromCodeCoverage]
    internal class Pop3Stream : MailClientStream
    {
        public Pop3Stream(IDictionary<TcpCommand, string> dict) : base(dict) { }

        protected override void write(byte[] buffer, int offset, int count)
        {
            var command = Encoding.ASCII.GetString(buffer, offset, count);

            if(command.StartsWith("USER "))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.USER]));
            }
            else if(command.StartsWith("PASS "))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.PASS]));
            }
            else if (command.StartsWith("QUIT"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.LOGT]));
            }
            else if (command.StartsWith("STAT"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.STAT]));
            }
            else if (command.StartsWith("TOP"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.TOP]));
            }
            else if (command.StartsWith("DELE"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.DELE]));
            }
            else if (command.StartsWith("RETR"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.RETR]));
            }
            else if (command.StartsWith("LIST"))
            {
                _innerStream = new MemoryStream(Encoding.ASCII.GetBytes(dict[TcpCommand.LIST]));
            }
        }
    }
}
