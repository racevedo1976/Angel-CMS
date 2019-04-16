using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.Configuration
{
    public class ClientAdminContext
    {
        public Client Client { get; set; }
        public ProductContext Product { get; set; }
    }
}
