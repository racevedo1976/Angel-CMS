using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Angelo.Connect.CoreWidgets.UI.ViewModels.Validation;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.CoreWidgets.Models
{
    public class Image : IWidgetModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Caption is required.")]
        public string Caption { get; set; }
        public string Src { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }
        public string Radius { get; set; } = "0px";

        [ImageLink]
        public string Link { get; set; }
        public bool LinkNewWindow { get; set; }
    }
}
