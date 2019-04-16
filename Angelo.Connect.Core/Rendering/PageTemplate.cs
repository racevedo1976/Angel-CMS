using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Rendering
{
    public class PageTemplate
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string PreviewImage { get; set; }
        public string ViewPath { get; set; }

        public IEnumerable<string> SeedData { get; set; }
    }
}
