using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteAlertList: ViewComponent
    {
        public SiteAlertList()
        {
            
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            
            List<string> alerts = new List<string>()
            {
                "Message1",
                "Message2"
            };

            return View(alerts);
        }
    }
}
