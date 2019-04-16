using Angelo.Common.Extensions;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components
{
    public class NotifyMeList : ViewComponent
    {
        private UserGroupManager _userGroupManager;
        private UserManager _userManager;

        public NotifyMeList(UserGroupManager userGroupManger, UserManager userManager)
        {
            _userGroupManager = userGroupManger;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeForSiteAsync(string siteId)
        {
            var groups = await _userGroupManager.GetPublicUserGroupsForSiteAsync(siteId, Models.UserGroupType.NotificationGroup);

            var model = new List<UserMembershipViewModel>();
            foreach (var group in groups)
            {
                model.Add(new UserMembershipViewModel()
                {
                    UserGroupId = group.Id,
                    UserGroupName = group.Name,
                    AllowPublicEnrollment = true,
                    AllowEmailMessaging = false,
                    AllowSmsMessaging = false,
                    IsMember = false
                });
            }
            model = model.OrderBy(m => m.UserGroupName).ToList();

            ViewData["userId"] = string.Empty;
            ViewData["siteId"] = siteId;

            return View("NotifyMeViewList", model);
        }

        public async Task<IViewComponentResult> InvokeForUserAsync(string siteId, Identity.Models.User user)
        {
            var groups = await _userGroupManager.GetPublicUserGroupsForUserAndSiteAsync(user.Id, siteId, Models.UserGroupType.NotificationGroup);
            var model = new List<UserMembershipViewModel>();
            foreach (var group in groups)
            {
                model.Add(new UserMembershipViewModel()
                {
                    UserGroupId = group.UserGroupId,
                    UserGroupName = group.UserGroup.Name,
                    AllowPublicEnrollment = group.UserGroup.AllowPublicEnrollment,
                    AllowEmailMessaging = group.AllowEmailMessaging,
                    AllowSmsMessaging = group.AllowSmsMessaging,
                    IsMember = (group.UserId != null)
                });
            }
            model = model.OrderBy(m => m.UserGroupName).ToList();

            ViewData["userId"] = user.Id;
            ViewData["siteId"] = siteId;

            return View("NotifyMeEditList", model);
        }

        public async Task<IViewComponentResult> InvokeAsync(string siteId, string userId)
        {
            var user = await _userManager.GetUserAsync(userId);
            if (user == null)
                return await InvokeForSiteAsync(siteId);
            else
                return await InvokeForUserAsync(siteId, user);
        }
    }
}
