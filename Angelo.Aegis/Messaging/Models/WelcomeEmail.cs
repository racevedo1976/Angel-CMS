using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Messaging.Models
{
    public class WelcomeEmail : IEmailTemplate
    {
        public string Subject { get; set; } = "Your account was created";
        public string Template { get; set; } = "~/UI/Views/Templates/Messaging/WelcomeEmail.cshtml";
        public string Username { get; set; }
        public string ConfirmEmailLink { get; set; }
    }
}
