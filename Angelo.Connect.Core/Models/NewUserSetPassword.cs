using Angelo.Common.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class NewUserSetPassword : IEmailTemplate
    {
        public string Subject { get; set; } = "Set Your Password.";
        public string Template { get; set; } = "~/UI/Views/Templates/Messaging/NewUserSetPassword.cshtml";
        public string EmailLink { get; set; }
        public string Username { get; set; }
    }
}

