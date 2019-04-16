using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.Widgets.Services;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class IconService : JsonWidgetService<Icon>
    {
        public IconService(ConnectDbContext db) : base(db)
        {
        }

        public override Icon GetDefaultModel()
        {
            // returning same options regardless of view (for now)
            return new Icon
            {
                Name = "bell",
                Tooltip = "Bell",
                Text = "Sample Text",
                Size = "18px"
            };
        }

        public IEnumerable<string> GetSupportedSizes()
        {
            return new string[] { "12px", "14px", "16px", "18px", "20px", "24px", "32px", "48px", "64px", "96px", "128px", "256px" };
        }

        public IEnumerable<IconInfo> GetSupportedIcons()
        {
            return new IconInfo[]
            {
                /* general */
                new IconInfo { Name ="bell",  Title = "Bell", Css = "fa fa-bell" },
                new IconInfo { Name ="briefcase",  Title = "Briefcase", Css = "fa fa-briefcase" },
                new IconInfo { Name ="cloud", Title = "Cloud", Css = "fa fa-cloud" },
                new IconInfo { Name ="coffee", Title = "Coffee", Css = "fa fa-coffee" },
                new IconInfo { Name ="gear", Title = "Gear",  Css = "fa fa-gear" },
                new IconInfo { Name ="globe", Title = "Globe", Css = "fa fa-globe" },
                new IconInfo { Name ="graduation-cap", Title = "Graduation",  Css = "fa fa-graduation-cap" },
                new IconInfo { Name ="heart",  Title = "Heart", Css = "fa fa-heart" },
                
                /* alerts */
                new IconInfo { Name ="info", Title = "Info", Css = "fa fa-info"},
                new IconInfo { Name ="info-circle", Title = "Info Alt", Css = "fa fa-info-circle" },
                new IconInfo { Name ="question", Title = "Question",  Css = "fa fa-question" },
                new IconInfo { Name ="question-circle", Title = "Question Alt",  Css = "fa fa-question-circle" },
                new IconInfo { Name ="warning", Title = "Warning", Css = "fa fa-warning" },
                
                /* social media */
                new IconInfo { Name = "facebook", Title = "Facebook", Css = "fa fa-facebook" },
                new IconInfo { Name = "google-plus", Title = "Google Plus", Css = "fa fa-google-plus" },
                new IconInfo { Name = "instagram", Title = "Instagram", Css = "fa fa-instagram" },
                new IconInfo { Name = "snapchat", Title = "Snapchat", Css = "fa fa-snapchat" },
                new IconInfo { Name = "twitter", Title = "Twitter", Css = "fa fa-twitter" },
                new IconInfo { Name = "yelp", Title = "Yelp", Css = "fa fa-yelp" },
                new IconInfo { Name = "youtube", Title = "Youtube", Css = "fa fa-youtube" },
            };
        }

        public IconInfo GetIconInfo(string name)
        {
            var icons = GetSupportedIcons();

            return icons.FirstOrDefault(x => x.Name == name);
        }
    }

    public class IconInfo
    {
        public string Name { get; set; }
        public string Css { get; set; }
        public string Title { get; set; }
    }
}
