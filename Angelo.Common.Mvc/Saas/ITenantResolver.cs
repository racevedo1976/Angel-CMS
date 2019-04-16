using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Angelo.Common.Mvc.Saas
{
    public interface ITenantResolver<TTenant>
    {
        Task<TenantContext<TTenant>> ResolveAsync(HttpContext context);
    }
}