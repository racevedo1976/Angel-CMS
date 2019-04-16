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
using Angelo.Identity.Services;

namespace Angelo.Connect.Web.UI.Components
{
    public class GroupMemberList : ViewComponent
    {
        private GroupManager _groupManager;

        public GroupMemberList(GroupManager groupManager)
        {
            _groupManager = groupManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string Id = "")
        {
            if (string.IsNullOrEmpty(Id))
                return new ViewComponentPlaceholder();

            var groupMembers = await _groupManager.GetGroupMembers(Id);
            if (groupMembers == null)
                return new ViewComponentPlaceholder();

            var membersList = groupMembers.Select(x => new GroupMembershipViewModel
            {
                Id = x.Id,
                UserId = x.UserId,
                GroupId = x.GroupId,
                UserName = x.User.UserName,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName
            }).ToList();
            ViewData["GroupId"] = Id;
            return View("GroupMemberList", membersList);
        }

    }
}

