using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Web.UI.Components.ClientLevel
{
    public class ClientUserSites : ViewComponent
    {
        private ClientAdminContext _clientContext;
        private ClientAdminContextAccessor _clientContextAccessor;
        SiteManager _siteManager;
        public ClientUserSites(ClientAdminContextAccessor clientContextAccessor, SiteManager siteManager)
        {
            _clientContextAccessor = clientContextAccessor;
            _clientContext = clientContextAccessor.GetContext();
            _siteManager = siteManager;
        }


        public async Task<IViewComponentResult> InvokeAsync()
        {
            var App = _clientContext?.Product;
            
            ViewData["appId"] = App.AppId;
            return await Task.Run(() => View());
        }

    }


}
