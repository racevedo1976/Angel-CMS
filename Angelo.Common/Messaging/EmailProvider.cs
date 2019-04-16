using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;


namespace Angelo.Common.Messaging
{
    public class EmailProvider
    {

        private SmtpOptions _options;

        public EmailProvider(IOptions<SmtpOptions> options)
        {
            _options = options.Value;
        }


        /// <summary>
        /// Sends an email using a string representation of recipient addresses
        /// </summary>
        /// <param name="to">Recipient address or multiple addresses seperated by a semicolon</param>
        /// <param name="from">The from address to display</param>
        /// <param name="subject">The message subject</param>
        /// <param name="content">The message body content</param>
        /// <param name="replyTo">An alternative replyTo address to display</param>
        /// <returns>Task</returns>
        public async Task SendAsync(string to, string from, string subject, string content, string replyTo = null)
        {
            var recipients = to.Split(';').Select(x => x.Trim());
            await SendAsync(recipients, from, subject, content, replyTo);
        }

        /// <summary>
        /// Sends an email using an enumeration of recipient addresses
        /// </summary>
        /// <param name="to">Recipient address or multiple addresses seperated by a comma</param>
        /// <param name="from">The from address to display</param>
        /// <param name="subject">The message subject</param>
        /// <param name="content">The message body content</param>
        /// <param name="replyTo">An alternative replyTo address to display</param>
        /// <returns>Task</returns>
        public async Task SendAsync(IEnumerable<string> to, string from, string subject, string content, string replyTo = null)
        {
            Ensure.Argument.NotNull(to, "to");
            Ensure.Argument.NotNullOrEmpty(from, "from");
            Ensure.Argument.NotNullOrEmpty(subject, "subject");
            Ensure.Argument.NotNullOrEmpty(content, "content");

            MimeMessage message = new MimeMessage();
            BodyBuilder body = new BodyBuilder();

            body.HtmlBody = content;

            message.To.AddRange(
                to.Select(x => new MailboxAddress("", x))
            );

            message.From.Add(new MailboxAddress("", from));
            message.Subject = subject;
            message.Body = body.ToMessageBody();

            await SendEmailAsync(message);
        }


        private async Task SendEmailAsync(MimeMessage message)
        {

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(_options.Server, _options.Port, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate("joey", "password");

                client.Send(message);
                client.Disconnect(true);
            }


            //using (var client = new SmtpClient())
            //{
            //    //await client.ConnectAsync(_options.Server, _options.Port, _options.RequireSsl).ConfigureAwait(false);
            //    await client.ConnectAsync(_options.Server, _options.Port, SecureSocketOptions.None).ConfigureAwait(false);

            //    // Disable XOAUTH2 authentication mechanism.
            //     client.AuthenticationMechanisms.Remove("XOAUTH2");

            //    if (_options.RequireAuthentication)
            //    {
            //        await client.AuthenticateAsync(_options.Credentials.Username, _options.Credentials.Password).ConfigureAwait(false);
            //    }

            //    await client.SendAsync(message).ConfigureAwait(false);
            //    await client.DisconnectAsync(true).ConfigureAwait(false);
            //}
        }
    }
}
