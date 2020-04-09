using System.IO;

namespace EmailAutomationLibrary
{
    internal interface ITcpClientAdapter
    {
        int ReceiveTimeout { get; set; }

        bool Connected { get; }

        void Connect(string hostname, int port);

        Stream GetStream();

        void Close();
    }
}
