using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class SharedFolderTreeViewComponent : ViewComponent
    {
        private SharedFolderManager _folderManager;
        private SiteManager _siteManager;
        private IOptions<RequestLocalizationOptions> _localizationOptions;

        public SharedFolderTreeViewComponent(SharedFolderManager folderManager, SiteManager siteManager, IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _folderManager = folderManager;
            _siteManager = siteManager;
            _localizationOptions = localizationOptions;
        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId = "", string siteId = "")
        {
            if (string.IsNullOrEmpty(siteId) == false)
            {
                ViewData["siteId"] = siteId;
                return await Task.FromResult(View("SiteSharedFolderTree"));
            }
            else if (string.IsNullOrEmpty(clientId) == false)
            {
                ViewData["clientId"] = clientId;
                return await Task.FromResult(View("ClientSharedFolderTree"));
            }
            else
                return new ViewComponentPlaceholder();
        }

    }
}
