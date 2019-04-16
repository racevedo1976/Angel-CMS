using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.UserConsole;

namespace Angelo.Connect.Web.UI.UserConsole.ViewModels
{
    public class UserConsoleViewModel
    {
        public IEnumerable<FactoryViewResult> ExplorerViews { get; set; }

        public string DefaultRoute { get; set; }

        public string DefaultContent { get; set; }
    }
}
