using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.Widgets.Services;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class SlideShowService : JsonWidgetService<SlideShow>
    {
        public SlideShowService(ConnectDbContext db) : base(db)
        {
        }

        public override SlideShow GetDefaultModel()
        {
            return new SlideShow
            {
                Image1Caption = "Fluffy Cat",
                Image1Src = "/img/SeedImages/cats1_600_400.jpg",
                Image2Caption = "Angry Cat",
                Image2Src = "/img/SeedImages/cats2_600_400.jpg",
                Image3Caption = "Sleepy Cat",
                Image3Src = "/img/SeedImages/cats3_600_400.jpg",
                Speed = 4000,
            };
        }
    }
}
