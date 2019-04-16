using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class ClientProductAddOn
    {
        public string ClientProductAppId { get; set; }
        public string ProductAddOnId { get; set; }

        public ClientProductApp ClientProductApp { get; set; }
        public ProductAddOn ProductAddOn { get; set; }
    }
}
