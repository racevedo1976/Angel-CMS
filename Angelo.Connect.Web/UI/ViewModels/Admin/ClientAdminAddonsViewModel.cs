using System;
using System.Collections.Generic;

using Angelo.Common.Models;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientAdminAddonsViewModel
    {
        public string ClientId { get; set; }
        public string ProductId { get; set; }
        public ICollection<ProductViewModel> Addons { get; set; }

        public ClientAdminAddonsViewModel()
        {
            Addons = new List<ProductViewModel>();
        }
    }
}
