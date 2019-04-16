using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Abstractions;
using Angelo.Connect.News.Data;
using Angelo.Connect.News.Services;
using Angelo.Connect.News.UI.ViewModels;
using Angelo.Connect.News.Security;
using Angelo.Connect.News.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.News.UI.Controllers
{
    public class NewsPublicController : Controller
    {

        private NewsDbContext _NewsDbContext;
        private NewsManager _newsManager;
        private NewsQueryService _newsQueryService;
        private NewsSecurityService _newsSecurity;
        private NewsWidgetService _newsWidgetService;
        private IContextAccessor<UserContext> _userContextAccessor;

        public NewsPublicController
        (
            NewsDbContext newsDbContext,
            NewsManager newsManager,
            NewsQueryService newsQueryService,
            NewsSecurityService newsSecurity,
            NewsWidgetService newsWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _NewsDbContext = newsDbContext;
            _newsManager = newsManager;
            _newsQueryService = newsQueryService;
            _newsSecurity = newsSecurity;
            _newsWidgetService = newsWidgetService;
            _userContextAccessor = userContextAccessor;
        }

        [HttpGet, Route("/sys/content/newspost/{id}")]
        public IActionResult PublicView(string id, [FromQuery] string version = null, [FromQuery] bool preview = false)
        {
            var newsPost = _newsManager.GetNewsPost(id, version);

            if (newsPost.IsPrivate && !_newsSecurity.AuthorizeForRead(newsPost))
                return Unauthorized();

            var settings = new ShellSettings(newsPost.Title);

            // show toolbar if user is authorized (unless in preview mode)
            preview = false;
            if (_newsSecurity.AuthorizeForEdit(newsPost) && !preview)
            {
                settings.Toolbar = new ToolbarSettings("~/UI/Views/Public/NewsPostToolbar.cshtml", newsPost);
            }

            var bindings = new ContentBindings
            {
                ContentType = NewsManager.CONTENT_TYPE_NEWSPOST,
                ContentId = newsPost.Id,
                VersionCode = newsPost.VersionCode,
                ViewPath = "~/UI/Views/Public/NewsPost.cshtml",
                ViewModel = newsPost,
            };
          
            return this.MasterPageView(bindings, settings);
        }


        [HttpGet, Route("/sys/content/Newspostlist/{id}")]
        public IActionResult Index(string id)
        {
            var newsWidget = _newsWidgetService.GetModel(id);

            return this.MasterPageView("~/UI/Views/Public/NewsList.cshtml", newsWidget, newsWidget.Title);
        }


        [HttpGet, Route("/sys/content/newspost/range")]
        public JsonResult GetPostRange(string id, int skip)
        {
            var newsPosts = _newsQueryService.QueryByWidget(id);
            var newsPostCount = newsPosts.Count();
            List<NewsPostViewModel> model = new List<NewsPostViewModel>();

            if (skip < newsPostCount)
            {
                var newsPostRange = newsPosts.OrderByDescending(x => x.Posted).Skip(skip).Take(10).ToList();
                
                foreach (NewsPost post in newsPostRange)
                {
                    model.Add(new NewsPostViewModel
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Image = post.Image,
                        Caption = post.Caption,
                        Excerp = post.Excerp,
                        PostedStr = post.Posted.ToString("ddd MMM dd hh:mm tt")
                    });
                }
            }

            return Json(model);
        }

    }
}
