using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace Angelo.Connect.Widgets.Models
{
    public class JsonWidget : IWidgetModel
    {
        public string Id { get; set; }

        public string ModelType { get; set; }

        public string ModelJson { get; set; }
      
        public JsonWidget()
        {

        }

        public JsonWidget(IWidgetModel model)
        {
            if (model.Id == null)
            {
                model.Id = Guid.NewGuid().ToString("N");
            }

            Id = model.Id;
            ModelType = model.GetType().FullName;
            ModelJson = JsonConvert.SerializeObject(model);
        }
    }
}
