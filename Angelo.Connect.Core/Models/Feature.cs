using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class Feature
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public JToken Settings { get; set; }
    }
}
