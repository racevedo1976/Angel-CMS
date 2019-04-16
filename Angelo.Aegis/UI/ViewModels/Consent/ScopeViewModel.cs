using System.Collections.Generic;
using System.Linq;

using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Angelo.Aegis.UI.ViewModels.Consent
{   
    public class ScopeViewModel
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool Emphasize { get; set; }
        public bool Required { get; set; }
        public bool Checked { get; set; }
    }
}
