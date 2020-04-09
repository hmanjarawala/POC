using System.IO;

namespace MailServerTester
{
    interface ITcpClientAdapter
    {
        int ReceiveTimeout { get; set; }

        bool Connected { get; }

        void Connect(string hostname, int port);

        Stream GetStream();

        void Close();
    }
}
