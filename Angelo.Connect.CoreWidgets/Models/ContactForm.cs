using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.CoreWidgets.Models
{
    public class ContactForm : IWidgetModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Recipients { get; set; }

        public bool AllowAnonymous { get; set; }
    }
}
