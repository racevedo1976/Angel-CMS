using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.CoreWidgets.UI.ViewModels
{
    public class ContactFormMessage
    {
        public string ContactFormId { get; set; }
        public string IPAddress { get; set; }
        [Required]
        public string SenderName { get; set; }
        [Required]
        public string SenderEmail { get; set; }
         [Required]
        public string MessageSubject { get; set; }
        [Required]
        public string MessageBody { get; set; }

    }
}
