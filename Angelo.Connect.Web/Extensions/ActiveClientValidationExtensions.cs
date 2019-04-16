
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.Extensions
{
    public static class ActiveClientValidationExtensions
    {
        public static IApplicationBuilder RunActiveClientValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ActiveClientValidationMiddleware>();
        }
    }
}
