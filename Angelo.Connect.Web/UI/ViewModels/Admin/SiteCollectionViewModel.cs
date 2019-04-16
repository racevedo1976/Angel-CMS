using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteCollectionViewModel
    {
        public SiteCollectionViewModel()
        {
            Sites = new List<SiteViewModel>();
            IsSelected = true;
        }

        [Display(Name = "Id", ShortName ="Id")]
        public string Id { get; set; }

        [Display(Name = "Client Id", ShortName = "Client Id")]
        public string ClientId { get; set; }

        [Display(Name = "Name", ShortName = "Name")]
        public string Name { get; set; }

        public bool IsSelected { get; set; }

        public ICollection<SiteViewModel> Sites { get; set; }
    }
}
