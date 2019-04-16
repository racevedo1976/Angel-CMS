using System;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Angelo.Connect.Configuration;

namespace Angelo.Connect.Web.Extensions
{
    public static class AegisStartupExtensions
    {
        public static void UseAegisOidc(this IApplicationBuilder app, IOptions<AegisOptions> aegisOptions, SiteContext siteContext )
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            var aegis = aegisOptions.Value;
            var client = siteContext.Client;

            // NOTE: Sign in scheme is set based on SiteContext, which is cached per domain entry point

            var perTenantAuthority = $"{aegis.Authority}/auth/{client.TenantKey}";
            var perTenantCookie = siteContext.AuthCookieName;

            // TODO: 
            // Ideally, we want to use "GetClaimsFromUserInfoEndpoint" but at this time
            // IdentityServer is not configured to use separate clients for each Connect 
            // Client, thus acr values are being used to specify the tenant which are only
            // passed on the initial request to get the user ticket, but not the subsequent 
            // requst to get profile
            var openIdOptions = new OpenIdConnectOptions()
            {
                Authority = perTenantAuthority,
                ClientId = aegis.ClientId,
                ClientSecret = aegis.ClientSecret,
                AuthenticationScheme = "oidc",                        
                SignInScheme = "cookies",
                ResponseType = "code id_token",
                RequireHttpsMetadata = false,
                GetClaimsFromUserInfoEndpoint = true,
                SaveTokens = true,
                
                TokenValidationParameters = new TokenValidationParameters()
                {
                    NameClaimType = aegis.NameClaimType,
                    RoleClaimType = aegis.RoleClaimType,
                },
                
                Events = app.ApplicationServices.GetRequiredService<OpenIdInterceptors>()
            };

            foreach (var scopeName in aegis.Scopes)
            {
                openIdOptions.Scope.Add(scopeName);
            }

            app.UseCookieAuthentication(new CookieAuthenticationOptions()
            {
                AuthenticationScheme = "cookies",
                AutomaticAuthenticate = true,
                ExpireTimeSpan = new TimeSpan(0, 30, 0),
                CookieName = perTenantCookie,
                CookieHttpOnly = false,
                CookieSecure = CookieSecurePolicy.SameAsRequest,
                AccessDeniedPath = "/sys/account/accessdenied",
                LoginPath = "/sys/account/login",
                LogoutPath = "/sys/account/logout",                                     
               
                Events = app.ApplicationServices.GetRequiredService<CookieAuthInterceptors>()
            });

            

            app.UseOpenIdConnectAuthentication(openIdOptions);
        }
    }
}
