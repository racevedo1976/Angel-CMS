using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Common.Mvc.ActionResults;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System;
using Angelo.Connect.Assignments.UI.ViewModels;
using Angelo.Connect.Assignments.Services;
using Angelo.Connect.Assignments.Models;
using Angelo.Connect.Assignments.Data;

namespace Angelo.Connect.Assignments.UI.Components
{
    public class AssignmentDetailsEdit : ViewComponent
    {
        private AssignmentManager _assignmentManager;

        public AssignmentDetailsEdit(AssignmentManager assignmentManager)
        {
            _assignmentManager = assignmentManager;
        }

        private async Task LoadUserGroupsAsync(AssignmentDetailsViewModel model, string assignmentId)
        {
            var userGroups = await _assignmentManager.GetUserGroupsAssignedToAssignmentAsync(assignmentId);
            foreach (var ug in userGroups)
            {
                if (ug.UserGroupType == UserGroupType.ConnectionGroup)
                {
                    model.ConnectionGroups.Add(ug);
                    model.ConnectionGroupIds.Add(ug.Id);
                }
            }
        }

        private async Task LoadCategoriesAsync(AssignmentDetailsViewModel model, string assignmentId)
        {
            var categories = await _assignmentManager.GetCategoriesAssignedToAssignmentAsync(assignmentId);
            foreach (var cat in categories)
            {
                model.Categories.Add(cat);
                model.CategoryIds.Add(cat.Id);
            }
        }

        public async Task<IViewComponentResult> InvokeAsync(string id, string ownerLevel, string ownerId, string categoryId)
        {
            if (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(ownerLevel))
                return new ViewComponentPlaceholder();

            ViewData["ownerLevel"] = ownerLevel ?? string.Empty;
            ViewData["ownerId"] = ownerId ?? string.Empty;
            ViewData["TimeZoneList"] = TimeZoneHelper.GetTimeZoneSelectList();

            AssignmentDetailsViewModel model;
            if (string.IsNullOrEmpty(id))
            {
                // Create new assignment
                model = new AssignmentDetailsViewModel()
                {
                    Id = string.Empty,
                    OwnerLevel = (OwnerLevel)Enum.Parse(typeof(OwnerLevel), ownerLevel),
                    OwnerId = ownerId,
                    Status = AssignmentStatus.Draft,
                    DueDT = DateTime.Now.AddDays(3),
                    TimeZoneId = TimeZoneHelper.DefaultTimeZoneId,
                    TimeZoneName = TimeZoneHelper.NameOfTimeZoneId(TimeZoneHelper.DefaultTimeZoneId),
                    AllowComments = false,
                    SendNotification = false
                };
                if (!string.IsNullOrEmpty(categoryId))
                {
                    var category = await _assignmentManager.GetCategoryAsync(categoryId);
                    if (category != null)
                    {
                        model.Categories.Add(category);
                        model.CategoryIds.Add(categoryId);
                    }
                }
                return View("AssignmentDetailsCreate", model);
            }
            else
            {
                // Load existing assignment
                var note = await _assignmentManager.GetAssignmentAsync(id);
                if (note == null)
                    return new ViewComponentPlaceholder();
                model = note.CloneToAssignmentDetailsViewModel();
                if (string.IsNullOrEmpty(model.TimeZoneId))
                {
                    model.TimeZoneId = TimeZoneHelper.DefaultTimeZoneId; // TO DO: Get the default time zone from the user context.
                    model.DueDT = TimeZoneHelper.Now(model.TimeZoneId);
                }
                model.TimeZoneName = TimeZoneHelper.NameOfTimeZoneId(model.TimeZoneId);
                await LoadCategoriesAsync(model, id);
                await LoadUserGroupsAsync(model, id);
                if (note.Status == AssignmentStatus.Draft)
                    return View("AssignmentDetailsEdit", model);
                else
                    return View("AssignmentDetailsView", model);
            }
        }
    }
}
