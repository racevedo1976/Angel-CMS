using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Identity.Services;
using Angelo.Identity.Models;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using System;
using Angelo.Connect.Web.UI.ViewModels.Admin;

namespace Angelo.Connect.Web.UI.Components
{
    public class GroupDetailsEdit : ViewComponent
    {
        private GroupManager _groupManager;
        private UserGroupManager _userGroupManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public GroupDetailsEdit(UserGroupManager userGroupManager, GroupManager groupManager, IContextAccessor<UserContext> userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
            _groupManager = groupManager;
        }

        //protected IViewComponentResult InvokeCreate(string ownerLevel, string ownerId, string userGroupType)
        //{
        //    OwnerLevel oLevel;
        //    if (!Enum.TryParse<OwnerLevel>(ownerLevel, out oLevel))
        //        throw new Exception("Unknown OwnerLevel: " + ownerLevel);

        //    UserGroupType ugType;
        //    if (!Enum.TryParse<UserGroupType>(userGroupType, out ugType))
        //        throw new Exception("Unknown UserGroupType: " + userGroupType);

        //    var model = new UserGroupViewModel()
        //    {
        //        OwnerLevel = oLevel,
        //        OwnerId = ownerId,
        //        UserGroupType = ugType,
        //    };

        //    switch (ugType)
        //    {
        //        case UserGroupType.ConnectionGroup:
        //            model.AllowPublicEnrollment = false;
        //            ViewData["ShowOpenEnrollmentCheckbox"] = true;
        //            break;

        //        case UserGroupType.NotificationGroup:
        //            model.AllowPublicEnrollment = true;
        //            ViewData["ShowOpenEnrollmentCheckbox"] = true;
        //            break;

        //        default:
        //            model.AllowPublicEnrollment = false;
        //            ViewData["ShowOpenEnrollmentCheckbox"] = true;
        //            break;
        //    }

        //    ViewData["isCreate"] = true;
        //    return View("UserGroupDetailsEdit", model);
        //}

        //protected async Task<IViewComponentResult> InvokeEditAsync(string id)
        //{
        //    var userGroup = await _userGroupManager.GetUserGroupAsync(id);
        //    if (userGroup == null)
        //        return new ViewComponentPlaceholder();
        //    var model = userGroup.ProjectTo<UserGroupViewModel>();

        //    ViewData["ShowOpenEnrollmentCheckbox"] = true;  //(model.UserGroupType == UserGroupType.ConnectionGroup);

        //    ViewData["isCreate"] = false;
        //    return View("UserGroupDetailsEdit", model);
        //}

        public async Task<IViewComponentResult> InvokeAsync(string id)
        {
            Group group = new Group();

            if (string.IsNullOrEmpty(id))
            {
                group = new Group
                {
                    UserId = _userContextAccessor.GetContext().UserId
                };
            }else
            {
                group = await _groupManager.GetGroup(id);
            }
            
            return View("GroupDetailsEdit", new GroupViewModel
            {
                Id = group.Id,
                OwnerId = group.UserId,
                Name = group.Name
            });
        }


    }
}
