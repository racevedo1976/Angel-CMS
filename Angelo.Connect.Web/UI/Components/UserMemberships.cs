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
    public class UserMemberships : ViewComponent
    {
        private UserGroupManager _userGroupManager;
        private UserManager _userManager;

        public UserMemberships(UserGroupManager userGroupManger, UserManager userManager)
        {
            _userGroupManager = userGroupManger;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string userId, string siteId, string userGroupType)
        {
           var user = await _userManager.GetUserAsync(userId);
            if (user == null)
                throw new Exception("Unable to find user: " + userId);

            ViewData["userId"] = userId;
            ViewData["siteId"] = siteId;
            ViewData["emailConfirmed"] = user.EmailConfirmed;
            ViewData["smsConfirmed"] = user.PhoneNumberConfirmed;

            var groupType = userGroupType.ToEnumOrDefault(Models.UserGroupType.NotificationGroup);
            var groups = await _userGroupManager.GetPublicUserGroupsForUserAndSiteAsync(userId, siteId, groupType);
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

            return View("UserMemberships", model);
        }
    }
}
