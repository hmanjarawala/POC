using EmailAutoMationApplication.Model;
using EmailAutomationLibrary;
using EmailAutomationLibrary.Imap;
using EmailAutomationLibrary.Pop3;
using System.Collections.Generic;
using System.Windows.Forms;

namespace EmailAutoMationApplication.Presentor
{
    class EmailAutomationPresentor : IEmailAutomationPresentor
    {
        Form _frmView;

        public event ActionChangedEventHandler Actionchanged;

        public event ProgressValueChangedEventHandler ProgressValueChanged;

        public void SetView(Form frmView)
        {
            _frmView = frmView;
        }

        public IEnumerable<Mailbox> FetchMailboxes(EmailAutomationModel model)
        {
            var client = new ImapClient(model.Hostname, model.Port, model.IsSecure);
            return new EmailAutomation().FetchMailboxes(client, model.Username, model.Password);
        }

        public void PlayEmailAutomation(object objModel)
        {
            EmailAutomationModel model = objModel as EmailAutomationModel;

            var ea = new EmailAutomation();
            IMailClient client;

            ea.ActionChangedEvent += (s, e) =>
            {
                Actionchanged?.Invoke(s, e);
            };

            ea.ProgressValueChangedEvent += (s, e) =>
            {
                ProgressValueChanged?.Invoke(s, e);
            };

            if (model.IsProtocolImap)
            {
                client = new ImapClient(model.Hostname, model.Port, model.IsSecure);
            }
            else
            {
                client = new Pop3Client(model.Hostname, model.Port, model.IsSecure);
            }

            ea.MailCount = model.MailsToDisplay;

            ea.PlayEmailAutomation(client, model.Username, model.Password, model.MailSearchCriteria, model.MailBox);

            ea.ProgressValueChangedEvent -= (s, e) =>
            {
                ProgressValueChanged?.Invoke(s, e);
            };

            ea.ActionChangedEvent -= (s, e) =>
            {
                Actionchanged?.Invoke(s, e);
            };

            ea = null;
        }
    }

    interface IEmailAutomationPresentor
    {
        void SetView(Form frmView);

        event ProgressValueChangedEventHandler ProgressValueChanged;

        event ActionChangedEventHandler Actionchanged;

        IEnumerable<Mailbox> FetchMailboxes(EmailAutomationModel model);

        void PlayEmailAutomation(object objModel);
    }
}
