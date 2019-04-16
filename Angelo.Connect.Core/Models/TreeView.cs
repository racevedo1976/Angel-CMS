using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class TreeView
    {
        private IContentBrowser _contentType;
        public TreeView()
        {
            Items = new List<TreeView>();
            
        }
        public string Id { get; set; }
        public string Title { get; set; }
        public string IconClass { get; set; }
        public string ContentBrowserType { get; set; }

        public List<TreeView> Items { get; set; }
        public bool HasChildren
        {
            get
            {
                return Items.Any();
            }
        }

        public string Text
        {
            get
            {
                return Title;
            }
        }
    }
}
