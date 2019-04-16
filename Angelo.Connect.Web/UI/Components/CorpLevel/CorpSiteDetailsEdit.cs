using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using AutoMapper.Extensions;
using Microsoft.AspNetCore.Html;
using Angelo.Common.Mvc.ActionResults;
using Angelo.Identity.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.Components
{
    public class CorpSiteDetailsEdit : ViewComponent
    {
        private SiteManager _siteManager;
        private IOptions<RequestLocalizationOptions> _localizationOptions;
        private ClientManager _clientManager;
        private DirectoryManager _directoryManager;

        public CorpSiteDetailsEdit(SiteManager siteManager, 
            IOptions<RequestLocalizationOptions> localizationOptions, 
            ClientManager clientManager,
            DirectoryManager directoryManager)
        {
            _siteManager = siteManager;
            _localizationOptions = localizationOptions;
            _clientManager = clientManager;
            _directoryManager = directoryManager;
        }

        protected List<SiteCultureViewModel> GetSiteCultureViewModels(IEnumerable<string> cultureKeys)
        {
            var siteCultures = new List<SiteCultureViewModel>();
            var cultures = _localizationOptions.Value.SupportedCultures;
            foreach (var culture in cultures)
            {
                siteCultures.Add(new SiteCultureViewModel()
                {
                    CultureKey = culture.Name,
                    DisplayName = culture.DisplayName,
                    IsSelected = cultureKeys.Contains(culture.Name)
                });
            }
            return siteCultures;
        }

        protected async Task<SiteViewModel> GetSiteAsync(string siteId)
        {
            var site = await _siteManager.GetByIdAsync(siteId);
            if (site == null)
                return null;

            var model = site.ProjectTo<SiteViewModel>();
            var directories = await _directoryManager.GetMappedDirectoriesAsync(site.SecurityPoolId);

            //model.Themes = await _clientManager.GetThemesAsync(model.ClientId);
            model.Cultures = GetSiteCultureViewModels(model.CultureKeys);
            model.DefaultCultureDisplayName = model.Cultures.Where(x => x.CultureKey == model.DefaultCultureKey).FirstOrDefault()?.DisplayName;
            model.SiteDirectories = directories.ProjectTo<SiteDirectoryViewModel>().ToList();

            return model;
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId = "")
        {
            var model = await GetSiteAsync(siteId);
            if (model == null)
                return new ViewComponentPlaceholder();
            else
            {
                ViewBag.AvailableSecurityPools = new List<SelectListItem>();
                return View(model);
            }
        }


    }
}

