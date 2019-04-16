using Angelo.Connect.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ViewTemplateViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ViewPath { get; set; }
        public string PreviewPath { get; set; }

        public ViewTemplateType Type { get; set; }
    }
}
