using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Icons;

namespace Angelo.Connect.Web.UI.ViewModels
{
    public class ErrorViewModel
    {
        public IconType Icon { get; set; } = IconType.Error;
        public string Message { get; set; }
        public string Details { get; set; }
        public Exception Exception { get; set; }
    }
}
