using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientAdminProductsViewModel
    {
        public string ClientId { get; set; }
        public ICollection<ProductViewModel> Products { get; set; }

        public ClientAdminProductsViewModel()
        {
            Products = new List<ProductViewModel>();
        }
    }
}

