using System;
using Microsoft.AspNetCore.Http;


namespace Angelo.Common.Mvc.Saas
{
    public class TenantPipelineBuilderContext<TTenant>
    {
        public TenantContext<TTenant> TenantContext { get; set; }
        public TTenant Tenant { get; set; }
    }
}
