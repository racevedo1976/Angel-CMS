using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

using Angelo.Connect.Models;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteCreateViewModel
    {
        public SiteCreateViewModel()
        {
            Id = null;
            Templates = new List<SiteTemplate>();
        }

        [Display(Name = "Site Id", ShortName = "Id")]
        public string Id { get; set; }

        [Display(Name = "Site Title", ShortName = "Title")]
        [Required(ErrorMessage = "Title.Error.Required")]
        public string Title { get; set; }

        [Display(Name = "Site Tenant Key", ShortName = "Tenant Key")]
        [Required(ErrorMessage = "Title.Error.Required")]
        public string TenantKey { get; set; }

        [Display(Name = "Site Client Id", ShortName = "Client Id")]
        public string ClientId { get; set; }

        [Display(Name = "Site Client Name", ShortName = "Client Name")]
        public string ClientName { get; set; }

        [Display(Name = "Site User Pool Key", ShortName = "User Pool")]
        public string UserPoolKey { get; set; }

        [Display(Name = "User Pool Name", ShortName = "User Pool Name")]
        public string UserPoolName { get; set; }

        public string ClientProductAppId { get; set; }

        [Display(Name = "Site Template Id", ShortName = "Template Id")]
        [Required(ErrorMessage = "Template.Error.Required")]
        public string TemplateId { get; set; }

        [Display(Name = "Site Template Name", ShortName = "Template")]
        public string TemplateName { get; set; }

        public List<SiteTemplate> Templates { get; set; }

        public SelectList AvailableTemplates
        {
            get
            {
                return new SelectList(Templates, "Id", "Title", TemplateId);
            }
        }

    }
}