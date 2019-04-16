using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientAdminAddonViewModel
    {
        public string ClientId { get; set; }
        public string ProductId { get; set; }
        public ProductViewModel Addon { get; set; }
        public string WarningMessage { get; set; }

        public ClientAdminAddonViewModel()
        {
            Addon = new ProductViewModel();
        }
    }
}




