using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;
using Angelo.Connect.Models;
using Angelo.Connect.Web.UI.ViewModels.Validation;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Services;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class NavigationMenuItemViewModel
    {
        public NavigationMenuItemViewModel()
        {
            ContentProviders = new List<SelectListItem>();
        }

        [Display(Name = "Id", ShortName = "Id")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Title", ShortName = "Title")]
        public string Title { get; set; }

        [Display(Name = "Nav Menu ID", ShortName = "Nav Menu ID")]
        public string NavMenuId { get; set; }

        [Display(Name = "Parent Item ID", ShortName = "Parent Item ID")]
        public string ParentId { get; set; }

        public string SiteId { get; set; }

        [NavigationMenuItemContent]
        [Display(Name = "Type", ShortName = "Type")]
        public string ContentType { get; set; }

        public string ContentTypeLabel { get; set; }

        public string ContentId { get; set; }

        public string ContentTitle { get; set; }

        [Display(Name = "Open in new window")]
        public bool TargetType { get; set; }

        [Display(Name = "Order", ShortName = "Order")]
        public int Order { get; set; }

        [Display(Name = "External URL", ShortName = "External URL")]
        public string ExternalURL { get; set; }

        public ICollection<SelectListItem> ContentProviders { get; set; }
    }
}
