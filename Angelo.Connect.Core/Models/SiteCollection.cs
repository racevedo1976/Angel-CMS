using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class SiteCollection
    {
        public SiteCollection()
        {
            SiteCollectionMaps = new List<SiteCollectionMap>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string ClientId { get; set; }

        public Client Client { get; set; }
        public ICollection<SiteCollectionMap> SiteCollectionMaps { get; set; }
    }
}
