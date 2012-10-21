using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Net.Mail;
using System.Net;

namespace Utilities
{
    public class SmtpMail
    {
        public static void Send(string from, string to, string subject, string body, string smtpHost, int smtpPort, string senderEmail, string senderPassword)
        {
            MailMessage mail = new MailMessage(from, to, subject, body);

            System.Net.Mail.SmtpClient client = new SmtpClient(smtpHost, smtpPort);
            client.Credentials = new NetworkCredential(senderEmail, senderPassword);
            client.EnableSsl = true;
            client.Send(mail);
        }
    }
}
