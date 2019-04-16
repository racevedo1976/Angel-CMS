using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;
using Angelo.Connect.Models;
using Angelo.Common.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientProductAppViewModel
    {
        public string ClientProductAppId { get; set; }
        public string Title { get; set; }
        public string ClientId { get; set; }
        [Required]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public ProductSubscriptionType SubscriptionType { get; set; }
        public DateTime SubscriptionStartUTC { get; set; }
        public DateTime? SubscriptionEndUTC { get; set; }
        public int MaxSiteCount { get; set; }
        public List<SelectListItem> Addons { get; set; }
        public List<String> AddonIds { get; set; }


        //public List<ProductAddOnViewModel> Addons { get; set; }

        public bool IsActive
        {
            get
            {
                var now = DateTime.UtcNow;
                return ((SubscriptionStartUTC < now) &&
                        ((SubscriptionEndUTC == null) || (SubscriptionEndUTC > now)));
            }
        }

        public ClientProductAppViewModel()
        {
            Addons = new List<SelectListItem>();
            AddonIds = new List<String>();
        }

    }
}
