using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Models
{
    public class CategoryFormViewModel
    {
        public CategoryFormViewModel()
        {
            CheckboxValues = new List<CheckboxValueTest>();
        }
        public string WidgetId { get; set; }
        public string SiteId { get; set; }

        public IEnumerable<CheckboxValueTest> CheckboxValues { get; set; }

        public IEnumerable<Category> Categories { get; set; }
    }
}
