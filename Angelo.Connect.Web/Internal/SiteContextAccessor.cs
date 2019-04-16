using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Menus;
using Angelo.Connect.Security;
using Angelo.Connect.Web.Config;
using Angelo.Identity;

namespace Angelo.Connect.Web
{
    public class SiteContextAccessor : IContextAccessor<SiteContext>
    {
        private IServiceProvider _serviceProvider;
        //private SiteContext _siteContext;

        public SiteContextAccessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public SiteContext GetContext()
        {
            //if (_siteContext == null)
            //{
                var httpContextAccessor = _serviceProvider.GetRequiredService<IHttpContextAccessor>();

               return httpContextAccessor.HttpContext.GetTenant<SiteContext>();
            //}

            //return _siteContext;
        }
    }
}
