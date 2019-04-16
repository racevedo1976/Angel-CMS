using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using Angelo.Aegis.Configuration;
using Angelo.Aegis.Internal;
using Angelo.Aegis.Messaging.Models;

namespace Angelo.Aegis.Messaging
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link http://go.microsoft.com/fwlink/?LinkID=532713
    public class MessagingService
    {

        private SmtpOptions _smtpOptions;
        private EmailProvider _emailService;
        private TemplateService _templateService;

        private string _replyTo = "";

        public MessagingService(TemplateService templateService, EmailProvider emailService, IOptions<SmtpOptions> smtpOptions)
        {
            _emailService = emailService;
            _smtpOptions = smtpOptions.Value;
            _templateService = templateService;

            _replyTo = _smtpOptions.DefaultReplyToAddress;
        }

        /// <summary>
        /// Creates and sends an email from the supplied Template
        /// </summary>
        /// <typeparam name="TEmailTemplate">TEmailTemplate Type that implements IEmailTemplate</typeparam>
        /// <param name="recipients">Semicolon delimited list of recipients</param>
        /// <param name="model">The TEmailTemplate data instance</param>
        /// <returns>Task</returns>
        public Task SendEmailAsync<TEmailTemplate>(string recipients, TEmailTemplate model) where TEmailTemplate : IEmailTemplate
        {
            var messageBody = _templateService.Interpolate(model.Template, model);
            return _emailService.SendAsync(recipients, _replyTo, model.Subject, messageBody);
        }

        /// <summary>
        /// Creates and sends an email for a supplied subject and message
        /// </summary>
        /// <param name="recipients">Semicolon delimited list of recipients</param>
        /// <param name="subject">The message subject</param>
        /// <param name="message">The message body</param>
        /// <returns></returns>
        public Task SendEmailAsync(string recipients, string subject, string message)
        {
            return _emailService.SendAsync(recipients, _replyTo, subject, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="number"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
