using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Validation
{
    public class PagePathAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            PageViewModel page = (PageViewModel)validationContext.ObjectInstance;
            var _pageManager = (PageManager)validationContext.GetService(typeof(PageManager));

            page.Path = CleanRoute(page.Path);

            var checkPage = _pageManager.GetByRouteAsync(page.SiteId, page.Path).Result;

            if (checkPage != null)
            {
                // check new or edited page
                if (page.Id == null || ((checkPage.Id != page.Id) && (checkPage.Path == page.Path)))
                {
                    return new ValidationResult("Page path must be unique.");
                }
            }

            return ValidationResult.Success;
        }

        private string CleanRoute(string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                route = "/";
            }
            else if (!route.Trim().StartsWith("/"))
            {
                route = "/" + route;
            }

            return route.Trim();
        }
    }
}
