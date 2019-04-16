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
    public class MenuItemSecureCustom : MenuItem
    {
        
        public Func<UserContext, bool> AuthorizeCallback;

        public MenuItemSecureCustom() : base()
        {
        }
   

        public override bool Authorize(UserContext user)
        {
            if(AuthorizeCallback != null)
            {
                return AuthorizeCallback.Invoke(user);
            }

            return true;
        }
    }
}
