using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Angelo.Connect.UI.ViewModels
{
    public class GenericViewResult
    {
        public string Title { get; set; }

        public string ViewPath { get; set; }

        public object ViewModel { get; set; }

        public Dictionary<string, object> ViewData { get; set; }

        public GenericViewResult()
        {
            ViewData = new Dictionary<string, object>();
        }
    }
}
