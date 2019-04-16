using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Widgets.Models
{
    public class StaticWidget : IWidgetModel
    {
        public string Id { get; set; }

        // static widgets have no instance settings
    }
}
