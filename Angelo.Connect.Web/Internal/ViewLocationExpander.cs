using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

using Angelo.Connect.Configuration;

namespace Angelo.Connect.Web
{
    public class ViewLocationExpander : IViewLocationExpander
    {
        private string _componentName; 

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {         
            // Areas
            yield return "~/UI/Views/{2}/{1}/{0}.cshtml";       ///UI/Views/{Area}/{Controller}/{View} 
            yield return "~/UI/Views/{2}/Shared/{0}.cshtml";    ///UI/Views/{Area}/Shared/{View}
            yield return "~/Views/{2}/{1}/{0}.cshtml";          ///Views/{Area}/{Controller}/{View} 
            yield return "~/Views/{2}/Shared/{0}.cshtml";       ///Views/{Area}/Shared/{View}
            yield return "~/Views/{1}/{0}.cshtml";              ///Views/{Controller}/{View} 
            yield return "~/Views/Shared/{0}.cshtml";           ///Views/Shared/{View}
            yield return "{0}";

            // Global
            yield return "~/UI/Views/Shared/{0}.cshtml";        ///UI/Views/Shared/{View}
            yield return "~/Views/Shared/{0}.cshtml";        ///Views/Shared/{View}

            // Components
            yield return "~/UI/Views/{0}.cshtml";               ///UI/Views/{Components/[ComponentName]/Default}
            yield return "~/Views/{0}.cshtml";                  ///Views/{Components/[ComponentName]/Default}

            if (!string.IsNullOrEmpty(_componentName))
            {
                yield return "~/UI/Views/Components2/CorpLevel/" + _componentName;
                yield return "~/UI/Views/Components2/ClientLevel/" + _componentName;
                yield return "~/UI/Views/Components2/SiteLevel/" + _componentName;
                yield return "~/UI/Views/Components2/Account/" + _componentName;
                yield return "~/UI/Views/Components2/Common/" + _componentName;
                yield return "~/UI/Views/Components2/UserLevel/" + _componentName;
            }
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            if (context.ViewName.StartsWith("Components/"))
            {
                var viewName = context.ViewName.Replace("/Default", "");
                var parts = viewName.Split(new char[] { '/' });

                if (parts.Length > 2)
                    _componentName = parts[2] + ".cshtml";
                else
                    _componentName = parts[1] + ".cshtml";

            }
        }
    }
}

