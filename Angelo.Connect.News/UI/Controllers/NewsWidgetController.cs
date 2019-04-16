using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.News.Services;
using Angelo.Connect.News.Models;
using Angelo.Connect.Services;
using Angelo.Connect.News.UI.ViewModels;
using Angelo.Connect.Models;
using Kendo.Mvc.UI;

namespace Angelo.Connect.News.UI.Controllers
{
    public class NewsWidgetController : Controller
    {
        private NewsWidgetService _NewsWidgetService;
        private TagManager _tagManager;

        public NewsWidgetController
        (
            NewsWidgetService newsWidgetService,
            TagManager tagManager
        )
        {
            _NewsWidgetService = newsWidgetService;
            _tagManager = tagManager;
        }

        [Authorize]
        [HttpDelete, Route("/api/widgets/News")]
        public IActionResult DeleteNewsWidget(NewsWidget model)
        {
            if (model.Id != null)
            {
                _NewsWidgetService.DeleteModel(model.Id);
                return Ok();
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/News")]
        public IActionResult UpdateNewsWidget(NewsWidget model)
        {
            if (ModelState.IsValid)
            {
                _NewsWidgetService.UpdateModel(model);
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/NewsCategory")]
        public IActionResult UpdateNewsWidgetCategories(NewsWidgetCategorySubmissionViewModel model)
        {
            if (model.Categories != null && ModelState.IsValid)
            {
                var categoryIds = model.Categories.Split(new char[] { ',' });

                _NewsWidgetService.SetWidgetCategories(model.WidgetId, categoryIds);

                return Ok(model);
            }
            else if (model.Categories == null && model.WidgetId != null)
            {
                _NewsWidgetService.ClearWidgetCategories(model.WidgetId);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        //[Authorize]
        //[HttpPost, Route("/api/widgets/NewsTag")]
        //public IActionResult UpdateNewsWidgetTags(NewsWidgetTagSubmissionViewModel model)
        //{
        //    if (model.Tags != null && ModelState.IsValid)
        //    {
        //        var tagIds = model.Tags.Split(new char[] { ',' });

        //        _NewsWidgetService.SetWidgetTags(model.WidgetId, tagIds);
        //    }
        //    else if (model.Tags == null && model.WidgetId != null)
        //    {
        //        _NewsWidgetService.ClearWidgetTags(model.WidgetId);

        //        return Ok(model);
        //    }

        //    return Ok(model);
        //}

        [Authorize]
        [HttpPost, Route("/api/posts/NewsWidgetTagCreate")]
        public async Task<IActionResult> CreateNewsWidgetTag([DataSourceRequest] DataSourceRequest request, Tag model, string widgetId, string userId)
        {
            if (model != null && ModelState.IsValid && string.IsNullOrEmpty(model.Id))
            {
                var newTag = new Tag();

                newTag.UserId = userId;
                newTag.TagName = model.TagName;
                model.Id = newTag.Id;

                await _tagManager.AddTag(newTag);

                _NewsWidgetService.AddWidgetTag(widgetId, newTag.Id);   

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
