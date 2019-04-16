﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Widgets;

namespace Angelo.Connect.CoreWidgets.Models
{
    public class Alert : IWidgetModel
    {
        public string Id { get; set; }
        public string Style { get; set; }
        public string Text { get; set; }
    }
}
