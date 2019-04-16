using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.ViewModels
{
    public class ProductsViewModel : ValidationAttribute
    {
        [Range(1, int.MaxValue, ErrorMessage ="Please select a Product.")]
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public List<SelectListItem> ProductList { get; set; }

        [Range(1, int.MaxValue, ErrorMessage ="Please select a Client.")]
        public int ClientId { get; set; }
        public string ClientName { get; set; }
        public List<SelectListItem> ClientList { get; set; }

        public List<string> sites { get; set; }
    }
}
