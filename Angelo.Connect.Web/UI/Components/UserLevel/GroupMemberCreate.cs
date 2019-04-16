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
using Angelo.Identity.Services;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Web.UI.Components
{
    public class GroupMemberCreate : ViewComponent
    {
        private GroupManager _groupManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public GroupMemberCreate(GroupManager groupManager, IContextAccessor<UserContext> userContextAccessor)
        {
            _groupManager = groupManager;
            _userContextAccessor = userContextAccessor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(string userGroupId, string poolId)
        {
           
            var model = new GroupMembership()
            {
                GroupId = userGroupId
             
            };

            ViewData["PoolId"] = poolId;
            ViewData["GroupId"] = userGroupId;

            return View("GroupMemberCreate", model);
        }
    }
}
