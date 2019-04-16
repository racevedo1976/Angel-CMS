using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class Product
    {
        public Product()
        {
            ClientProductApps = new List<ClientProductApp>();
            ProductAddOnLinks = new List<ProductAddOnLink>();
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string SchemaFile { get; set; }
        public string CategoryId { get; set; }  // FK for ProductCategory ID
        public bool Active { get; set; }

        public ProductCategory Category { get; set; }

        public ICollection<ClientProductApp> ClientProductApps { get; set; }
        public ICollection<ProductAddOnLink> ProductAddOnLinks { get; set; }
    }
}