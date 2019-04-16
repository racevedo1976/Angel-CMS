using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.Widgets.Services;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class HeroService : JsonWidgetService<HeroUnit>
    {
        public HeroService(ConnectDbContext dbContext) : base(dbContext)
        {
            
        }

        public override HeroUnit GetDefaultModel()
        {
            return new HeroUnit
            {
                Title = "Hello, world!",
                Body = "This is a simple hero unit, a simple jumbotron-style component for calling extra attention to featured content or information."
            };
        }
    }
}
