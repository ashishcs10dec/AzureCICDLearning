using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace MyTutor.Usefull
{
    public class SendEmails
    {
        public static log4net.ILog logger = log4net.LogManager.GetLogger(typeof(SendEmails));
        public static bool EmailSend(string SenderEmail, string Subject, string Message, bool IsBodyHtml = false)
        {
            bool status = false;
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("support@maitutors.com");
                mailMessage.Subject = Subject;
                mailMessage.Body = Message;
                mailMessage.IsBodyHtml = IsBodyHtml;
                mailMessage.To.Add(new MailAddress(SenderEmail));
                SmtpClient smtp = new SmtpClient();
                //for local enviornment
                //smtp.Host = "smtpout.secureserver.net";
                //for server
                smtp.Host = "relay-hosting.secureserver.net";
                smtp.EnableSsl = false;
                NetworkCredential networkCredential = new NetworkCredential();
                networkCredential.UserName = mailMessage.From.Address;
                networkCredential.Password = "ashishCS@10";
                smtp.UseDefaultCredentials = true;
                smtp.Credentials = networkCredential;
                smtp.Port = 25;
                smtp.Send(mailMessage);
                status = true;
                return status;
            }
            catch (Exception ex)
            {
                return status;
            }
        }
    }
}