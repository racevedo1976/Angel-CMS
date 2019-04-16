using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class ProductAddOn
    {
        public ProductAddOn()
        {
            ProductAddOnLinks = new List<ProductAddOnLink>();
            ClientProductAddOns = new List<ClientProductAddOn>();
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SchemaFile { get; set; }
        public bool Active { get; set; }

        public ICollection<ProductAddOnLink> ProductAddOnLinks { get; set; }
        public ICollection<ClientProductAddOn> ClientProductAddOns { get; set; }
    }
}
