using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Blog.Data;
using Angelo.Connect.Blog.Services;
using Angelo.Connect.Blog.UI.ViewModels;
using Angelo.Connect.Blog.Security;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Blog.UI.Controllers
{
    public class BlogPublicController : Controller
    {

        private BlogDbContext _blogDbContext;
        private BlogManager _blogManager;
        private BlogQueryService _blogQueryService;
        private BlogSecurityService _blogSecurity;
        private BlogWidgetService _blogWidgetService;
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogPublicController
        (
            BlogDbContext blogDbContext,
            BlogManager blogManager,
            BlogQueryService blogQueryService,
            BlogSecurityService blogSecurity,
            BlogWidgetService blogWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _blogDbContext = blogDbContext;
            _blogManager = blogManager;
            _blogQueryService = blogQueryService;
            _blogSecurity = blogSecurity;
            _blogWidgetService = blogWidgetService;
            _userContextAccessor = userContextAccessor;
        }

        [HttpGet, Route("/sys/content/blogpost/{id}")]
        public IActionResult PublicView(string id, [FromQuery] string version = null, [FromQuery] bool preview = false)
        {
            var blogPost = _blogManager.GetBlogPost(id, version);

            if (blogPost.IsPrivate && !_blogSecurity.AuthorizeForRead(blogPost))
                return Unauthorized();

            var settings = new ShellSettings(blogPost.Title);

            // show toolbar if user is authorized (unless in preview mode)
            preview = false;
            if (_blogSecurity.AuthorizeForEdit(blogPost) && !preview)
            {
                settings.Toolbar = new ToolbarSettings("~/UI/Views/Public/BlogPostToolbar.cshtml", blogPost);
            }

            var bindings = new ContentBindings
            {
                ContentType = BlogManager.CONTENT_TYPE_BLOGPOST,
                ContentId = blogPost.Id,
                VersionCode = blogPost.VersionCode,
                ViewPath = "~/UI/Views/Public/BlogPost.cshtml",
                ViewModel = blogPost,
            };
          
            return this.MasterPageView(bindings, settings);
        }


        [HttpGet, Route("/sys/content/blogpostlist/{id}")]
        public IActionResult Index(string id)
        {
            var blogWidget = _blogWidgetService.GetModel(id);

            return this.MasterPageView("~/UI/Views/Public/BlogList.cshtml", blogWidget, blogWidget.Title);
        }


        [HttpGet, Route("/sys/content/blogpost/range")]
        public JsonResult GetPostRange(string id, int skip)
        {
            var blogPosts = _blogQueryService.QueryByWidget(id);
            var blogPostCount = blogPosts.Count();
            List<BlogPostViewModel> model = new List<BlogPostViewModel>();

            if (skip < blogPostCount)
            {
                var blogPostRange = blogPosts.OrderByDescending(x => x.Posted).Skip(skip).Take(10).ToList();
                
                foreach (BlogPost post in blogPostRange)
                {
                    model.Add(new BlogPostViewModel
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
