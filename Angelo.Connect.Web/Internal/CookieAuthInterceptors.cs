using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Angelo.Connect.Web
{
    public class CookieAuthInterceptors : ICookieAuthenticationEvents
    {
        public Task RedirectToAccessDenied(CookieRedirectContext context)
        {
            return RedirectHttpOnly(context);
        }

        public Task RedirectToLogin(CookieRedirectContext context)
        {
            return RedirectHttpOnly(context);
        }

        public Task RedirectToLogout(CookieRedirectContext context)
        {
            return Task.FromResult(0);
        }

        public Task RedirectToReturnUrl(CookieRedirectContext context)
        {
            return Task.FromResult(0);
        }

        public Task SignedIn(CookieSignedInContext context)
        {
            return Task.FromResult(0);
        }

        public Task SigningIn(CookieSigningInContext context)
        {
            return Task.FromResult(0);
        }

        public Task SigningOut(CookieSigningOutContext context)
        {
            return Task.FromResult(0);
        }

        public Task ValidatePrincipal(CookieValidatePrincipalContext context)
        {
            return Task.FromResult(0);
        }


        private Task RedirectHttpOnly(CookieRedirectContext context)
        {
            if (context.Request.Path.ToString().Contains("/api/") && context.Response.StatusCode == (int)HttpStatusCode.OK)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
            else
            {
                context.Response.Redirect(context.RedirectUri);
            }

            return Task.FromResult(0);
        }
    }
}
