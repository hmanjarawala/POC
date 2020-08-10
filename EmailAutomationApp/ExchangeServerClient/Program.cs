using Microsoft.Exchange.WebServices.Data;
using System;
using System.Net;
using System.Windows.Forms;

namespace ExchangeServerClient
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            //string _uri = "https://outlook.office365.com/EWS/Exchange.asmx";
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmMain());

            //new ExchangeMailAutomation().FetchEmail();
        }
    }

    internal class ExchangeMailAutomation
    {
        public void FetchEmail()
        {
            try
            {
                ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2010_SP1);

                service.Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx");

                service.Credentials = new WebCredentials("h.manjarawala@hotmail.com", "gr@ndf@ther");

                Mailbox mb = new Mailbox("h.manjarawala@hotmail.com");

                FolderId fid1 = new FolderId(WellKnownFolderName.Inbox, mb);

                PropertySet itempropertyset = new PropertySet(BasePropertySet.FirstClassProperties);

                //PropertySet itempropertyset = new PropertySet(BasePropertySet.FirstClassProperties,ItemSchema.TextBody, EmailMessageSchema.Body);
                //itempropertyset.RequestedBodyType = BodyType.HTML;
                itempropertyset.RequestedBodyType = BodyType.Text;

                ItemView itemview = new ItemView(1000);

                itemview.PropertySet = itempropertyset;

                SearchFilter sf = new SearchFilter.SearchFilterCollection(LogicalOperator.And, new SearchFilter.IsEqualTo(EmailMessageSchema.IsRead, false));
                Console.WriteLine("in main method");
                FindItemsResults<Item> findResults = service.FindItems(fid1, sf, new ItemView(1000));
                Console.WriteLine("in main method1");
                service.LoadPropertiesForItems(findResults, PropertySet.FirstClassProperties);
                Console.WriteLine("in main method2");
                foreach (Item item in findResults.Items)
                {
                    Console.WriteLine("Mail found item id[" + item.Id + "]");
                    //EmailMessage message = (EmailMessage)item;

                    EmailMessage message = EmailMessage.Bind(service, item.Id, new PropertySet(BasePropertySet.FirstClassProperties, ItemSchema.UniqueBody));
                    Console.WriteLine("Sender id[" + message.Sender + "]");
                    String messsageBody = message.Body;
                    messsageBody = messsageBody.ToUpper();


                    foreach (Attachment attachment in message.Attachments)
                    {
                        Console.WriteLine(attachment.Name + " attachment.IsInline -" + attachment.IsInline);
                        String attachmentNameWithOutExtn = attachment.Name.Substring(0, attachment.Name.LastIndexOf("."));
                        attachmentNameWithOutExtn = attachmentNameWithOutExtn.ToUpper();
                        Console.WriteLine("Attachment name without extension " + attachmentNameWithOutExtn);
                        Console.WriteLine("messsageBody.Contains(attachmentNameWithOutExtn)" + messsageBody.Contains(attachmentNameWithOutExtn));
                        Console.WriteLine("messsageBody.IndexOf(attachmentNameWithOutExtn)" + messsageBody.IndexOf(attachmentNameWithOutExtn));
                        if (messsageBody.Contains("IMG") && messsageBody.Contains(attachmentNameWithOutExtn))
                        {
                            Console.WriteLine(attachment.Name + " ----------------is an inline image");
                        }
                        if (attachment is FileAttachment)
                        {
                            FileAttachment fileAttachment = attachment as FileAttachment;
                            Console.WriteLine("Attachment is a file attachment");
                        }
                    }

                    Console.WriteLine("Press any key to exit...");
                    Console.ReadKey();

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


        }
    }
}
