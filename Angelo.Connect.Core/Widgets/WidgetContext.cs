using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.Configuration;
using Angelo.Connect.Security;

namespace Angelo.Connect.Widgets
{
    public class WidgetContext<TWidgetSettings> : WidgetContext where TWidgetSettings : IWidgetModel
    {
        public new TWidgetSettings Settings { get; internal set; }

        public WidgetContext(WidgetContext baseContext)
        {
            HttpContext = baseContext.HttpContext;
            SiteContext = baseContext.SiteContext;
            UserContext = baseContext.UserContext;
            HostUrl = baseContext.HostUrl;

            Settings = (TWidgetSettings)baseContext.Settings;
        }
    }

    public class WidgetContext
    {
        public IWidgetModel Settings { get; internal set; }


        public HttpContext HttpContext { get; internal set; }

        public SiteContext SiteContext { get; internal set; }

        public UserContext UserContext { get; internal set; }

        public string HostUrl { get; internal set; }
    }
}
