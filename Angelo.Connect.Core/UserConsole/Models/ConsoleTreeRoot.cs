using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UserConsole
{
    public class ConsoleTreeRoot : GenericTree
    {
        public string TreeType { get; set; }

        public IEnumerable<GenericMenuItem> TreeMenu { get; set; }
    }
}
