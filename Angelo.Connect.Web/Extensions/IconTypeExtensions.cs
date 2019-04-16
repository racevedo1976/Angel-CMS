using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

using Angelo.Connect.Icons;

namespace Angelo.Connect.Web
{
    public static class IconTypeExtensions
    {
        public static HtmlString Render(this IconType icon)
        {
            return new HtmlString($"<i class=\"{icon.ToString()}\"></i>");
        }

    }
}
