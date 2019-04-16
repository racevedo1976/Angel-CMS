using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Search.ViewModels
{
    public class LuceneSearchResult
    {
        public bool IndexExists { get; set; }
        public string SearchPhrase { get; set; }
        public int ResultCount { get; set; }

        public IEnumerable<LuceneSearchEntry> Entries { get; set; }
    }
}
