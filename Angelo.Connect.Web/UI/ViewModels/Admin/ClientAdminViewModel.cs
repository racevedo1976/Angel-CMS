using System;
using System.Collections.Generic;

using Angelo.Common.Models;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientAdminViewModel
    {
        public ClientViewModel Client { get; set; }
        public ICollection<ProductViewModel> Products { get; set; }
        //public string SelectedProductId { get; set; }
        //public ProductViewModel SelectedProduct
        //{
        //    get
        //    {
        //        foreach (var item in Products)
        //            if (!string.IsNullOrEmpty(item.Id) && item.Id.Equals(SelectedProductId, System.StringComparison.OrdinalIgnoreCase) == true)
        //                return item;
        //        return null;
        //    }
        //}
        
        public ClientAdminViewModel()
        {
            Client = new ClientViewModel();
            Products = new List<ProductViewModel>();
            //SelectedProductId = string.Empty;
        }
    }
}
