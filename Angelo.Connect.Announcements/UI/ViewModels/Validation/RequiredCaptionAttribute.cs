using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Announcement.UI.ViewModels.Validation
{
    public class RequiredCaptionAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            bool hasImage = false;
            string imageCaption = null;

            if(validationContext.ObjectInstance is AnnouncementPostViewModel)
            {
                var post = (AnnouncementPostViewModel)validationContext.ObjectInstance;
                if (!String.IsNullOrEmpty(post.Image))
                    hasImage = true;
                imageCaption = post.Caption;
            }
            else if(validationContext.ObjectInstance is AnnouncementPostUpdateModel)
            {
                var post = (AnnouncementPostUpdateModel)validationContext.ObjectInstance;
                if (!String.IsNullOrEmpty(post.Image))
                    hasImage = true;
                imageCaption = post.Caption;
            }

            if (hasImage && String.IsNullOrEmpty(imageCaption))
            {
                return new ValidationResult(GetErrorMessage());
            }

            return ValidationResult.Success;
        }

        private string GetErrorMessage()
        {
            return $"Image Alternate Text must not be empty.";
        }
    }
}
