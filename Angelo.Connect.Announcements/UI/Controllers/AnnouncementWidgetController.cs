using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Announcement.Services;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Announcement.UI.ViewModels;
using Angelo.Connect.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Angelo.Connect.Announcement.UI.Controllers
{
    public class AnnouncementWidgetController : Controller
    {
        private AnnouncementWidgetService _announcementWidgetService;
        private TagManager _tagManager;

        public AnnouncementWidgetController
        (
            AnnouncementWidgetService announcementWidgetService,
            TagManager tagManager
        )
        {
            _announcementWidgetService = announcementWidgetService;
            _tagManager = tagManager;
        }

        [Authorize]
        [HttpDelete, Route("/api/widgets/announcement")]
        public IActionResult DeleteAnnouncementWidget(AnnouncementWidget model)
        {
            if (model.Id != null)
            {
                _announcementWidgetService.DeleteModel(model.Id);
                return Ok();
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/announcement")]
        public IActionResult UpdateAnnouncementWidget(AnnouncementWidget model)
        {
            if (ModelState.IsValid)
            {
                _announcementWidgetService.UpdateModel(model);
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/announcementCategory")]
        public IActionResult UpdateAnnouncementWidgetCategories(AnnouncementWidgetCategorySubmissionViewModel model)
        {
            if (model.Categories != null && ModelState.IsValid)
            {
                var categoryIds = model.Categories.Split(new char[] { ',' });

                _announcementWidgetService.SetWidgetCategories(model.WidgetId, categoryIds);

                return Ok(model);
            }
            else if (model.Categories == null && model.WidgetId != null)
            {
                _announcementWidgetService.ClearWidgetCategories(model.WidgetId);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/announcementTag")]
        public IActionResult UpdateAnnouncementWidgetTags(AnnouncementWidgetTagSubmissionViewModel model)
        {
            if (model.Tags != null && ModelState.IsValid)
            {
                var tagIds = model.Tags.Split(new char[] { ',' });

                _announcementWidgetService.SetWidgetTags(model.WidgetId, tagIds);
            }
            else if (model.Tags == null && model.WidgetId != null)
            {
                _announcementWidgetService.ClearWidgetTags(model.WidgetId);

                return Ok(model);
            }

            return Ok(model);
        }

        [Authorize]
        [HttpPost, Route("/api/posts/announcementWidgetTagCreate")]
        public async Task<IActionResult> CreateAnnouncementWidgetTag([DataSourceRequest] DataSourceRequest request, Tag model, string widgetId, string userId)
        {
            if (model != null && ModelState.IsValid && string.IsNullOrEmpty(model.Id))
            {
                var newTag = new Tag();

                newTag.UserId = userId;
                newTag.TagName = model.TagName;
                model.Id = newTag.Id;

                await _tagManager.AddTag(newTag);

                _announcementWidgetService.AddWidgetTag(widgetId, newTag.Id);   

                return Ok(model);
            }
            else if (model != null && ModelState.IsValid && !string.IsNullOrEmpty(model.Id))
            {
                return Ok(model);
            }

            return BadRequest(ModelState);
        }
    }
}
