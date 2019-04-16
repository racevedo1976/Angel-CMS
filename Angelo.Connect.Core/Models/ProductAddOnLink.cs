using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class ProductAddOnLink
    {
        public string ProductId { get; set; }
        public string ProductAddOnId { get; set; }

        public Product Product { get; set; }
        public ProductAddOn ProductAddOn { get; set; }
    }
}
