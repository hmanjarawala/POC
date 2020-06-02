using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ExchangeServerClient.Data
{
    public class DbManager
    {
        public string Provider { get; set; }

        public string DataSource { get; set; }

        public string Database { get; set; }

        public bool UseWindowAuthentication { get; set; }

        public string DbUserName { get; set; }

        public string DbPassword { get; set; }

        public string ExtraParameter { get; set; }

        public string CustomConnectionString { protected get; set; }

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(CustomConnectionString?.Trim()))
                {
                    if (string.IsNullOrEmpty(Provider?.Trim()))
                        throw new ArgumentNullException(nameof(Provider));
                    if (string.IsNullOrEmpty(DataSource?.Trim()))
                        throw new ArgumentNullException(nameof(DataSource));
                    if (string.IsNullOrEmpty(Database?.Trim()))
                        throw new ArgumentNullException(nameof(Database));
                    if (!UseWindowAuthentication)
                    {
                        if (string.IsNullOrEmpty(DbUserName?.Trim()))
                            throw new ArgumentNullException(nameof(DbUserName));
                        if (string.IsNullOrEmpty(DbPassword?.Trim()))
                            throw new ArgumentNullException(nameof(DbPassword));
                    }
                    var connectionBuilder = new StringBuilder();
                    connectionBuilder.AppendFormat("Provider={0}", Provider.Trim());
                    connectionBuilder.AppendFormat(";Data Source={0}", Database.Trim());
                    connectionBuilder.AppendFormat(";Initial Catalog={0}", Database.Trim());
                    if (!string.IsNullOrEmpty(ExtraParameter.Trim()))
                        connectionBuilder.AppendFormat(";{0}", ExtraParameter.Trim());
                    return connectionBuilder.ToString();
                }
                else
                {
                    return CustomConnectionString;
                }
            }
        }

        public void InsertEmailDetails(EmailMessage message)
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            var transaction = connection.BeginTransaction();

            try
            {
                var query = string.Concat("Insert Into MailMaster(EmailId,EmailFrom,EmailBody,EmailSubject,ReceipientDate,",
                    "SenderDate,IsRead,HasAttachments)", " Values(@EmailId,@EmailFrom,@EmailBody,@EmailSubject,",
                    "@ReceipientDate,@SenderDate,@IsRead,@HasAttachment)");
                SqlParameter[] sqlParameters = new SqlParameter[]
                {
                    new SqlParameter("@EmailId", message.Id.UniqueId),
                    new SqlParameter("@EmailFrom", message.From.Address),
                    new SqlParameter("@EmailBody", message.Body.Text),
                    new SqlParameter("@EmailSubject", message.Subject),
                    new SqlParameter("@ReceipientDate", message.DateTimeReceived),
                    new SqlParameter("@SenderDate", message.DateTimeSent),
                    new SqlParameter("@IsRead", (message.IsRead) ? 1 : 0),
                    new SqlParameter("@HasAttachment", (message.HasAttachments) ? 1 : 0)
                };
                SqlHelper.ExecuteNonQuery(transaction,
                    CommandType.Text,
                    query,
                    sqlParameters);
                sqlParameters = null;

                InsertEmailAddressDetail(message.Id, message.ToRecipients, "TO", transaction);

                InsertEmailAddressDetail(message.Id, message.CcRecipients, "CC", transaction);

                InsertEmailAddressDetail(message.Id, message.BccRecipients, "BCC", transaction);

                InsertEmailAttachmentDetail(message.Id, message.Attachments, transaction);

                transaction.Commit();
            }
            catch (Exception e)
            {
                transaction.Rollback();
            }
            finally
            {
                connection.Close();
            }
        }

        private static void InsertEmailAttachmentDetail(ItemId messageID, AttachmentCollection attachments, 
            SqlTransaction transaction)
        {
            var attachmentPath = string.Concat(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"\Attachment");
            if (!Directory.Exists(attachmentPath))
                Directory.CreateDirectory(attachmentPath);

            foreach (var attachment in attachments)
            {
                ((FileAttachment)attachment).Load(string.Concat(attachmentPath, "\\", attachment.Name));
                var query = string.Concat("Insert Into Mail_Attachment_Detail(EmailId,AttachmentPath) ",
                    "Values(@EmailId,@AttachmentPath)");
                var sqlParameters = new SqlParameter[]
                {
                        new SqlParameter("@EmailId", messageID.UniqueId),
                        new SqlParameter("@AttachmentPath", string.Concat(attachmentPath, "\\", attachment.Name))
                };
                SqlHelper.ExecuteNonQuery(transaction,
                CommandType.Text,
                query,
                sqlParameters);
                sqlParameters = null;
            }
        }

        private static void InsertEmailAddressDetail(ItemId messageID, EmailAddressCollection addresses, 
            string emailAddressType, SqlTransaction transaction)
        {
            foreach (var address in addresses)
            {
                var query = string.Concat("Insert Into Mail_Address_Detail(EmailId,EmailAddress,EmailAddressType) ",
                    "Values(@EmailId,@EmailAddress,@EmailAddressType)");
                var sqlParameters = new SqlParameter[]
                {
                        new SqlParameter("@EmailId", messageID.UniqueId),
                        new SqlParameter("@EmailAddress", address.Address),
                        new SqlParameter("@EmailAddressType", emailAddressType),
                };
                SqlHelper.ExecuteNonQuery(transaction,
                CommandType.Text,
                query,
                sqlParameters);
                sqlParameters = null;
            }
        }
    }
}
