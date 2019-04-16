using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Security;
using Microsoft.EntityFrameworkCore;
using Angelo.Common.Extensions;
using Angelo.Connect.Assignments.Services;
using Angelo.Connect.Assignments.Models;

namespace Angelo.Connect.Assignments.UI.Components
{
    public class AssignmentCategoryList : ViewComponent
    {
        private AssignmentManager _assignmentManager;

        public AssignmentCategoryList(AssignmentManager assignmentManager)
        {
            _assignmentManager = assignmentManager;
        }

        private async Task<List<AssignmentCategory>> GetCategoryList(string ownerLevel, string ownerId)
        {
            var oLevel = ownerLevel.ToEnumOrDefault(OwnerLevel.User);
            var query = _assignmentManager.GetCategoriesOfOwnerIdQuery(oLevel, ownerId).OrderBy(x => x.Title);
            var list = await query.ToListAsync();
            return list;
        }

        public async Task<IViewComponentResult> InvokeAsync(string ownerLevel, string ownerId)
        {
            var model = await GetCategoryList(ownerLevel, ownerId);
            return View("AssignmentCategoryList", model);
        }
    }
}
