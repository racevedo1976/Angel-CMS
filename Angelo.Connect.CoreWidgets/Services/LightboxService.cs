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
    public class LightboxService : JsonWidgetService<Lightbox>
    {
        public LightboxService(ConnectDbContext db) : base(db)
        {
        }

        public new void UpdateModel(Lightbox model)
        {
            if (model.TriggerType == "Image")
            {
                model.ImageHeight = ParseSize(model.ImageHeight);
                model.ImageWidth = ParseSize(model.ImageWidth);
            }
            else
            {
                model.ImageHeight = null;
                model.ImageWidth = null;
            }

            base.UpdateModel(model);
        }

        public new void SaveModel(Lightbox model)
        {
            if(model.TriggerType == "Image")
            {
                model.ImageHeight = ParseSize(model.ImageHeight);
                model.ImageWidth = ParseSize(model.ImageWidth);
            }
            else
            {
                model.ImageHeight = null;
                model.ImageWidth = null;
            }
          

            base.UpdateModel(model);
        }

        public override Lightbox GetDefaultModel()
        {
            return new Lightbox
            {
                TriggerType = "Image"
            };
        }

        private string ParseSize(string size)
        {
            double value;

            // Simple approach to cleaning up image size values
            if (size != null)
                size = size.Replace(" ", "").ToLower();

            if (string.IsNullOrEmpty(size))
                size = null;    // null size means original image size

            else if (size.StartsWith("a"))
                size = "auto";  // scale to container size

            //TODO: Use regex to ensure <num>, <num>px, or <num>%, where num is a valid double (12.5, etc)
            else if (double.TryParse(size, out value))
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
