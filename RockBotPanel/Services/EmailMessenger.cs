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
        private static readonly NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public EmailMessenger(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
            try
            {
                smtpClient.Connect(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort, false);
            }
            catch(Exception e)
            {
                logger.Fatal($"Service is not connected. Swap to Development environment, if you want to see authentication settings. Error: {e.Message}");
                LogConnectionConfiguration();
            }
            try
            {
                smtpClient.Authenticate(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
            }
            catch (MailKit.ServiceNotConnectedException e)
            {
                logger.Fatal($"Service is not connected. Swap to Development environment, if you want to see authentication settings. Error: {e.Message}");
                LogConnectionConfiguration();
            }
            catch (MailKit.Security.AuthenticationException e)
            {
                logger.Fatal($"Authentication Exception. Swap to Development environment, if you want to see authentication settings. Error: {e.Message}");
                LogAuthenticationConfiguration();
            }
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

            try
            {
                await smtpClient.SendAsync(emailMessage);
            }
            catch(MailKit.ServiceNotConnectedException e)
            {
                logger.Fatal($"Service is not connected. Swap to Development environment, if you want to see authentication settings. Error: {e.Message}");
                LogConnectionConfiguration();
            }
            catch(MailKit.ServiceNotAuthenticatedException e)
            {
                logger.Fatal($"Authentication Exception. Swap to Development environment, if you want to see authentication settings. Error: {e.Message}");
                LogAuthenticationConfiguration();
            }
            catch(Exception e)
            {
                logger.Fatal($"Exception. Swap to Development environment, if you want to see email settings. Error: {e.Message}");
                LogConnectionConfiguration();
                LogAuthenticationConfiguration();
            }
        }

        private void LogConnectionConfiguration()
        {
            logger.Debug($"Server: {_emailConfiguration.SmtpServer}");
            logger.Debug($"Port: {_emailConfiguration.SmtpPort}");
        }

        private void LogAuthenticationConfiguration()
        {
            logger.Debug($"Username: {_emailConfiguration.SmtpUsername}");
            logger.Debug($"Password: {_emailConfiguration.SmtpPassword}");
        }
    }
}
