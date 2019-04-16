using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Security;
using System;
using Angelo.Common.Extensions;
using Angelo.Connect.Assignments.UI.ViewModels;
using Angelo.Connect.Assignments.Services;

namespace Angelo.Connect.Assignments.UI.Components
{
    public class AssignmentCategoryEdit : ViewComponent
    {
        private AssignmentManager _assignmentManager;

        public AssignmentCategoryEdit(AssignmentManager assignmentManager)
        {
            _assignmentManager = assignmentManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string ownerLevel, string ownerId)
        {
            if (string.IsNullOrEmpty(ownerId) && string.IsNullOrEmpty(id))
                return new ViewComponentPlaceholder();

            if (string.IsNullOrEmpty(id))
            {
                // New Category
                var oLevel = ownerLevel.ToEnumOrDefault(OwnerLevel.User);
                var model = new AssignmentCategoryViewModel()
                {
                    OwnerLevel = oLevel,
                    OwnerId = ownerId
                };
                return View("AssignmentCategoryCreate", model);
            }
            else
            {
                // Edit Category
                var category = await _assignmentManager.GetCategoryAsync(id);
                if (category == null)
                    throw new Exception($"Unable to find Assignment Category (Id:{id})");

                var model = new AssignmentCategoryViewModel()
                {
                    Id = category.Id,
                    OwnerLevel = category.OwnerLevel,
                    OwnerId = category.OwnerId,
                    Title = category.Title
                };
                return View("AssignmentCategoryEdit", model);
            }
        }


    }
}
