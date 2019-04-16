using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

using Angelo.Connect.Rendering;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteViewModel
    {
        public SiteViewModel()
        {
            Id = null;
            Domains = new List<CorpSiteDomainViewModel>();
            Cultures = new List<SiteCultureViewModel>();
            CultureKeys = new List<String>();
            SiteCollections = new List<SiteCollectionViewModel>();
            SiteAdmins = new List<UserViewModel>();
            UserPools = new List<Identity.Models.SecurityPool>();
            Templates = new List<SiteTemplate>();
        }

        [Display(Name = "Site Id", ShortName = "Id")]
        public string Id { get; set; }

        [Display(Name = "Site Title", ShortName = "Title")]
        [Required(ErrorMessage = "Title.Error.Required")]
        public string Title { get; set; }

        [Display(Name = "Site Tenant Key", ShortName = "Tenant Key")]
        public string TenantKey { get; set; }

        [Display(Name = "Site Security Pool Key", ShortName = "Security Pool")]
        public string SecurityPoolId { get; set; }

        [Display(Name = "Site Security Pool Name", ShortName = "Pool Name")]
        public string SecurityPoolName { get; set; }

        [Display(Name = "Site Client Id", ShortName = "Client Id")]
        public string ClientId { get; set; }

        [Display(Name = "Site Client Name", ShortName = "Client")]
        public string ClientName { get; set; }

        [Display(Name = "Application Id", ShortName = "App Id")]
        public string AppId { get; set; }

        [Display(Name = "Application Name", ShortName = "App Name")]
        public string AppName { get; set; }

        public string ClientProductAppId { get; set; }

        //[Display(Name = "ProductId.Name", ShortName = "ProductId.ShortName")]
        //public string ProductId { get; set; }

        //[Display(Name = "ProductName.Name", ShortName = "ProductName.ShortName")]
        //public string ProductName { get; set; }

        [Display(Name = "Site Banner", ShortName = "Banner")]
        public string Banner { get; set; }

        [Display(Name = "Site Template Id", ShortName = "Template Id")]
        [Required(ErrorMessage = "Template.Error.Required")]
        public string TemplateId { get; set; }

        [Display(Name = "Site Template Name", ShortName = "Template")]
        public string TemplateName { get; set; }

        [Display(Name = "Site Theme Id", ShortName = "Theme Id")]
        [Required(ErrorMessage = "Theme.Error.Required")]
        public string ThemeId { get; set; }

        [Display(Name = "Site Theme Name", ShortName = "Theme")]
        public string ThemeName { get; set; }


        [Display(Name = "Site is Published", ShortName = "Published")]
        public bool Published { get; set; }

        [Display(Name = "Status", ShortName = "Status")]
        public string Status { get; set; }


        [Display(Name = "Default Site Culture Key", ShortName = "Default Culture")]
        public string DefaultCultureKey { get; set; }

        [Display(Name = "Default Site Culture Name", ShortName = "Default Culture Name")]
        public string DefaultCultureDisplayName { get; set; }

        [Display(Name = "Cultures", ShortName = "Cultures")]
        public List<SiteCultureViewModel> Cultures { get; set; }

        [Display(Name = "Domains", ShortName = "Domains")]
        public List<CorpSiteDomainViewModel> Domains { get; set; }

        [Display(Name = "Site Collections", ShortName = "Site Collections")]
        public List<SiteCollectionViewModel> SiteCollections { get; set; }

        [Display(Name = "Site Admins", ShortName = "Site Admins")]
        public List<UserViewModel> SiteAdmins { get; set; }

        public List<SiteDirectoryViewModel> SiteDirectories { get; set; }

        public List<String> CultureKeys { get; set; }
        public List<Identity.Models.SecurityPool> UserPools { get; set; }

        public List<SiteTemplate> Templates { get; set; }

        public List<SiteCultureViewModel> SelectedCultures
        {
            get
            {
                return Cultures.Where(x => x.IsSelected == true).ToList();
            }
        }

        public SelectList AvailableCultures
        {
            get
            {
                return new SelectList(Cultures, "CultureKey", "DisplayName", DefaultCultureKey);
            }
        }

        public SelectList AvailableUserPools
        {
            get
            {
                return new SelectList(UserPools, "PoolId", "Text", SecurityPoolId);
            }
        }

        public SelectList AvailableTemplates
        {
            get
            {
                return new SelectList(Templates, "Id", "Title", TemplateId);
            }
        }
    }
}