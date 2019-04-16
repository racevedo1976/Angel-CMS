using Angelo.Connect.CoreWidgets.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.CoreWidgets.UI.ViewModels.Validation
{
    public class ImageLinkAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            Image thisImage = (Image)validationContext.ObjectInstance;

            if (!String.IsNullOrEmpty(thisImage.Link))
            {
                if((thisImage.Link.Substring(0, 7) != "http://") && (thisImage.Link.Substring(0, 8) != "https://"))
                    return new ValidationResult("Image link must be a full URL.");
            }

            return ValidationResult.Success;
        }
    }
}
