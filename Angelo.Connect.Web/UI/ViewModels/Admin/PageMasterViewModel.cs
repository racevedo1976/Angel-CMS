using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class PageMasterViewModel
    {
        public PageMasterViewModel()
        {
            ViewTemplates = new List<SelectListItem>();
        }

        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Site")]
        public string SiteId { get; set; }

        [Display(Name = "Template")]
        public string TemplateId { get; set; }

        [Display(Name = "Template Title")]
        public string ViewTemplateTitle { get; set; }

        public string PreviewPath { get; set; }

        public List<SelectListItem> ViewTemplates { set; get; }

        public SelectList AvailableLayouts {
            get
            {
                return new SelectList(ViewTemplates, "Value", "Text", TemplateId);
            }
        }
    }
}
