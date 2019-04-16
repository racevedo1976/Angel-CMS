using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Messaging.Models
{
    public class ResetPasswordEmail : IEmailTemplate
    {
        public string Subject { get; set; } = "Reset Your Password";
        public string Template { get; set; } = "~/UI/Views/Templates/Messaging/ResetPasswordEmail.cshtml";
        public string Username { get; set; }
        public string EmailLink { get; set; }
    }
}
