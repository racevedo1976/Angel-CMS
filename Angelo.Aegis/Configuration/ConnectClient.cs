using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;

namespace Angelo.Aegis.Configuration
{
    public class ConnectClient : IdentityServer4.Models.Client
    {
        public ConnectClient() : base()
        {

            ClientId = "Angelo.Connect.Web";
            ClientName = "Angelo Connect CMS by SchoolInSites";
            ClientUri = "http://schoolinsites.com";
            LogoUri = "/img/sis-logo.png";
            RequireConsent = false;

            //AccessTokenType = AccessTokenType.Jwt,
            AccessTokenLifetime = 360;
            AllowedGrantTypes = GrantTypes.HybridAndClientCredentials;
            AllowOfflineAccess = true;
            PrefixClientClaims = false;
            AlwaysIncludeUserClaimsInIdToken = false;

            AllowedScopes = new List<string>
            {
                Scopes.OpenId.Name,
                Scopes.Profile.Name,
                Scopes.Email.Name,
                Scopes.Security.Name,
                Scopes.Drive.Name
            };

            ClientSecrets = new List<Secret>
            {
                new Secret("aries".Sha256())
            };

            // Set per tenant
            RedirectUris = new List<string>();
            PostLogoutRedirectUris = new List<string>();
        }

          
        public void SetSigninRedirectUris(IEnumerable<string> redirectUris)
        {
            if (RedirectUris == null)
                RedirectUris = new List<string>();

            foreach (var uri in redirectUris)
            {
                if (uri.StartsWith("http://") || uri.StartsWith("https://"))
                {
                    // add the uri specifically as given
                    RedirectUris.Add(uri);
                }
                else
                {
                    // otherwise add for for both HTTP & HTTPS since we don't know which one
                    RedirectUris.Add($"http://{uri}");
                    RedirectUris.Add($"https://{uri}");
                }
            }

        }

        public void SetPostLogoutRedirectUris(IEnumerable<string> redirectUris)
        {
            if (PostLogoutRedirectUris == null)
                PostLogoutRedirectUris = new List<string>();

            foreach (var uri in redirectUris)
            {
                if (uri.StartsWith("http://") || uri.StartsWith("https://"))
                {
                    // add the uri specifically as given
                    PostLogoutRedirectUris.Add(uri);
                }
                else
                {
                    // otherwise add for for both HTTP & HTTPS since we don't know which one
                    PostLogoutRedirectUris.Add($"http://{uri}");
                    PostLogoutRedirectUris.Add($"https://{uri}");
                }
            }
        }

    }
}
