using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Messaging.Models
{
    public interface IEmailTemplate
    {
        string Subject { get; set; }
        string Template { get; set; }
    }
}
