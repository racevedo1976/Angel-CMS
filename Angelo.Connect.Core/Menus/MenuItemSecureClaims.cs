using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Configuration;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Icons;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;

namespace Angelo.Connect.Menus
{
    public class MenuItemSecureClaims : MenuItem
    {
        
        public List<SecurityClaim> AuthorizedClaims { get; set; }

        public MenuItemSecureClaims() : base()
        {
        }
   

        public override bool Authorize(UserContext user)
        {
            if (AuthorizedClaims == null || AuthorizedClaims.Count == 0)
            {
                // if no claims are specified assume available for anyone
                return true;
            }

            // return true if the user contains at least one claim in the list of valid claims
            return AuthorizedClaims.Any(valid =>
                user.SecurityClaims.Any(
                    claim => claim.Type == valid.Type
                    && ( 
                        claim.Value == valid.Value
                        || valid.Value == "*"           // eg, "*" means any claim value is sufficient
                    )
                )
            );
        }
    }
}
