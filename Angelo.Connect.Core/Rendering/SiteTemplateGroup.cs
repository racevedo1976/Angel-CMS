using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Rendering
{
    public class SiteTemplateGroup
    {
        public SiteTemplateGroup()
        {
            Templates = new List<SiteTemplate>();
        }

        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<SiteTemplate> Templates { get; set; }
    }
}
