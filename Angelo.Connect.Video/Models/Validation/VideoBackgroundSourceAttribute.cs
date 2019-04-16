using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Video.Models.Validation
{
    public class VideoBackgroundSourceAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            VideoBackgroundWidgetViewModel videoBackground = (VideoBackgroundWidgetViewModel)validationContext.ObjectInstance;

            if ((videoBackground.VideoSourceType == VideoSourceTypes.Vimeo && String.IsNullOrEmpty(videoBackground.VimeoVideoId)) ||
                (videoBackground.VideoSourceType == VideoSourceTypes.YouTube && String.IsNullOrEmpty(videoBackground.YoutubeVideoId)))
            {
                return new ValidationResult("Video ID must not be empty.");
            }
            return ValidationResult.Success;
        }
    }
}
