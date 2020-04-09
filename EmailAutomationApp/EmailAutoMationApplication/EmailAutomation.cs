using EmailAutomationLibrary;
using EmailAutomationLibrary.Imap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace EmailAutoMationApplication
{
    delegate void ProgressValueChangedEventHandler(object sender, ProgressValueChangeEventArgs e);
    delegate void ActionChangedEventHandler(object sender, ActionEventArgs e);

    internal class EmailAutomation
    {
        public event ProgressValueChangedEventHandler ProgressValueChangedEvent
        {
            add
            {
                this._progressValueChangedEventHandler += value;
            }
            remove
            {
                this._progressValueChangedEventHandler -= value;
            }
        }
        private ProgressValueChangedEventHandler _progressValueChangedEventHandler;

        public event ActionChangedEventHandler ActionChangedEvent
        {
            add
            {
                this.action += value;
            }
            remove
            {
                this.action -= value;
            }
        }
        private ActionChangedEventHandler action;

        //internal int MailStatus { get; set; }

        internal int MailCount { get; set; }

        public void PlayEmailAutomation(IMailClient client, string username, string password, string searchCriteria, string mailbox)
        {
            string retValue = string.Empty;
            int countMail = 0;

            retValue = client.LogIn(username, password);

            if (string.IsNullOrEmpty(retValue))
            {
                retValue = client.GetTotalMail(out countMail, mailbox);

                if (string.IsNullOrEmpty(retValue))
                {
                    if (countMail > 0)
                    {
                        var _messagesIndexs = GetMailIndex(client, searchCriteria, mailbox);

                        _messagesIndexs = (from p in _messagesIndexs
                                           orderby p descending
                                           select p).ToList();

                        PerformAction(client, _messagesIndexs, mailbox);
                    }
                    else
                    {
                        client.LogOut();
                        ((IDisposable)client).Dispose();
                    }
                }
            }

            if (!string.IsNullOrEmpty(retValue))
            {
                throw new Exception(retValue);
            }
        }

        public IEnumerable<Mailbox> FetchMailboxes(ImapClient client, string username, string password)
        {
            IEnumerable<Mailbox> list = null;

            string retval = client.LogIn(username, password);

            if (string.IsNullOrEmpty(retval))
            {
                retval = client.FetchMailboxes(out list);
                if (string.IsNullOrEmpty(retval))
                {
                    list.ToList().Sort();
                }
            }
            client.LogOut();

            if (!string.IsNullOrEmpty(retval))
            {
                throw new Exception(retval);
            }
            return list;
        }

        private void PerformAction(IMailClient client, IEnumerable<int> mailIndexes, string mailbox)
        {
            int cnt = 0;

            foreach (var mailIndex in mailIndexes)
            {
                ListViewItem item = null;

                var parser = client.FetchMailFromHeader(mailIndex, mailbox);
                item = new ListViewItem(parser.From.ToString());
                item.SubItems.Add(parser.Subject);
                item.SubItems.Add(parser.Headers["Date"]);
                item.Tag = client;

                //System.Threading.Thread.Sleep(50);
                action?.Invoke(this, new ActionEventArgs() { Item = item });
                //UpdateProgressBar(pBar, mailIndexes.Count(), ++cnt);
                _progressValueChangedEventHandler?.Invoke(this, new ProgressValueChangeEventArgs() { MinValue = 0, MaxValue = Math.Min(mailIndexes.Count(), MailCount) - 1, Value = cnt++ });
                if (MailCount > 0 && cnt == MailCount)
                    break;
            }
        }

        private List<int> GetMailIndex(IMailClient client, string searchCriteria, string mailbox)
        {
            IEnumerable<int> strMsgIds = null;

            string mailIndexs = string.Empty;

            if (string.IsNullOrWhiteSpace(searchCriteria))
                mailIndexs = client.GetAllMessagesFlag(out strMsgIds, mailbox);
            else
                mailIndexs = client.SearchMail(searchCriteria, out strMsgIds, mailbox);

            return (strMsgIds.ToList());
        }
    }

    class ActionEventArgs
    {
        public ListViewItem Item { get; set; }
    }

    class ProgressValueChangeEventArgs
    {
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public int Value { get; set; }
    }
}
