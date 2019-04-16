using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Common.Mvc.ActionResults;
using AutoMapper.Extensions;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;

namespace Angelo.Connect.Web.UI.Components
{
    public class UserGroupDetailsEdit : ViewComponent
    {
        private UserGroupManager _userGroupManager;

        public UserGroupDetailsEdit(UserGroupManager userGroupManager)
        {
            _userGroupManager = userGroupManager;
        }

        protected IViewComponentResult InvokeCreate(string ownerLevel, string ownerId, string userGroupType)
        {
            OwnerLevel oLevel;
            if (!Enum.TryParse<OwnerLevel>(ownerLevel, out oLevel))
                throw new Exception("Unknown OwnerLevel: " + ownerLevel);

            UserGroupType ugType;
            if (!Enum.TryParse<UserGroupType>(userGroupType, out ugType))
                throw new Exception("Unknown UserGroupType: " + userGroupType);

            var model = new UserGroupViewModel()
            {
                OwnerLevel = oLevel,
                OwnerId = ownerId,
                UserGroupType = ugType,
            };

            switch (ugType)
            {
                case UserGroupType.ConnectionGroup:
                    model.AllowPublicEnrollment = false;
                    ViewData["ShowOpenEnrollmentCheckbox"] = true;
                    break;

                case UserGroupType.NotificationGroup:
                    model.AllowPublicEnrollment = true;
                    ViewData["ShowOpenEnrollmentCheckbox"] = true;
                    break;

                default:
                    model.AllowPublicEnrollment = false;
                    ViewData["ShowOpenEnrollmentCheckbox"] = true;
                    break;
            }

            ViewData["isCreate"] = true;
            return View("UserGroupDetailsEdit", model);
        }

        protected async Task<IViewComponentResult> InvokeEditAsync(string id)
        {
            var userGroup = await _userGroupManager.GetUserGroupAsync(id);
            if (userGroup == null)
                return new ViewComponentPlaceholder();
            var model = userGroup.ProjectTo<UserGroupViewModel>();

            ViewData["ShowOpenEnrollmentCheckbox"] = true;  //(model.UserGroupType == UserGroupType.ConnectionGroup);

            ViewData["isCreate"] = false;
            return View("UserGroupDetailsEdit", model);
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string ownerLevel, string ownerId, string userGroupType)
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(ownerLevel))
                return new ViewComponentPlaceholder();

            if (string.IsNullOrEmpty(id))
                return InvokeCreate(ownerLevel, ownerId, userGroupType);
            else
                return await InvokeEditAsync(id);
        }


    }
}
