using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteCollectionMapViewModel
    {
        public string SiteId { get; set; }
        public string ClientId { get; set; }
        public List<Item> Items { get { return _items; } }

        private List<Item> _items;

        public class Item
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public bool Selected { get; set; }
            public bool OriginalStatus { get; set; }
        }

        public SiteCollectionMapViewModel()
        {
            _items = new List<Item>();
        }
    }
}
