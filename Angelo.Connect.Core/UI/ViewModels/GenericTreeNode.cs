using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.UI.ViewModels
{
    public class GenericTreeNode : GenericMenuItem
    {
        public string NodeType { get; set; }

        public bool HasChildren { get; set; }
    }
}
