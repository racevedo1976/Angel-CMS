using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Common.Mvc
{
    public class SelectListViewModel
    {
        public SelectListViewModel()
        {
            Items = new List<SelectListItem>();
        }

        public List<SelectListItem> Items { get; set; }

        public string AsJsonString
        {
            get { return GetJsonString(); }
            set { SetJsonString(value); }
        }

        private string GetJsonString()
        {
            return JsonConvert.SerializeObject(Items);
        }

        private void SetJsonString(string json)
        {
            Items = JsonConvert.DeserializeObject<List<SelectListItem>>(json);
        }
    }
}
