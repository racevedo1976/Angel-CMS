using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Rendering
{
    public class SiteTemplatePage
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string Template { get; set; }
        public string Master { get; set; }
        public string[] SeedData { get; set; }
    }
}
