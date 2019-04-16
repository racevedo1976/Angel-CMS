using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientAdminProductViewModel
    {
        public string ClientId { get; set; }
        public string ParentProductId { get; set; }
        public ProductViewModel Product { get; set; }
        public string ActiveProductName { get; set; }
        public string WarningMessage { get; set; }

        public ClientAdminProductViewModel()
        {
            Product = new ProductViewModel();
        }
    }
}



