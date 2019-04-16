using System;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace Angelo.Common.Mvc
{
    public static class HttpContextExtensions
    {
        public static string GetRelativeUrl(this HttpRequest request)
        {
            var path = request.Path;
            var queryString = request.QueryString.ToString().Trim();

            if(!string.IsNullOrEmpty(queryString) && queryString != "?")
            {
                if (queryString.StartsWith("?"))
                    path += queryString;
                else
                    path += "?" + queryString;
            }

            return path;
        }

        public static string GetRelativeUrlEncoded(this HttpRequest request)
        {
            var url = GetRelativeUrl(request);
            return WebUtility.UrlEncode(url);
        }
    }
}
