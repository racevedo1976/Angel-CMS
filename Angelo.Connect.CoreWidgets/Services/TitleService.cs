using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.Widgets.Services;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class TitleService : JsonWidgetService<Title>
    {
        public TitleService(ConnectDbContext db) : base(db)
        {
        }

        public override Title GetDefaultModel()
        {
            // returning same options regardless of view (for now)
            return new Title
            {
                Text = "Sample Title"
            };
        }
    }
}
