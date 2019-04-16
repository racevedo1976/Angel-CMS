using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientProductAppListItemViewModel
    {
        public string ItemId { get; set; }
        public string Title { get; set; }
        public string ProductId { get; set; }
        public string AddOnId { get; set; }
        public string ClientProductAppId { get; set; }
        public string ClientId { get; set; }
        public string SiteCount { get; set; }
        public string TotalSpace { get; set; }
    }
}
