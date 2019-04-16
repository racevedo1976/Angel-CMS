using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class ViewTemplate
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public string StyleSheet { get; set; }
        public string PreviewPath { get; set; }

        public ViewTemplateType Type { get; set; }
    }
}
