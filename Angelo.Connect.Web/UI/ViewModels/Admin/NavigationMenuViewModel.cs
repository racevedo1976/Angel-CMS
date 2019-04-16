using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;
using Angelo.Connect.Models;
using System.ComponentModel.DataAnnotations;
using Kendo.Mvc.UI;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class NavigationMenuViewModel
    {
        public NavigationMenuViewModel()
        {
            Items = new List<TreeViewItemModel>();
        }

        [Display(Name = "Id", ShortName = "Id")]
        public string Id { get; set; }

        [Display(Name = "Title", ShortName = "Title")]
        public string Title { get; set; }

        [Display(Name = "Site Id", ShortName = "Site Id")]
        public string SiteId { get; set; }

        public List<TreeViewItemModel> Items { get; set; }
    }
}
