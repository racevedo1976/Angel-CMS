using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Models;
using Angelo.Connect.Assignments.UI.ViewModels;
using Angelo.Connect.Assignments.Services;
using Angelo.Connect.Assignments.Data;

namespace Angelo.Connect.Assignments.UI.Components
{
    public class AssignmentDetailsView : ViewComponent
    {
        private AssignmentManager _assignmentManager;

        public AssignmentDetailsView(AssignmentManager assignmentManager)
        {
            _assignmentManager = assignmentManager;
        }

        private async Task LoadUserGroupsAsync(AssignmentDetailsViewModel model, string assignmentId)
        {
            var userGroups = await _assignmentManager.GetUserGroupsAssignedToAssignmentAsync(assignmentId);
            foreach (var ug in userGroups)
            {
                if (ug.UserGroupType == UserGroupType.ConnectionGroup)
                    model.ConnectionGroups.Add(ug);
            }
        }

        private async Task LoadCategoriesAsync(AssignmentDetailsViewModel model, string id)
        {
            model.Categories = await _assignmentManager.GetCategoriesAssignedToAssignmentAsync(id);
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string ownerLevel, string ownerId)
        {
            if (string.IsNullOrEmpty(id))
                return new ViewComponentPlaceholder();

            var note = await _assignmentManager.GetAssignmentAsync(id);
            if (note == null) 
                return new ViewComponentPlaceholder();

            var model = note.CloneToAssignmentDetailsViewModel();
            await LoadUserGroupsAsync(model, id);
            await LoadCategoriesAsync(model, id);
            
            model.TimeZoneName = TimeZoneHelper.NameOfTimeZoneId(model.TimeZoneId);

            ViewData["ownerLevel"] = ownerLevel ?? string.Empty;
            ViewData["ownerId"] = ownerId ?? string.Empty;

            return await Task.Run(() => {
                return View("AssignmentDetailsView", model);
            });
        }
    }
}
