using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.UI.ViewModels
{
    public class GenericTree
    {
        public string Title { get; set; }

        public IEnumerable<GenericTreeNode> RootNodes { get; set; }
      
    }
}
