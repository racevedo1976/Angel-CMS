using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;

namespace Angelo.Connect.Web.UI.Components
{ 
    public class ProductDetailsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(ProductViewModel product)
        {

            return View(product);
        }

    }
}
