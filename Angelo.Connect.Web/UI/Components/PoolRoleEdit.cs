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
using Angelo.Common.Mvc.ActionResults;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.Components
{
    public class PoolRoleEditViewComponent : ViewComponent
    {
        private Identity.SecurityPoolManager _poolManager;
        private RoleManager _roleManager;

        public PoolRoleEditViewComponent(Identity.SecurityPoolManager poolManager, RoleManager roleManager)
        {
            _poolManager = poolManager;
            _roleManager = roleManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string poolId = "", string roleId = "")
        {
            if (string.IsNullOrEmpty(poolId) || string.IsNullOrEmpty(roleId))
                return new ViewComponentPlaceholder();

            var role = await _roleManager.GetByIdAsync(roleId);

            var model = new PoolRoleViewModel()
            {
                Id = role.Id,
                Name = role.Name,
                IsDefault = role.IsDefault,
                IsLocked = role.IsLocked,
                PoolId = poolId
            };

            return View("PoolRoleEdit", model);
        }
    }
}
