using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;

using Angelo.Common.Mvc.Saas;
using Angelo.Aegis.Configuration;

namespace Angelo.Aegis.Internal
{
    public class AegisTenantClientStore : IClientStore
    {
        private AegisTenant _tenant = null;

        public AegisTenantClientStore(IHttpContextAccessor httpContextAccessor)
        {
            if (httpContextAccessor == null)
                throw new ArgumentNullException(nameof(httpContextAccessor));

            // just to be sure, we are in a tenant context
            var tenantContext = httpContextAccessor.HttpContext.GetTenantContext<AegisTenant>();
            if (tenantContext == null)
                throw new ArgumentNullException(nameof(tenantContext));

            _tenant = tenantContext.Tenant;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = _tenant.Clients.FirstOrDefault(x => x.ClientId == clientId);

            return Task.FromResult(client);
        }
    }
}
