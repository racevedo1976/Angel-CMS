using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Angelo.Connect.Models;
using Angelo.Connect.News.Models;

namespace Angelo.Connect.News.UI.ViewModels
{
    public class NewsWidgetCategorySubmissionViewModel
    {
        public string WidgetId { get; set; }
        public string Categories { get; set; }
    }
}
