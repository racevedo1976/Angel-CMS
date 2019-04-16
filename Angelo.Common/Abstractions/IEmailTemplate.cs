using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Abstractions
{
    public interface IEmailTemplate
    {
        string Subject { get; set; }
        string Template { get; set; }
    }
}
