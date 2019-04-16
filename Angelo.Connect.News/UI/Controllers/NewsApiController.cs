using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Connect.News.Security;
using Angelo.Connect.News.Services;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.UI.ViewModels;


namespace Angelo.Connect.News.UI.Controllers
{
    public class NewsApiController : Controller
    {
        private NewsManager _NewsManager;
        private NewsSecurityService _NewsSecurity;
        private NewsQueryService _NewsQueries;
        private CategoryManager _categoryManager;
        private ContentManager _contentManager;
        private TagManager _tagManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public NewsApiController
        (
            NewsManager NewsManager,
            NewsQueryService NewsQueries,
            NewsSecurityService NewsSecurity,
            CategoryManager categoryManager,
            ContentManager contentManager,
            TagManager tagManager,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _NewsManager = NewsManager;
            _NewsQueries = NewsQueries;
            _NewsSecurity = NewsSecurity;
            _categoryManager = categoryManager;
            _contentManager = contentManager;
            _tagManager = tagManager;

            _userContextAccessor = userContextAccessor;
        }

        [Authorize]
        [HttpPost, Route("/sys/News/posts")]
        [HttpPost, Route("/sys/api/News/posts")]
        public IActionResult UpdateNewsPost(NewsPostUpdateModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var user = _userContextAccessor.GetContext();
                var oldPost = _NewsManager.GetNewsPost(model.Id);
                
                if (_NewsSecurity.AuthorizeForEdit(oldPost))
                {
                    var newPost = model.ProjectTo<NewsPost>();
                    newPost.UserId = user.UserId;

                    // update the versioned post & non-versioned settings
                    _NewsManager.UpdateNewsPost(newPost);
                    _NewsManager.UpdateNewsPostSettings(newPost.Id, model.IsPrivate);

                    // update category mappings
                    var categoryIds = (model.CategoryIds ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    _NewsManager.SetNewsPostCategories(newPost.Id, categoryIds);

                    // update version label
                    if (!string.IsNullOrEmpty(model.NewVersionLabel))
                    {
                        _contentManager.UpdateVersionLabel(NewsManager.CONTENT_TYPE_NEWSPOST, model.Id, model.VersionCode, model.NewVersionLabel).Wait();
                    }
                }

                if(model.ShouldPublish && _NewsSecurity.AuthorizeForPublish(oldPost))
                {
                    _NewsManager.PublishNewsPost(model.Id, model.VersionCode);
                }
                
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete, Route("/sys/News/posts/{id}")]
        [HttpDelete, Route("/sys/api/News/posts/{id}")]
        public IActionResult DeleteNewsPost(string id)
        {
            var NewsPost = _NewsManager.GetNewsPost(id);

            if (!_NewsSecurity.AuthorizeForDelete(NewsPost))
                return Unauthorized();

            try
            {
                _NewsManager.DeleteNewsPost(NewsPost);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpPost, Route("/sys/News/posts/{id}/settings")]
        [HttpPost, Route("/sys/api/News/posts/{id}/settings")]
        public IActionResult UpdateNewsPostSettings(string id, NewsPostSettingsUpdateModel model)
        {
            try {
                var NewsPost = _NewsManager.GetNewsPost(id);

                if (!_NewsSecurity.AuthorizeForEdit(NewsPost))
                    return Unauthorized();

                // Update Category Mappings
                var categoryIds = (model.CategoryIds ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                _NewsManager.SetNewsPostCategories(id, categoryIds);

                // Update General Settings
                NewsPost.IsPrivate = model.IsPrivate;
                _NewsManager.UpdateNewsPostSettings(id, model.IsPrivate);

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost, Route("/sys/News/posts/{id}/categories")]
        [HttpPost, Route("/sys/api/News/posts/{id}/categories")]
        public IActionResult SetNewsPostCategories(string id, [FromForm] string categories)
        {
            if (ModelState.IsValid)
            {
                var categoryIds = (categories ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _NewsManager.SetNewsPostCategories(id, categoryIds);

                return Ok();
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/sys/News/posts/{id}/tags")]
        [HttpPost, Route("/sys/api/News/posts/{id}/tags")]
        public IActionResult SetNewsPostTags(string id, [FromForm] string tags)
        {
            if (ModelState.IsValid)
            {
                var tagIds = (tags ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _NewsManager.SetNewsPostTags(id, tagIds);

                return Ok();
            }
           
            return BadRequest(ModelState);
        }

        [Authorize]
        [Route("/sys/api/News/categories/query")]
        public ActionResult NewsCategoryQuery([DataSourceRequest] DataSourceRequest request, string text = null, bool shared = false)
        {
            var user = _userContextAccessor.GetContext();
            var query = shared
                ? _NewsQueries.QueryCategoriesSharedWithUser(user, text)
                : _NewsQueries.QueryCategoriesOwnedByUser(user, text);

            return Json(query.ToDataSourceResult(request));
        }

        [Authorize]
        [HttpDelete, Route("/sys/data/News/categories/{id}")]
        [HttpDelete, Route("/sys/api/News/categories/{id}")]
        public ActionResult DeleteNewsCategory(string id)
        {
            var user = _userContextAccessor.GetContext();

            try
            {
                _NewsManager.DeleteNewsCategory(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost, Route("/sys/data/News/categories")]
        [HttpPost, Route("/sys/api/News/categories")]
        public ActionResult SaveNewsCategory(NewsCategory category)
        {
            var user = _userContextAccessor.GetContext();

            category.UserId = user.UserId;
            category.IsActive = true;

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(category.Id))
                        _NewsManager.CreateNewsCategory(category);
                    else
                        _NewsManager.UpdateNewsCategory(category);
                }
                catch (Exception ex)
                {
                    return BadRequest(ex);
                }

                return Ok(category);
            }

            return BadRequest(ModelState);
        }


    }
}
