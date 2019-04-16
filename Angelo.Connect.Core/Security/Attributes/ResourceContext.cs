using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security
{
    public class ResourceContext : Attribute, IFilterFactory
    {
        private string _routeKey { get; set; }

        public bool IsReusable { get; } = false;

        public ResourceContext(string routeKey)
        {
            _routeKey = routeKey;    
        }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            return new ResourceContextFilter(_routeKey, serviceProvider);
        }

        public class ResourceContextFilter : IAuthorizationFilter, IOrderedFilter, IFilterMetadata
        {
            private string _routeKey;

            public int Order { get; } = -1;

            internal ResourceContextFilter(string routeKey, IServiceProvider serviceProvider)
            {
                _routeKey = routeKey;
            }

            public void OnAuthorization(AuthorizationFilterContext context)
            {
                if (!string.IsNullOrEmpty(_routeKey))
                {
                    var resourceId = context.RouteData.Values[_routeKey];

                }
            }
        }
    }
}
