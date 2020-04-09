namespace EmailAutoMationApplication.Model
{
    class EmailAutomationModel
    {
        public string Hostname { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool IsSecure { get; set; }

        public bool IsProtocolImap { get; set; }

        public string MailBox { get; set; }

        public string MailSearchCriteria { get; set; }

        public int MailsToDisplay { get; set; }
    }
}
