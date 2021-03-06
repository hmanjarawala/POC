﻿using EmailAutomationLibrary.BodyStructure;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;

namespace EmailAutomationLibrary
{
    internal abstract class MessageBuilder
    {
        internal MailMessage FromHeader(string text)
        {
            NameValueCollection header = ParseMailHeader(text);
            MailMessage m = new MailMessage();

            foreach(string key in header)
            {
                string value = header.GetValues(key)[0];

                try
                {
                    m.Headers.Add(key, value);
                }
                catch
                {
                    
                }
            }

            Match ma = Regex.Match(header["Subject"] ?? "", @"=\?([A-Za-z0-9\-_]+)");

            if (ma.Success)
            {
                m.SubjectEncoding = Util.GetEncoding(ma.Groups[1].Value);

                try
                {
                    m.Subject = Util.DecodeWords(header["Subject"]).
                        Replace(Environment.NewLine, "");
                }
                catch
                {
                    m.Subject = header["Subject"];
                }
            }
            else
            {
                m.SubjectEncoding = Encoding.ASCII;
                m.Subject = header["Subject"];
            }

            SetAddressFields(m, header);

            return m;
        }

        internal NameValueCollection ParseMailHeader(string header)
        {
            NameValueCollection headers = new NameValueCollection();
            StringReader reader = new StringReader(header);

            string line;
            string headerName = null;

            try
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if (line == string.Empty)
                        continue;

                    if (line[0] == ' ' || line[0] == '\t')
                    {
                        if (headerName != null)
                            headers[headerName] += line.Trim();
                        continue;
                    }

                    int colonIndx = line.IndexOf(':');

                    if (colonIndx < 0)
                        continue;

                    headerName = line.Substring(0, colonIndx);

                    //if (exclude.Contains(headerName))
                    //    continue;

                    string headerValue = line.Substring(colonIndx + 1).Trim();

                    headers.Add(headerName, headerValue);
                } 
            }
            catch (Exception ex)
            {
                
            }
            return headers;
        }

        internal abstract MailAddress[] ParseAddressList(string list);

        void SetAddressFields(MailMessage m, NameValueCollection header)
        {
            MailAddress[] addr;
            if (header["To"] != null)
            {
                addr = ParseAddressList(header["To"]);
                foreach (MailAddress a in addr)
                    m.To.Add(a);
            }
            if (header["Cc"] != null)
            {
                addr = ParseAddressList(header["Cc"]);
                foreach (MailAddress a in addr)
                    m.CC.Add(a);
            }
            if (header["Bcc"] != null)
            {
                addr = ParseAddressList(header["Bcc"]);
                foreach (MailAddress a in addr)
                    m.Bcc.Add(a);
            }
            if (header["From"] != null)
            {
                addr = ParseAddressList(header["From"]);
                if (addr.Length > 0)
                    m.From = addr[0];
            }
            if (header["Sender"] != null)
            {
                addr = ParseAddressList(header["Sender"]);
                if (addr.Length > 0)
                    m.Sender = addr[0];
            }
        }

        internal MailMessage FromMIME822(string text)
        {
            StringReader reader = new StringReader(text);
            StringBuilder header = new StringBuilder();
            string line;
            while (!string.IsNullOrEmpty(line = reader.ReadLine()))
                header.AppendLine(line);
            MailMessage m = FromHeader(header.ToString());
            MIMEPart[] parts = ParseMailBody(reader.ReadToEnd(), m.Headers);

            foreach (MIMEPart part in parts)
                AddBodypart(m, BodypartFromMIME(part), part.MessageText);

            return m;
        }

        MIMEPart[] ParseMailBody(string body,
            NameValueCollection header)
        {
            NameValueCollection contentType = ParseMIMEField(header["Content-Type"]);
            if (contentType["Boundary"] != null)
            {
                return ParseMIMEParts(new StringReader(body), contentType["Boundary"]);
            }
            else {
                return new MIMEPart[] {
                    new MIMEPart() { MessageText = body,
                        Headers = new NameValueCollection() {
                            { "Content-Type", header["Content-Type"] },
                            { "Content-Id", header["Content-Id"] },
                            { "Content-Transfer-Encoding", header["Content-Transfer-Encoding"] },
                            { "Content-Disposition", header["Content-Disposition"] }
                        }
                    }
                };
            }
        }

        protected abstract NameValueCollection ParseMIMEField(string field);

        MIMEPart[] ParseMIMEParts(StringReader reader, string boundary)
        {
            List<MIMEPart> list = new List<MIMEPart>();
            string start = "--" + boundary, end = "--" + boundary + "--", line;
            // Skip everything up to the first boundary.
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith(start))
                    break;
            }
            // Read the MIME parts which are delimited by boundary strings.
            while (line != null && line.StartsWith(start))
            {
                MIMEPart p = new MIMEPart();
                // Read the part header.
                StringBuilder header = new StringBuilder();
                while (!String.IsNullOrEmpty(line = reader.ReadLine()))
                    header.AppendLine(line);
                p.Headers = ParseMailHeader(header.ToString());
                // Account for nested multipart content.
                NameValueCollection contentType = ParseMIMEField(p.Headers["Content-Type"]);
                if (contentType["Boundary"] != null)
                    list.AddRange(ParseMIMEParts(reader, contentType["boundary"]));
                // Read the part body.
                StringBuilder body = new StringBuilder();
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.StartsWith(start))
                        break;
                    body.AppendLine(line);
                }
                p.MessageText = body.ToString();
                // Add the MIME part to the list unless body is null or empty which means the body
                // contained nested multipart content.
                if (p.MessageText != null && p.MessageText.Trim() != String.Empty)
                    list.Add(p);
                // If this boundary is the end boundary, we're done.
                if (line == null || line.StartsWith(end))
                    break;
            }
            return list.ToArray();
        }

        /// <summary>
		/// Glue method to create a bodypart from a MIMEPart instance.
		/// </summary>
		/// <param name="mimePart">The MIMEPart instance to create the bodypart instance from.</param>
		/// <returns>An initialized instance of the Bodypart class.</returns>
		Bodypart BodypartFromMIME(MIMEPart mimePart)
        {
            NameValueCollection contentType = ParseMIMEField(
                mimePart.Headers["Content-Type"]);
            Bodypart p = new Bodypart(null);
            Match m = Regex.Match(contentType["value"], "(.+)/(.+)");
            if (m.Success)
            {
                p.Type = ContentTypeMap.fromString(m.Groups[1].Value);
                p.Subtype = m.Groups[2].Value;
            }
            p.Encoding = ContentTransferEncodingMap.fromString(
                mimePart.Headers["Content-Transfer-Encoding"]);
            p.Id = mimePart.Headers["Content-Id"];
            foreach (string k in contentType.AllKeys)
                p.Parameters.Add(k, contentType[k]);
            p.Size = mimePart.MessageText.Length;
            if (mimePart.Headers["Content-Disposition"] != null)
            {
                NameValueCollection disposition = ParseMIMEField(
                    mimePart.Headers["Content-Disposition"]);
                p.Disposition.Type = ContentDispositionTypeMap.fromString(
                    disposition["value"]);
                p.Disposition.Filename = disposition["Filename"];
                foreach (string k in disposition.AllKeys)
                    p.Disposition.Attributes.Add(k, disposition[k]);
            }
            return p;
        }

        /// <summary>
		/// Parses a mail message identifier from a string.
		/// </summary>
		/// <param name="field">The field to parse the message id from</param>
		/// <exception cref="ArgumentException">The field argument does not contain a valid message
		/// identifier.</exception>
		/// <returns>The parsed message id.</returns>
		/// <remarks>A message identifier (msg-id) is a globally unique identifier for a
		/// message.</remarks>
		string ParseMessageId(string field)
        {
            // A msg-id is enclosed in < > brackets.
            Match m = Regex.Match(field, @"<(.+)>");
            if (m.Success)
                return m.Groups[1].Value;
            throw new ArgumentException("The field does not contain a valid message " +
                "identifier: " + field);
        }

        /// <summary>
		/// Adds a body part to an existing MailMessage instance.
		/// </summary>
		/// <param name="message">Extension method for the MailMessage class.</param>
		/// <param name="part">The body part to add to the MailMessage instance.</param>
		/// <param name="content">The content of the body part.</param>
		internal void AddBodypart(MailMessage message, Bodypart part, string content)
        {
            Encoding encoding = part.Parameters.ContainsKey("Charset") ?
                Util.GetEncoding(part.Parameters["Charset"]) : Encoding.ASCII;
            // Decode the content if it is encoded.
            byte[] bytes;
            try
            {
                switch (part.Encoding)
                {
                    case ContentTransferEncoding.QuotedPrintable:
                        bytes = encoding.GetBytes(Util.QPDecode(content, encoding));
                        break;
                    case ContentTransferEncoding.Base64:
                        bytes = Util.Base64Decode(content);
                        break;
                    default:
                        bytes = Encoding.ASCII.GetBytes(content);
                        break;
                }
            }
            catch
            {
                // If it's not a valid Base64 or quoted-printable encoded string just leave the data as is.
                bytes = Encoding.ASCII.GetBytes(content);
            }

            // If the part has a name it most likely is an attachment and it should go into the
            // Attachments collection.
            bool hasName = part.Parameters.ContainsKey("name");

            // If the MailMessage's Body fields haven't been initialized yet, put it there. Some weird
            // (i.e. spam) mails like to omit content-types so we don't check for that here and just
            // assume it's text.
            if (String.IsNullOrEmpty(message.Body) &&
                part.Disposition.Type != ContentDispositionType.Attachment)
            {
                message.Body = encoding.GetString(bytes);
                message.BodyEncoding = encoding;
                message.IsBodyHtml = part.Subtype.ToLower() == "html";
                return;
            }

            // Check for alternative view.
            string ContentType = ParseMIMEField(message.Headers["Content-Type"])["value"];
            bool preferAlternative = string.Compare(ContentType, "multipart/alternative", true) == 0;

            // Many attachments are missing the disposition-type. If it's not defined as alternative
            // and it has a name attribute, assume it is Attachment rather than an AlternateView.
            if (part.Disposition.Type == ContentDispositionType.Attachment ||
                (part.Disposition.Type == ContentDispositionType.Unknown &&
                preferAlternative == false && hasName))
                message.Attachments.Add(CreateAttachment(part, bytes));
            else
                message.AlternateViews.Add(CreateAlternateView(part, bytes));
        }

        /// <summary>
		/// Creates an instance of the AlternateView class used by the MailMessage class to store
		/// alternate views of the mail message's content.
		/// </summary>
		/// <param name="part">The MIME body part to create the alternate view from.</param>
		/// <param name="bytes">An array of bytes composing the content of the alternate view.</param>
		/// <returns>An initialized instance of the AlternateView class.</returns>
		AlternateView CreateAlternateView(Bodypart part, byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            System.Net.Mime.ContentType contentType;
            try
            {
                contentType = new System.Net.Mime.ContentType(
                    part.Type.ToString().ToLower() + "/" + part.Subtype.ToLower());
            }
            catch
            {
                contentType = new System.Net.Mime.ContentType();
            }
            AlternateView view = new AlternateView(stream, contentType);
            try
            {
                view.ContentId = ParseMessageId(part.Id);
            }
            catch { }
            return view;
        }

        /// <summary>
		/// Creates an instance of the Attachment class used by the MailMessage class to store mail
		/// message attachments.
		/// </summary>
		/// <param name="part">The MIME body part to create the attachment from.</param>
		/// <param name="bytes">An array of bytes composing the content of the attachment.</param>
		/// <returns>An initialized instance of the Attachment class.</returns>
		Attachment CreateAttachment(Bodypart part, byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            string name = part.Disposition.Filename;
            // Many MUAs put the file name in the name parameter of the content-type header instead of
            // the filename parameter of the content-disposition header.
            if (String.IsNullOrEmpty(name) && part.Parameters.ContainsKey("name"))
                name = part.Parameters["name"];
            if (String.IsNullOrEmpty(name))
                name = Path.GetRandomFileName();
            Attachment attachment = new Attachment(stream, name);
            try
            {
                attachment.ContentId = ParseMessageId(part.Id);
            }
            catch { }
            try
            {
                attachment.ContentType = new System.Net.Mime.ContentType(
                    part.Type.ToString().ToLower() + "/" + part.Subtype.ToLower());
            }
            catch
            {
                attachment.ContentType = new System.Net.Mime.ContentType();
            }
            // Workaround: filename from Attachment constructor is ignored with Mono.
            attachment.Name = name;
            attachment.ContentDisposition.FileName = name;
            return attachment;
        }
    }
}
