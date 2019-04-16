using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;

using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Connect.Menus;


namespace Angelo.Connect.Web.UI.Components
{
    public class BreadCrumb : ViewComponent
    {
        AdminContext _adminContext;

        public BreadCrumb(AdminContext adminContext)
        {
            _adminContext = adminContext;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
     
    }
}
