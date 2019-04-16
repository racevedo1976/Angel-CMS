using System;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AutoMapper.Extensions;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Announcement.Data;
using Angelo.Connect.Announcement.Services;
using Angelo.Connect.Announcement.UI.ViewModels;
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Announcement.UI.Controllers
{
    public class AnnouncementPublicController : Controller
    {

        private AnnouncementDbContext _announcementDbContext;
        private AnnouncementManager _announcementManager;
        private AnnouncementQueryService _announcementQueryService;
        private AnnouncementSecurityService _announcementSecurity;
        private AnnouncementWidgetService _announcementWidgetService;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementPublicController
        (
            AnnouncementDbContext announcementDbContext,
            AnnouncementManager announcementManager,
            AnnouncementQueryService announcementQueryService,
            AnnouncementSecurityService announcementSecurity,
            AnnouncementWidgetService announcementWidgetService,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _announcementDbContext = announcementDbContext;
            _announcementManager = announcementManager;
            _announcementQueryService = announcementQueryService;
            _announcementSecurity = announcementSecurity;
            _announcementWidgetService = announcementWidgetService;
            _userContextAccessor = userContextAccessor;
        }

        [HttpGet, Route("/sys/content/announcementpost/{id}")]
        public IActionResult PublicView(string id, [FromQuery] string version = null, [FromQuery] bool preview = false)
        {
            var announcementPost = _announcementManager.GetAnnouncementPost(id, version);

            if (announcementPost.IsPrivate && !_announcementSecurity.AuthorizeForRead(announcementPost))
                return Unauthorized();

            var settings = new ShellSettings(announcementPost.Title);

            // show toolbar if user is authorized (unless in preview mode)
            preview = false;
            if (_announcementSecurity.AuthorizeForEdit(announcementPost) && !preview)
            {
                settings.Toolbar = new ToolbarSettings("~/UI/Views/Public/AnnouncementPostToolbar.cshtml", announcementPost);
            }

            var bindings = new ContentBindings
            {
                ContentType = AnnouncementManager.CONTENT_TYPE_ANNOUNCEMENTPOST,
                ContentId = announcementPost.Id,
                VersionCode = announcementPost.VersionCode,
                ViewPath = "~/UI/Views/Public/AnnouncementPost.cshtml",
                ViewModel = announcementPost,
            };
          
            return this.MasterPageView(bindings, settings);
        }


        [HttpGet, Route("/sys/content/announcementpostlist/{id}")]
        public IActionResult Index(string id)
        {
            var announcementWidget = _announcementWidgetService.GetModel(id);

            return this.MasterPageView("~/UI/Views/Public/AnnouncementList.cshtml", announcementWidget, announcementWidget.Title);
        }


        [HttpGet, Route("/sys/content/announcementpost/range")]
        public JsonResult GetPostRange(string id, int skip)
        {
            var announcementPosts = _announcementQueryService.QueryByWidget(id);
            var announcementPostCount = announcementPosts.Count();
            List<AnnouncementPostViewModel> model = new List<AnnouncementPostViewModel>();

            if (skip < announcementPostCount)
            {
                var announcementPostRange = announcementPosts.OrderByDescending(x => x.Posted).Skip(skip).Take(10).ToList();
                
                foreach (AnnouncementPost post in announcementPostRange)
                {
                    model.Add(new AnnouncementPostViewModel
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
