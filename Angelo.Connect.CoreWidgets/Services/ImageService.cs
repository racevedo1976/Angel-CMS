using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;

using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.Widgets.Services;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class ImageService : JsonWidgetService<Image>
    {
        public ImageService(ConnectDbContext db) : base(db)
        {
        }

        public new void UpdateModel(Image model)
        {
            model.Height = ParseSize(model.Height);
            model.Width = ParseSize(model.Width);

            base.UpdateModel(model);
        }

        public new void SaveModel(Image model)
        {
            model.Height = ParseSize(model.Height);
            model.Width = ParseSize(model.Width);

            base.UpdateModel(model);
        }

        public override Image GetDefaultModel()
        {
            //var images = new List<string>
            //{
            //    "/img/SeedImages/technics1_400_200.jpg",
            //    "/img/SeedImages/technics4_400_200.jpg",
            //    "/img/SeedImages/technics5_400_200.jpg",
            //    "/img/SeedImages/sports1_400_200.jpg",
            //    "/img/SeedImages/sports2_400_200.jpg",
            //    "/img/SeedImages/sports3_400_200.jpg",
            //    "/img/SeedImages/people1_400_200.jpg",
            //    "/img/SeedImages/people2_400_200.jpg",
            //    "/img/SeedImages/nature2_600_400.jpg",
            //    "/img/SeedImages/nature3_600_400.jpg",
            //    "/img/SeedImages/nature5_600_400.jpg"
            //};

            //var index = new CryptoRandom().Next(images.Count);

            // returning same options regardless of view (for now)
            return new Image
            {
                Caption = "",
                Src = ""
            };
        }

        //TODO. Move css value parsers / validators to a common util
        private string ParseSize(string size)
        {
            double value;

            // Simple approach to cleaning up image size values
            if(size != null)
                size = size.Replace(" ", "").ToLower();

            if (string.IsNullOrEmpty(size))
                size = null;    // null size means original image size

            else if (size.StartsWith("a"))
                size = "auto";  // scale to container size

            //TODO: Use regex to ensure <num>, <num>px, or <num>%, where num is a valid double (12.5, etc)
            else if(double.TryParse(size, out value))
            {
                // not allowing 0 values since user should remove image
                // if hidden, then possible would be stuck as hidden forever (undesignable)
                if (value == 0d)
                    size = null;
                else
                    size += "px"; // assume px when % or px not supplied
            }

            return size;
        }
    }
}
