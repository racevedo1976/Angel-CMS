using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Widgets;


namespace Angelo.Connect.CoreWidgets.Models
{
    public class NavBar : IWidgetModel
    {
        public string Id { get; set; }
        public string ItemWidth { get; set; }
        public string NavMenuId { get; set; }

        public NavigationMenu NavMenu { get; set; }
    }
}
