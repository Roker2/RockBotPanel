using MailKit.Net.Smtp;
using MimeKit;
using RockBotPanel.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RockBotPanel.Services
{
    public interface IEmailMessenger
    {
        void SendMsg(Models.EmailMessage msg);
    }
    public class EmailMessenger : IEmailMessenger
    {
        private readonly IEmailConfiguration _emailConfiguration;
        private readonly SmtpClient smtpClient = new SmtpClient();
        public EmailMessenger(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
            smtpClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);
            smtpClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
        }
        ~EmailMessenger()
        {
            smtpClient.Disconnect(true);
        }

        public async void SendMsg(Models.EmailMessage msg)
        {
            MimeMessage emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("Rock Bot Panel", _emailConfiguration.SmtpUsername));
            foreach(string address in msg.ToAddresses)
            {
                emailMessage.To.Add(new MailboxAddress("User", address));
            }
            emailMessage.Subject = msg.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = msg.Content };

            await smtpClient.SendAsync(emailMessage);
        }
    }
}
