using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Models
{
    public enum DbActionResult
    {
        Error = 0,
        Insert = 1,
        Update = 2,
        Delete = 3,
        NotFound = 4
    }
}
