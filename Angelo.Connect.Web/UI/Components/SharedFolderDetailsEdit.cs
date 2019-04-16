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
    public class SharedFolderDetailsEdit : ViewComponent
    {
        private SharedFolderManager _folderManager;
        private SiteManager _siteManager;
        private IOptions<RequestLocalizationOptions> _localizationOptions;

        public SharedFolderDetailsEdit(SharedFolderManager folderManager, SiteManager siteManager, IOptions<RequestLocalizationOptions> localizationOptions)
        {
            _folderManager = folderManager;
            _siteManager = siteManager;
            _localizationOptions = localizationOptions;
        }

        public async Task<IViewComponentResult> InvokeAsync(string folderId = "")
        {
            if (string.IsNullOrEmpty(folderId) == false)
            {
                var folder = await _folderManager.GetFolderAsync(folderId);
                var model = folder.ProjectTo<SharedFolderViewModel>();
                return View("SharedFolderDetailsEdit", model);
            }
            else
                return new ViewComponentPlaceholder();
        }

    }
}
