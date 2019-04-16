using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Blog.Services;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Blog.UI.ViewModels;
using Angelo.Connect.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Angelo.Connect.Blog.UI.Controllers
{
    public class BlogWidgetController : Controller
    {
        private BlogWidgetService _blogWidgetService;
        private TagManager _tagManager;

        public BlogWidgetController
        (
            BlogWidgetService blogWidgetService,
            TagManager tagManager
        )
        {
            _blogWidgetService = blogWidgetService;
            _tagManager = tagManager;
        }

        [Authorize]
        [HttpDelete, Route("/api/widgets/blog")]
        public IActionResult DeleteBlogWidget(BlogWidget model)
        {
            if (model.Id != null)
            {
                _blogWidgetService.DeleteModel(model.Id);
                return Ok();
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/blog")]
        public IActionResult UpdateBlogWidget(BlogWidget model)
        {
            if (ModelState.IsValid)
            {
                _blogWidgetService.UpdateModel(model);
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/blogCategory")]
        public IActionResult UpdateBlogWidgetCategories(BlogWidgetCategorySubmissionViewModel model)
        {
            if (model.Categories != null && ModelState.IsValid)
            {
                var categoryIds = model.Categories.Split(new char[] { ',' });

                _blogWidgetService.SetWidgetCategories(model.WidgetId, categoryIds);

                return Ok(model);
            }
            else if (model.Categories == null && model.WidgetId != null)
            {
                _blogWidgetService.ClearWidgetCategories(model.WidgetId);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/blogTag")]
        public IActionResult UpdateBlogWidgetTags(BlogWidgetTagSubmissionViewModel model)
        {
            if (model.Tags != null && ModelState.IsValid)
            {
                var tagIds = model.Tags.Split(new char[] { ',' });

                _blogWidgetService.SetWidgetTags(model.WidgetId, tagIds);
            }
            else if (model.Tags == null && model.WidgetId != null)
            {
                _blogWidgetService.ClearWidgetTags(model.WidgetId);

                return Ok(model);
            }

            return Ok(model);
        }

        [Authorize]
        [HttpPost, Route("/api/posts/blogWidgetTagCreate")]
        public async Task<IActionResult> CreateBlogWidgetTag([DataSourceRequest] DataSourceRequest request, Tag model, string widgetId, string userId)
        {
            if (model != null && ModelState.IsValid && string.IsNullOrEmpty(model.Id))
            {
                var newTag = new Tag();

                newTag.UserId = userId;
                newTag.TagName = model.TagName;
                model.Id = newTag.Id;

                await _tagManager.AddTag(newTag);

                _blogWidgetService.AddWidgetTag(widgetId, newTag.Id);   

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
