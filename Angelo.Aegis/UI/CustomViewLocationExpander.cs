using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;

namespace Angelo.Aegis.UI
{
    public class CustomViewLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(
            ViewLocationExpanderContext context,
            IEnumerable<string> viewLocations
        )
        {
            yield return "~/UI/Views/{1}/{0}.cshtml";
            yield return "~/UI/Views/Shared/{0}.cshtml";
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
        }
    }
}
