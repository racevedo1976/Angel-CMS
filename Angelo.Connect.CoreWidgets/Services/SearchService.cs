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
    public class SearchService : JsonWidgetService<Search>
    {
        public SearchService(ConnectDbContext db) : base(db)
        {
        }
        
        public override Search GetDefaultModel()
        {
            // returning same options regardless of view (for now)
            return new Search
            {
                Title = ""
            };
        }
    }
}
