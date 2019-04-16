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
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.Components
{
    public class GroupMemberEdit : ViewComponent
    {
        private UserGroupManager _userGroupManager;
        private UserManager _userManager;
        private EnumLocalizer _enumLocalizer;

        public GroupMemberEdit(UserGroupManager userGroupManager, UserManager userManager, EnumLocalizer enumLocalizer)
        {
            _userGroupManager = userGroupManager;
            _userManager = userManager;
            _enumLocalizer = enumLocalizer;
        }

        protected List<SelectListItem> GetAccessLevelSelectList(AccessLevel accessLevel)
        {
            var list = new List<SelectListItem>();
            foreach(AccessLevel level in Enum.GetValues(typeof(AccessLevel)))
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
        public async Task<IViewComponentResult> InvokeAsync(string userGroupId, string userId)
        {
            if (string.IsNullOrEmpty(userGroupId) || string.IsNullOrEmpty(userId))
                return new ViewComponentPlaceholder();

            var model = new UserGroupMembershipViewModel()
            {
                UserGroupId = userGroupId,
                UserId = userId
            };

            var group = await _userGroupManager.GetUserGroupAsync(model.UserGroupId);
            if (group == null)
                return new ViewComponentPlaceholder();

            var member = await _userGroupManager.GetUserGroupMembershipAsync(model.UserGroupId, model.UserId);
            if (member == null)
                return new ViewComponentPlaceholder();

            var user = await _userManager.GetUserAsync(model.UserId);
            if (user == null)
                return new ViewComponentPlaceholder();

            model.AccessLevel = member.AccessLevel;
            model.AllowEmailMessaging = member.AllowEmailMessaging;
            model.AllowSmsMessaging = member.AllowSmsMessaging;
            model.UserName = user.UserName;
            model.Email = user.Email;
            model.EmailConfirmed = user.EmailConfirmed;
            model.PhoneNumber = user.PhoneNumber;
            model.PhoneNumberConfirmed = user.PhoneNumberConfirmed;

            ViewData["ShowAllowMessagingCheckbox"] = true; 
            ViewData["AccessLevelSelectList"] = _enumLocalizer.GetSelectList<AccessLevel>();
          
            return View("UserGroupMemberEdit", model);
        }
    }
}
