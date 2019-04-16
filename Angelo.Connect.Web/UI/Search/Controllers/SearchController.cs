using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.Search.ViewModels;

using LUtil = Lucene.Net.Util;
using LIndex = Lucene.Net.Index;
using LDoc = Lucene.Net.Documents;
using LStore = Lucene.Net.Store;
using LSearch = Lucene.Net.Search;
using LQuery = Lucene.Net.Queries;
using LAnalysis = Lucene.Net.Analysis;
using LParsers = Lucene.Net.QueryParsers;


namespace Angelo.Connect.Web.UI.Search.Controllers
{
    public class SearchController : Controller
    {
        private ConnectCoreOptions _coreOptions;
        private SiteContextAccessor _siteContextAccessor;
        private PageMasterManager _masterPageManager;

        public SearchController
        (
            ILogger<SearchController> logger,
            ConnectCoreOptions coreOptions,
            SiteContextAccessor siteContextAccessor,
            PageMasterManager masterPageManager
        )
        {
            _coreOptions = coreOptions;
            _siteContextAccessor = siteContextAccessor;
            _masterPageManager = masterPageManager;
        }

        [HttpGet("/sys/search")]
        public IActionResult Index([FromQuery] string q)
        {
            var siteContext = _siteContextAccessor.GetContext();
            var siteOptions = siteContext.Options;
          
            if (siteOptions.ContainsKey("GoogleSiteId"))
            {
                return GoogleSearch(q, siteOptions["GoogleSiteId"], siteOptions["GoogleSearchCode"]);
            }

            // else
            return LuceneSearch(q, siteContext.SiteId);
        }

        private IActionResult GoogleSearch(string q, string googleSiteId, string googleSearchCode)
        {

            ViewData["SearchQuery"] = q;
            ViewData["GoogleSiteId"] = googleSiteId;
            ViewData["GoogleSearchCode"] = googleSearchCode;

            return this.MasterPageView("Search Results", "~/UI/Search/Views/GoogleResults.cshtml");
        }

        private IActionResult LuceneSearch(string q, string siteId)
        {
            var viewModel = new ViewModels.LuceneSearchResult();
            var indexFolderPath = System.IO.Path.Combine(_coreOptions.SearchIndexRoot, siteId);
            var segmentsFilePath = System.IO.Path.Combine(indexFolderPath, "segments.gen");

            viewModel.SearchPhrase = q;
            viewModel.IndexExists = System.IO.File.Exists(segmentsFilePath);

            if(viewModel.IndexExists)
            {
                var query = CreateWeightedQuery(q);

                viewModel.Entries = ExecuteQuery(indexFolderPath, query);
                viewModel.ResultCount = viewModel.Entries.Count();
            }

            return this.MasterPageView("~/UI/Search/Views/LuceneResults.cshtml", viewModel, "Search Results");
        }


        private LSearch.Query CreateWeightedQuery(string searchText)
        {
            var version = LUtil.LuceneVersion.LUCENE_48;
            var stopwords = LAnalysis.Standard.StandardAnalyzer.STOP_WORDS_SET;
            var analyzer = new LAnalysis.En.EnglishAnalyzer(version, stopwords);

            var boosts = new Dictionary<string, float>
            {
                { "title", 120 },
                { "keywords", 100 },
                { "description", 80 },
                { "path", 60 },
                { "content", 40 }
            };

            var parser = new LParsers.Classic.MultiFieldQueryParser(version, boosts.Keys.ToArray(), analyzer, boosts);
            parser.AutoGeneratePhraseQueries = true;

            return parser.Parse(searchText);
        }

        private LSearch.Query CreateSimpleQuery(string searchText)
        {
            var version = LUtil.LuceneVersion.LUCENE_48;
            var stopwords = LAnalysis.Standard.StandardAnalyzer.STOP_WORDS_SET;
            var analyzer = new LAnalysis.En.EnglishAnalyzer(version, stopwords);
            var parser = new LParsers.Classic.QueryParser(version, "content", analyzer);

            parser.AutoGeneratePhraseQueries = true;
            parser.Enable_tracing();

            return parser.Parse(searchText);
        }

        public IEnumerable<LuceneSearchEntry> ExecuteQuery(string indexFolder, LSearch.Query query)
        {
            var searchIndex = LStore.FSDirectory.Open(indexFolder);
            var searchManager = new LSearch.SearcherManager(searchIndex, null);
            
            // try to release locs
            searchManager.MaybeRefreshBlocking();

            //execute search
            var searcher = searchManager.Acquire();
            var rawResults = searcher.Search(query, 20);

            //convert raw results to model
            var outResults = new List<LuceneSearchEntry>();

            for (var i = 0; i < rawResults.ScoreDocs.Length; i++)
            {
                var result = rawResults.ScoreDocs[i];
                var doc = searcher.Doc(result.Doc);

                outResults.Add(new LuceneSearchEntry
                {
                   Rank = i,
                   Id = doc.Get("id"),
                   Uri = doc.Get("uri"),
                   Title = doc.Get("title"),
                   Snippet = doc.Get("snippet"),
                   Score = result.Score
                });
            }

            return outResults;
        }

    }
}