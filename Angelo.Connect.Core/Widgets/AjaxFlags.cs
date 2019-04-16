using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Widgets
{
    [Flags]
    public enum AjaxFlags
    {
        NONE = 0,        
        POST = 1,
        PUT = 2,
        DELETE = 4,
        ALL = POST | PUT | DELETE
    }
}
