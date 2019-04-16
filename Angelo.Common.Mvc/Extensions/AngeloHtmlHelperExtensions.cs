using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Angelo.Common.Mvc
{
    public static class AngeloHtmlHelperExtensions
    {
        public static AngeloHtmlBuilderFactory Angelo(this IHtmlHelper htmlHelper)
        {
            return new AngeloHtmlBuilderFactory(htmlHelper);
        }


    }


}

