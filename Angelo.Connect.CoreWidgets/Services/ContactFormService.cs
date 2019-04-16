using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Messaging;
using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.Widgets.Services;
using Angelo.Connect.CoreWidgets.UI.ViewModels;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class ContactFormService : JsonWidgetService<ContactForm>
    {

        private TemplateEmailService _messagingService;

        public ContactFormService(ConnectDbContext db, TemplateEmailService messagingService) : base(db)
        {
            _messagingService = messagingService;
        }
        
        public void Send(ContactFormMessage message)
        {
            var settings = GetModel(message.ContactFormId);

            var subject = $"Contact Form {settings.Name }: " + message.MessageSubject;
            var body = "<pre>";

            body += $"{message.SenderName} [{message.SenderEmail}]";
            body += "\n\n" + message.MessageBody + "\n\n";
            body += "IP Address: " + message.IPAddress;
            body += "</pre>";

            _messagingService.SendEmailAsync(settings.Recipients, subject, body).Wait();
        }

        public override ContactForm GetDefaultModel()
        {
            // returning empty instance of options
            return new ContactForm();
        }
    }
}
