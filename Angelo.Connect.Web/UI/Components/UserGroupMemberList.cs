using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using AutoMapper.Extensions;
using Microsoft.AspNetCore.Html;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class UserGroupMemberList : ViewComponent
    {
        private UserGroupManager _userGroupManager;

        public UserGroupMemberList(UserGroupManager userGroupManager)
        {
            _userGroupManager = userGroupManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string Id = "")
        {
            if (string.IsNullOrEmpty(Id))
                return new ViewComponentPlaceholder();

            var group = await _userGroupManager.GetUserGroupAsync(Id);
            if (group == null)
                return new ViewComponentPlaceholder();

            ViewData["hideLevelColumn"] = false;
            ViewData["hideMessageColumn"] = (group.UserGroupType == UserGroupType.NotificationGroup);
            ViewData["userGroupId"] = Id;
            return View("UserGroupMemberList");
        }

    }
}

