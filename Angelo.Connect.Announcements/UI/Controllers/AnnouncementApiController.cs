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
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.Announcement.Services;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.UI.ViewModels;


namespace Angelo.Connect.Announcement.UI.Controllers
{
    public class AnnouncementApiController : Controller
    {
        private AnnouncementManager _announcementManager;
        private AnnouncementSecurityService _announcementSecurity;
        private AnnouncementQueryService _announcementQueries;
        private CategoryManager _categoryManager;
        private ContentManager _contentManager;
        private TagManager _tagManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementApiController
        (
            AnnouncementManager announcementManager,
            AnnouncementQueryService announcementQueries,
            AnnouncementSecurityService announcementSecurity,
            CategoryManager categoryManager,
            ContentManager contentManager,
            TagManager tagManager,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _announcementManager = announcementManager;
            _announcementQueries = announcementQueries;
            _announcementSecurity = announcementSecurity;
            _categoryManager = categoryManager;
            _contentManager = contentManager;
            _tagManager = tagManager;

            _userContextAccessor = userContextAccessor;
        }

        [Authorize]
        [HttpPost, Route("/sys/announcement/posts")]
        [HttpPost, Route("/sys/api/announcement/posts")]
        public IActionResult UpdateAnnouncementPost(AnnouncementPostUpdateModel model)
        {
            if (model != null && ModelState.IsValid)
            {
                var user = _userContextAccessor.GetContext();
                var oldPost = _announcementManager.GetAnnouncementPost(model.Id);
                
                if (_announcementSecurity.AuthorizeForEdit(oldPost))
                {
                    var newPost = model.ProjectTo<AnnouncementPost>();
                    newPost.UserId = user.UserId;

                    // update the versioned post & non-versioned settings
                    _announcementManager.UpdateAnnouncementPost(newPost);
                    _announcementManager.UpdateAnnouncementPostSettings(newPost.Id, model.IsPrivate);

                    // update category mappings
                    var categoryIds = (model.CategoryIds ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    _announcementManager.SetAnnouncementPostCategories(newPost.Id, categoryIds);

                    // update version label
                    if (!string.IsNullOrEmpty(model.NewVersionLabel))
                    {
                        _contentManager.UpdateVersionLabel(AnnouncementManager.CONTENT_TYPE_ANNOUNCEMENTPOST, model.Id, model.VersionCode, model.NewVersionLabel).Wait();
                    }
                }

                if(model.ShouldPublish && _announcementSecurity.AuthorizeForPublish(oldPost))
                {
                    _announcementManager.PublishAnnouncementPost(model.Id, model.VersionCode);
                }
                
                return Ok(model);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete, Route("/sys/announcement/posts/{id}")]
        [HttpDelete, Route("/sys/api/announcement/posts/{id}")]
        public IActionResult DeleteAnnouncementPost(string id)
        {
            var announcementPost = _announcementManager.GetAnnouncementPost(id);

            if (!_announcementSecurity.AuthorizeForDelete(announcementPost))
                return Unauthorized();

            try
            {
                _announcementManager.DeleteAnnouncementPost(announcementPost);

                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [Authorize]
        [HttpPost, Route("/sys/announcement/posts/{id}/settings")]
        [HttpPost, Route("/sys/api/announcement/posts/{id}/settings")]
        public IActionResult UpdateAnnouncementPostSettings(string id, AnnouncementPostSettingsUpdateModel model)
        {
            try {
                var announcementPost = _announcementManager.GetAnnouncementPost(id);

                if (!_announcementSecurity.AuthorizeForEdit(announcementPost))
                    return Unauthorized();

                // Update Category Mappings
                var categoryIds = (model.CategoryIds ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                _announcementManager.SetAnnouncementPostCategories(id, categoryIds);

                // Update General Settings
                announcementPost.IsPrivate = model.IsPrivate;
                _announcementManager.UpdateAnnouncementPostSettings(id, model.IsPrivate);

                return Ok();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [Authorize]
        [HttpPost, Route("/sys/announcement/posts/{id}/categories")]
        [HttpPost, Route("/sys/api/announcement/posts/{id}/categories")]
        public IActionResult SetAnnouncementPostCategories(string id, [FromForm] string categories)
        {
            if (ModelState.IsValid)
            {
                var categoryIds = (categories ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _announcementManager.SetAnnouncementPostCategories(id, categoryIds);

                return Ok();
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/sys/announcement/posts/{id}/tags")]
        [HttpPost, Route("/sys/api/announcement/posts/{id}/tags")]
        public IActionResult SetAnnouncementPostTags(string id, [FromForm] string tags)
        {
            if (ModelState.IsValid)
            {
                var tagIds = (tags ?? "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                _announcementManager.SetAnnouncementPostTags(id, tagIds);

                return Ok();
            }
           
            return BadRequest(ModelState);
        }

        [Authorize]
        [Route("/sys/api/announcement/categories/query")]
        public ActionResult AnnouncementCategoryQuery([DataSourceRequest] DataSourceRequest request, string text = null, bool shared = false)
        {
            var user = _userContextAccessor.GetContext();
            var query = shared
                ? _announcementQueries.QueryCategoriesSharedWithUser(user, text)
                : _announcementQueries.QueryCategoriesOwnedByUser(user, text);

            return Json(query.ToDataSourceResult(request));
        }

        [Authorize]
        [HttpDelete, Route("/sys/data/announcement/categories/{id}")]
        [HttpDelete, Route("/sys/api/announcement/categories/{id}")]
        public ActionResult DeleteAnnouncementCategory(string id)
        {
            var user = _userContextAccessor.GetContext();

            try
            {
                _announcementManager.DeleteAnnouncementCategory(id);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }

            return Ok();
        }

        [Authorize]
        [HttpPost, Route("/sys/data/announcement/categories")]
        [HttpPost, Route("/sys/api/announcement/categories")]
        public ActionResult SaveAnnouncementCategory(AnnouncementCategory category)
        {
            var user = _userContextAccessor.GetContext();

            category.UserId = user.UserId;
            category.IsActive = true;

            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrEmpty(category.Id))
                        _announcementManager.CreateAnnouncementCategory(category);
                    else
                        _announcementManager.UpdateAnnouncementCategory(category);
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
