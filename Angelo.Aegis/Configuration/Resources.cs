using IdentityModel;
using System.Collections.Generic;

using IdentityServer4.Models;

namespace Angelo.Aegis.Configuration
{
    public class Resources
    {
        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                Scopes.OpenId.ToIdentityResource(),
                Scopes.Profile.ToIdentityResource(),
                Scopes.Email.ToIdentityResource(),
                Scopes.Security.ToIdentityResource(),
                Scopes.Drive.ToIdentityResource(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                Scopes.Drive.ToApiResource(
                    secret: new Secret("secret".Sha256()),
                    scopes: new Scope[]{
                        Scopes.OpenId
                    }
                )
            };
        }



    }
}