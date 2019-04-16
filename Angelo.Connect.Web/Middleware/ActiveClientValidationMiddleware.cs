using Angelo.Connect.Web;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Configuration;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web
{
    public class ActiveClientValidationMiddleware
    {
        private IContextAccessor<AdminContext> _clientContextAccessor;
        private readonly RequestDelegate _next;
        private IContextAccessor<SiteAdminContext> _siteAdminContextAccessor;
        private IContextAccessor<SiteContext> _siteContextAccessor;
        private IContextAccessor<UserContext> _userContextAccessor;
        private const string redirectView = "/sys/oops";

        public ActiveClientValidationMiddleware(RequestDelegate next, 
            IContextAccessor<AdminContext> clientContextAccessor, 
            IContextAccessor<UserContext> userContextAccessor,
            IContextAccessor<SiteContext> siteContextAccessor,
            IContextAccessor<SiteAdminContext> siteAdminContextAccessor)
        {
            _next = next;
            _clientContextAccessor = clientContextAccessor;
            _userContextAccessor = userContextAccessor;
            _siteContextAccessor = siteContextAccessor;
            _siteAdminContextAccessor = siteAdminContextAccessor;
        }
        public async Task Invoke(HttpContext httpContext)
        {
            var route = httpContext.Request.Path.ToString().ToLower();
            UserContext userContext;

            if (route.StartsWith("/sys/clients"))
            {
                var adminContext = _clientContextAccessor.GetContext();
                if (adminContext.ClientContext.Client != null)
                {
                    userContext = _userContextAccessor.GetContext();
                    if (!adminContext.ClientContext.Client.Active && !userContext.IsCorpUser)
                    {
                        RedirectToInactivePage(httpContext);
                    }
                }
            }
            else if (route.StartsWith("/sys/sites"))
            {
                // retrieve site admin context and check site is published + client active flags
                userContext = _userContextAccessor.GetContext();
                var siteAdminContext = _siteAdminContextAccessor.GetContext();
                if (!siteAdminContext.Site.Published && !userContext.IsCorpUser)
                {
                    RedirectToInactivePage(httpContext);
                }

            }
            else if (route.Equals("/"))
            {
                // retrieve site context (not siteadmincontext) and check site active + client active flags
                userContext = _userContextAccessor.GetContext();
                var siteContext = _siteContextAccessor.GetContext();
                if ((!siteContext.Published || !siteContext.Client.Active) && !userContext.IsCorpUser)
                {
                    RedirectToInactivePage(httpContext);
                }
            }

            await _next(httpContext);
        }


        public void RedirectToInactivePage(HttpContext httpContext)
        {
            httpContext.Response.Redirect(redirectView, permanent: false);
            return;
            
        }
    }
}
