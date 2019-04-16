using System;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Web.Config;
using Angelo.Connect.Services;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web
{
    public class ClientAdminContextAccessor : IContextAccessor<ClientAdminContext>
    {
        private IHttpContextAccessor _httpContextAccessor;
        private ClientManager _clientManager;
        private ClientAdminContext _currentContext;

        public ClientAdminContextAccessor(IHttpContextAccessor httpContextAccessor, 
                                          ClientManager clientManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _clientManager = clientManager;
        }

        public ClientAdminContext GetContext()
        {
            var session = _httpContextAccessor.HttpContext.Session;
            
            if (_currentContext == null)
            {
                _currentContext = GetFromRequestContext().Result;

                if (_currentContext == null)
                {
                    _currentContext = GetFromCurrentSiteContext().Result;
                }
            }

            return _currentContext;
        }

        private async Task<ClientAdminContext> GetFromRequestContext()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var route = httpContext.Request.Path.ToString();
            var query = httpContext.Request.Query;

            var context = new ClientAdminContext();

            if(route.StartsWith("/sys/clients/", StringComparison.OrdinalIgnoreCase))
            {
                var match = Regex.Match(route, @"/sys/clients/(?<tenant>[^\s/]*)");
                if (match.Success)
                {
                    var tenant = match.Groups["tenant"].Value;

                    context.Client = await _clientManager.GetByTenantKeyAsync(tenant);
                }
            }
            else if (query.ContainsKey("clientId"))
            {
                var clientId = query["clientId"];

                context.Client = await _clientManager.GetByIdAsync(clientId);
            }

            if (context.Client != null)
            {
                context.Product = await _clientManager.GetDefaultProductContextAsync(context.Client.Id);
                return context;
            }

            return null;
        }

        private async Task<ClientAdminContext> GetFromCurrentSiteContext()
        {
            var siteContext = _httpContextAccessor.HttpContext.GetTenant<SiteContext>();
            var context = new ClientAdminContext();

            context.Client = await _clientManager.GetByIdAsync(siteContext.Client.Id);
            context.Product = siteContext.ProductContext;

            return context;
        }

    }
}
