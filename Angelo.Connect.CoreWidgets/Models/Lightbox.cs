using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Angelo.Connect.CoreWidgets.UI.ViewModels.Validation;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.CoreWidgets.Models
{
    public class Lightbox : IWidgetModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Caption is required.")]
        public string TriggerType { get; set; }
        public string Text { get; set; }
        public string Caption { get; set; }
        public string ImageSrc { get; set; }
        public string ImageWidth { get; set; }
        public string ImageHeight { get; set; }
        public string ImageRadius { get; set; } = "0px";
        public string IconType { get; set; }
        public int Timer { get; set; }
    }
}
