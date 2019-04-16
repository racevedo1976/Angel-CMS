using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.Widgets.Services;
using AutoMapper.Extensions;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class AlertService : JsonWidgetService<Alert>
    {
        public AlertService(ConnectDbContext db) : base(db)
        {
        }
        
        public override Alert GetDefaultModel()
        {
            // returning same options regardless of view (for now)
            return new Alert
            {
                Style = "alert-info",
                Text = "Notice: This is a sample alert."
            };
        }
    }
}
