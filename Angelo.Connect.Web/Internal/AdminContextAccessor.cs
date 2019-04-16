using System;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Web.Config;
using Angelo.Connect.Services;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web
{
    public class AdminContextAccessor : IContextAccessor<AdminContext>
    {
        //private IHttpContextAccessor _httpContextAccessor;
        private UserContextAccessor _userContextAccessor;
        private ClientAdminContextAccessor _clientAdminContextAccessor;
        private SiteAdminContextAccessor _siteAdminContextAccessor;

        public AdminContextAccessor(
            //IHttpContextAccessor httpContextAccessor,
            UserContextAccessor userContextAccessor,
            ClientAdminContextAccessor clientAdminContextAccessor,
            SiteAdminContextAccessor siteAdminContextAccessor
        )
        {
            //_httpContextAccessor = httpContextAccessor;
            _userContextAccessor = userContextAccessor;
            _clientAdminContextAccessor = clientAdminContextAccessor;
            _siteAdminContextAccessor = siteAdminContextAccessor;
        }

        public AdminContext GetContext()
        {
           return new AdminContext(
                _userContextAccessor,
                _clientAdminContextAccessor,
                _siteAdminContextAccessor
            );
        }

    }
}
