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
using Angelo.Connect.Blog.Security;
using Angelo.Connect.Blog.Services;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.UI.ViewModels;


namespace Angelo.Connect.Blog.UI.Controllers
{
    public class BlogApiController : Controller
    {
        private BlogManager _blogManager;
        private BlogSecurityService _blogSecurity;
        private BlogQueryService _blogQueries;
        private CategoryManager _categoryManager;
        private ContentManager _contentManager;
        private TagManager _tagManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogApiController
        (
            BlogManager blogManager,
            BlogQueryService blogQueries,
            BlogSecurityService blogSecurity,
            CategoryManager categoryManager,
            ContentManager contentManager,
            TagManager tagManager,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _blogManager = blogManager;
            _blogQueries = blogQueries;
            _blogSecurity = blogSecurity;
            _categoryManager = categoryManager;
            _contentManager = contentManager;
            _tagManager = tagManager;

            _userContextAccessor = userContextAccessor;
        }

        [Authorize]
        [HttpPost, Route("/sys/blog/posts")]
        [HttpPost, Route("/sys/api/blog/posts")]
        public IActionResult UpdateBlogPost(BlogPostUpdateModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var user = _userContextAccessor.GetContext();
                var oldPost = _blogManager.GetBlogPost(model.Id);
                
                if (_blogSecurity.AuthorizeForEdit(oldPost))
                {
                    var newPost = model.ProjectTo<BlogPost>();
                    newPost.UserId = user.UserId;

                    // update the versioned post & non-versioned settings
                    _blogManager.UpdateBlogPost(newPost);
                    _blogManager.UpdateBlogPostSettings(newPost.Id, model.IsPrivate);

                    // update category mappings
                    var categoryIds = (model.CategoryIds ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    _blogManager.SetBlogPostCategories(newPost.Id, categoryIds);

                    // update version label
                    if (!string.IsNullOrEmpty(model.NewVersionLabel))
                    {
                        _contentManager.UpdateVersionLabel(BlogManager.CONTENT_TYPE_BLOGPOST, model.Id, model.VersionCode, model.NewVersionLabel).Wait();
                    }
                }

                if(model.ShouldPublish && _blogSecurity.AuthorizeForPublish(oldPost))
                {
                    _blogManager.PublishBlogPost(model.Id, model.VersionCode);
                }
                
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete, Route("/sys/blog/posts/{id}")]
        [HttpDelete, Route("/sys/api/blog/posts/{id}")]
        public IActionResult DeleteBlogPost(string id)
        {
            var blogPost = _blogManager.GetBlogPost(id);

            if (!_blogSecurity.AuthorizeForDelete(blogPost))
                return Unauthorized();

            try
            {
                _blogManager.DeleteBlogPost(blogPost);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpPost, Route("/sys/blog/posts/{id}/settings")]
        [HttpPost, Route("/sys/api/blog/posts/{id}/settings")]
        public IActionResult UpdateBlogPostSettings(string id, BlogPostSettingsUpdateModel model)
        {
            try {
                var blogPost = _blogManager.GetBlogPost(id);

                if (!_blogSecurity.AuthorizeForEdit(blogPost))
                    return Unauthorized();

                // Update Category Mappings
                var categoryIds = (model.CategoryIds ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                _blogManager.SetBlogPostCategories(id, categoryIds);

                // Update General Settings
                blogPost.IsPrivate = model.IsPrivate;
                _blogManager.UpdateBlogPostSettings(id, model.IsPrivate);

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost, Route("/sys/blog/posts/{id}/categories")]
        [HttpPost, Route("/sys/api/blog/posts/{id}/categories")]
        public IActionResult SetBlogPostCategories(string id, [FromForm] string categories)
        {
            if (ModelState.IsValid)
            {
                var categoryIds = (categories ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _blogManager.SetBlogPostCategories(id, categoryIds);

                return Ok();
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/sys/blog/posts/{id}/tags")]
        [HttpPost, Route("/sys/api/blog/posts/{id}/tags")]
        public IActionResult SetBlogPostTags(string id, [FromForm] string tags)
        {
            if (ModelState.IsValid)
            {
                var tagIds = (tags ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _blogManager.SetBlogPostTags(id, tagIds);

                return Ok();
            }
           
            return BadRequest(ModelState);
        }

        [Authorize]
        [Route("/sys/api/blog/categories/query")]
        public ActionResult BlogCategoryQuery([DataSourceRequest] DataSourceRequest request, string text = null, bool shared = false)
        {
            var user = _userContextAccessor.GetContext();
            var query = shared
                ? _blogQueries.QueryCategoriesSharedWithUser(user, text)
                : _blogQueries.QueryCategoriesOwnedByUser(user, text);

            return Json(query.ToDataSourceResult(request));
        }

        [Authorize]
        [HttpDelete, Route("/sys/data/blog/categories/{id}")]
        [HttpDelete, Route("/sys/api/blog/categories/{id}")]
        public ActionResult DeleteBlogCategory(string id)
        {
            var user = _userContextAccessor.GetContext();

            try
            {
                _blogManager.DeleteBlogCategory(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost, Route("/sys/data/blog/categories")]
        [HttpPost, Route("/sys/api/blog/categories")]
        public ActionResult SaveBlogCategory(BlogCategory category)
        {
            var user = _userContextAccessor.GetContext();

            category.UserId = user.UserId;
            category.IsActive = true;

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(category.Id))
                        _blogManager.CreateBlogCategory(category);
                    else
                        _blogManager.UpdateBlogCategory(category);
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
