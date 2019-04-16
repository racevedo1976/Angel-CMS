using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Configuration;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using AutoMapper.Extensions;
using Microsoft.Extensions.Logging;
using System;
using Angelo.Connect.Security;
using System.Collections.Generic;
using Angelo.Identity;
using Angelo.Common.Extensions;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Web.UI.Controllers.Api
{

    public class UserGroupDataController : BaseController
    {
        private UserGroupManager _userGroupManager;
        private UserManager _userManager;
        private EnumLocalizer _enumLocalizer;
        private IEnumerable<ISecurityGroupProvider> _groupProviders;

        public UserGroupDataController(SiteContext siteContext,
            UserGroupManager userGroupManager, 
            UserManager userManager,
            EnumLocalizer enumLocalizer,
            ILogger<UserGroupDataController> logger,
            IEnumerable<ISecurityGroupProvider> groupProviders
            ) : base(logger)
            {
            _userGroupManager = userGroupManager;
            _userManager = userManager;
            _enumLocalizer = enumLocalizer;
            _groupProviders = groupProviders;
        }

        [Authorize]
        [HttpPost, Route("/api/usergroups/byowner")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId)
        {
            var oLevel = ownerLevel.ToEnum<OwnerLevel>();
            var query = _userGroupManager.GetUserGroupsOfOwnerQuery(oLevel, ownerId);
            var result = query.ToDataSourceResult(request);
            return Json(result);
        }

        [Authorize]
        [HttpPost, Route("/api/usergroups/byownerandtype")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId, string userGroupType)
        {
            var oLevel = ownerLevel.ToEnum<OwnerLevel>();
            var ugType = userGroupType.ToEnum<UserGroupType>();
            var query = _userGroupManager.GetUserGroupsOfOwnerAndTypeQuery(oLevel, ownerId, ugType);
            var result = query.ToDataSourceResult(request);
            return Json(result);
        }

        [Authorize]
        [HttpPost, Route("/api/usergroups/memberships/data")]
        public async Task<JsonResult> GetUserGroupMembers([DataSourceRequest] DataSourceRequest request, string id)
        {
            var query = _userGroupManager.GetUserGroupMembershipQuery(id);
            var model = query.ToDataSourceResult(request);
            var list = new List<UserGroupMembershipViewModel>();
            foreach(UserGroupMembership item in model.Data)
            {
                list.Add(new UserGroupMembershipViewModel()
                {
                    UserGroupId = item.UserGroupId,
                    UserId = item.UserId,
                    AccessLevel = item.AccessLevel,
                    AccessLevelName = _enumLocalizer.GetLocalName(item.AccessLevel),
                    AllowEmailMessaging = item.AllowEmailMessaging,
                    AllowSmsMessaging = item.AllowSmsMessaging
                });
            }
            await PopulateUserNameInViewModelAsync(list);
            model.Data = list;
            var result = Json(model);
            return result;
        }

        [Authorize]
        [HttpPost, Route("/api/usergroups/usergroups")]
        public async Task<ActionResult> Data([DataSourceRequest]DataSourceRequest request, string userId = null)
        {
            List<UserGroupMultiSelectViewModel> groupList = new List<UserGroupMultiSelectViewModel>();
            foreach (var provider in _groupProviders)
            {
                groupList.AddRange(UserGroupMappings.ToMultiSelectViewModel((IEnumerable<UserGroup>)await provider.GetGroups(), provider.GetType().Name));
            }
          
            return Json(groupList.ToDataSourceResult(request));
        }

        [Authorize]
        [HttpDelete, Route("/api/usergroups")]
        public async Task<ActionResult> DeleteUserGroup(UserGroupViewModel model)
        {
            if (string.IsNullOrEmpty(model.Id))
                return BadRequest();

            await _userGroupManager.DeleteUserGroupAsync(model.Id);
            return Ok(model);
        }

        [Authorize]
        [HttpPost, Route("/api/usergroups")]
        public async Task<ActionResult> SaveUserGroup(UserGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Id))
                {
                    model.Id = Guid.NewGuid().ToString("N");
                    var insertGroup = model.ProjectTo<Angelo.Connect.Models.UserGroup>();
                    await _userGroupManager.InsertUserGroupAsync(insertGroup);
                }
                else
                {
                    var updateGroup = model.ProjectTo<Angelo.Connect.Models.UserGroup>();
                    await _userGroupManager.UpdateUserGroupAsync(updateGroup);
                }
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete, Route("/api/usergroups/memberships")]
        public async Task<ActionResult> DeleteUserGroupMembership(UserGroupMembershipViewModel model)
        {
            if (string.IsNullOrEmpty(model.UserGroupId) || string.IsNullOrEmpty(model.UserId))
                return BadRequest();

            await _userGroupManager.DeleteUserGroupMembershipAsync(model.UserGroupId, model.UserId);
            return Ok(model);
        }

        [Authorize]
        [HttpPost, Route("/api/usergroups/memberships")]
        public async Task<ActionResult> SaveUserGroupMembership(UserGroupMembershipViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(model.UserId);
                if (user == null)
                {
                    ModelState.AddModelError("UserId", "Invalid UserId:" + model.UserId);
                    return BadRequest(ModelState);
                }
                var member = model.ProjectTo<Angelo.Connect.Models.UserGroupMembership>();
                await _userGroupManager.SaveUserGroupMembershipAsync(member);
                model.UserName = user.UserName;
                model.AccessLevelName = _enumLocalizer.GetLocalName(model.AccessLevel);
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/usergroups/setsubscription")]
        public async Task<IActionResult> SetSubscription(string userGroupId, string userId, string noteType, string noteValue)
        {
            var noteEnabled = noteValue.Equals("true", StringComparison.OrdinalIgnoreCase);
            if (noteType == NotificationType.Email)
                await _userGroupManager.UpdateNotificationGroupSubscriptionAsync(userGroupId, userId, allowEmail: noteEnabled);
            else if (noteType == NotificationType.SMS)
                await _userGroupManager.UpdateNotificationGroupSubscriptionAsync(userGroupId, userId, allowSms: noteEnabled);
            else
                return BadRequest();
            return Ok();
        }

        [Authorize]
        [HttpPost, Route("/api/usergroups/updatememberships")]
        public async Task<IActionResult> UpdateUserMemberships(IList<UserMembershipViewModel> model, string userId)
        {
            try
            {
                // TO DO: validate that the logged-in user is this user (ie: userId) or is a site admin.

                var addMemberships = new List<UserGroupMembership>();
                var removeMemberships = new List<UserGroupMembership>();
                foreach (UserMembershipViewModel item in model)
                {
                    var membership = new UserGroupMembership()
                    {
                        UserId = userId,
                        UserGroupId = item.UserGroupId,
                        AllowEmailMessaging = item.AllowEmailMessaging,
                        AllowSmsMessaging = item.AllowSmsMessaging,
                        AccessLevel = Identity.Models.AccessLevel.Audience
                    };

                    if (item.IsMember || item.AllowEmailMessaging || item.AllowSmsMessaging)
                        addMemberships.Add(membership);
                    else 
                        removeMemberships.Add(membership);
                }

                await _userGroupManager.UpdateGroupMemberships(userId, addMemberships, removeMemberships);

                return Ok(model);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                return BadRequest();
            }
        }

        protected async Task PopulateUserNameInViewModelAsync(List<UserGroupMembershipViewModel> list)
        {
            var userIds = list.Select(x => x.UserId).ToList();
            var users = await _userManager.GetUsersAsync(userIds);
            foreach (var membership in list)
            {
                var userName = users.Where(x => x.Id == membership.UserId).Select(x => x.FirstName + " " + x.LastName).FirstOrDefault();
                membership.UserName = userName ?? string.Empty;
            }
        }

    }
}
