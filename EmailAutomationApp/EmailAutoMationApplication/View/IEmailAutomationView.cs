namespace EmailAutoMationApplication.View
{
    interface IEmailAutomationView
    {
        string Hostname { get; set; }

        int PortNumber { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        bool IsSecure { get; set; }

        int SetDefaultPort(bool isImap, bool isSecure);

        void FetchEmailFolder();
    }
}
