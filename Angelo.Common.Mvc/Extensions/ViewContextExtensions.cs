using System;
using System.Net;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Common.Mvc
{
    public static class ViewContextExtensions
    {
        public static string GetRelativeUrl(this ViewContext context)
        {
            return HttpContextExtensions.GetRelativeUrl(context.HttpContext.Request);
        }

        public static string GetRelativeUrlEncoded(this ViewContext context)
        {
            return HttpContextExtensions.GetRelativeUrlEncoded(context.HttpContext.Request);
        }
    }
}
