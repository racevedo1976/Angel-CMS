using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Configuration
{
    public class PasswordOptions
    {
        public int MinLength { get; set; } = 6;
        public bool RequireDigit { get; set; } = true;
        public bool RequireUpper { get; set; } = true;
        public bool RequireLower { get; set; } = true;
        public bool RequireSpecial { get; set; } = false;
    }
}
