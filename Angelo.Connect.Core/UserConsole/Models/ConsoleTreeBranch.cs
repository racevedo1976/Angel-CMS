using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UserConsole
{
    public class ConsoleTreeBranch
    {
        public string TreeType { get; set; }

        public IEnumerable<GenericTreeNode> Nodes { get; set; }
    }
}
