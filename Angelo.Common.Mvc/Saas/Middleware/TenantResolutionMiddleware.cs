using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Angelo.Common.Mvc.Saas.Middleware
{
    public class TenantResolutionMiddleware<TTenant>
    {
        private readonly RequestDelegate next;
        private readonly ILogger log;

        public TenantResolutionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            Ensure.Argument.NotNull(next, nameof(next));
            Ensure.Argument.NotNull(loggerFactory, nameof(loggerFactory));

            this.next = next;
            this.log = loggerFactory.CreateLogger<TenantResolutionMiddleware<TTenant>>();
        }

        public async Task Invoke(HttpContext httpContext, ITenantResolver<TTenant> tenantResolver)
        {
            Ensure.Argument.NotNull(httpContext, nameof(httpContext));
            Ensure.Argument.NotNull(tenantResolver, nameof(tenantResolver));

            log.LogDebug("Resolving TenantContext using {loggerType}.", tenantResolver.GetType().Name);

            var tenantContext = await tenantResolver.ResolveAsync(httpContext);
            if (tenantContext != null)
            {
                log.LogDebug("TenantContext Resolved. Adding to HttpContext.");
                httpContext.SetTenantContext(tenantContext);
            }
            else
            {
                log.LogDebug("TenantContext Not Resolved.");
            }

            await next.Invoke(httpContext);
        }
    }
}
