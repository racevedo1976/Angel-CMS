using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Models
{
    public class ClientAddOnProducts
    {
        public string Id { get; set; }
        public string ClientId { get; set; }
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Add { get; set; }
    }
}
