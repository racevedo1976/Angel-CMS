using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.DataAnnotations;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.CoreWidgets.Models
{
    public class RawHtml : IWidgetModel
    {
        public string Id { get; set; }
        public string Html { get; set; }
        public bool StaticToolbar { get; set; }
    }
}
