using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Common.Mvc.ActionResults;
using AutoMapper.Extensions;
using Angelo.Connect.Models;
using Angelo.Identity.Models;
using Angelo.Connect.Security;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;

namespace Angelo.Connect.Web.UI.Components
{
    public class UserGroupMemberCreate : ViewComponent
    {
        private UserGroupManager _userGroupManager;
        private EnumLocalizer _enumLocalizer;

        public UserGroupMemberCreate(UserGroupManager userGroupManager, EnumLocalizer enumLocalizer)
        {
            _userGroupManager = userGroupManager;
            _enumLocalizer = enumLocalizer;
        }

        protected List<SelectListItem> GetAccessLevelSelectList(AccessLevel accessLevel)
        {
            var list = new List<SelectListItem>();
            foreach (AccessLevel level in Enum.GetValues(typeof(AccessLevel)))
            {
                list.Add(new SelectListItem()
                {
                    Text = level.ToString(),
                    Value = level.ToString(),
                    Selected = (level == accessLevel)
                });
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(string userGroupId, string poolId)
        {
            if (string.IsNullOrEmpty(userGroupId))
                return new ViewComponentPlaceholder();

            var group = await _userGroupManager.GetUserGroupAsync(userGroupId);
            if (group == null)
                return new ViewComponentPlaceholder();

            var model = new UserGroupMembershipViewModel()
            {
                UserGroupId = userGroupId,
                AccessLevel = AccessLevel.Audience,
                AllowEmailMessaging = false,
                AllowSmsMessaging = false
            };

            if (group.UserGroupType == UserGroupType.NotificationGroup)
            {
                model.AllowEmailMessaging = true;
                model.AllowSmsMessaging = true;
            }

            ViewData["ShowAllowMessagingCheckbox"] = true;  //(group.UserGroupType != UserGroupType.NotificationGroup);
            ViewData["AccessLevelSelectList"] = _enumLocalizer.GetSelectList<AccessLevel>();
            ViewData["PoolId"] = poolId;

            return View("UserGroupMemberCreate", model);
        }
    }
}
