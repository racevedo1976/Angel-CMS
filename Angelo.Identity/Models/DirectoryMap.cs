using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Models
{
    public class DirectoryMap
    {
        public string PoolId { get; set; }
        public string DirectoryId { get; set; }        
        public Directory Directory { get; set; }
        public SecurityPool SecurityPool { get; set; }
    }
}
