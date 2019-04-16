using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Angelo.Connect.UI.ViewModels
{
    public class GenericMenuItem
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Title { get; set; }

        public string LinkUrl { get; set; }

        public string LinkCss { get; set; }

        public string IconCss { get; set; }

        public Dictionary<string, string> Data { get; set; }
    }
}
