using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Search.ViewModels
{
    public class LuceneSearchEntry
    {
        public int Rank { get; set; }
        public string Id { get; set; }
        public string Uri { get; set; }
        public string Title { get; set; }
        public string Snippet { get; set; }
        public float Score { get; set; }
    }
}
