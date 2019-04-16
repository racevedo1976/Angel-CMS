using Angelo.Connect.Web.UI.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Validation
{
    public class NavigationMenuItemContentAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            NavigationMenuItemViewModel navMenuItem = (NavigationMenuItemViewModel)validationContext.ObjectInstance;

            if (navMenuItem.ContentType == "ExternalURL" && String.IsNullOrEmpty(navMenuItem.ExternalURL))
            {
                return new ValidationResult("External URL must not be empty.");
            }

            if (navMenuItem.ContentType == "PageLink" && String.IsNullOrEmpty(navMenuItem.ContentId))
            {
                return new ValidationResult("An item must be selected.");
            }

            return ValidationResult.Success;
        }
    }
}
