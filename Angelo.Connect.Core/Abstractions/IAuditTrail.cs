using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IAuditTrail
    {
        string CreatedBy { get; set; }
    }
}
