using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace Angelo.Connect.Widgets
{
    public static class MvcExtensions
    {
        public static HtmlString ToHtmlString(this IHtmlContent content)
        {
            var result = "";

            using (var output = new StringWriter())
            {
                content.WriteTo(output, HtmlEncoder.Default);
                result = output.ToString();
            }

            return new HtmlString(result);
        }
    }
}
