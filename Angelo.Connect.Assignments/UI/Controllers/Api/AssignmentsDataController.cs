using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;


using Angelo.Common.Extensions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using System;
using Angelo.Connect.Security;
using System.Collections.Generic;
using Angelo.Identity.Models;
using Angelo.Connect.Assignments.UI.ViewModels;
using Angelo.Connect.Assignments.Services;
using Angelo.Connect.Assignments.Models;
using Angelo.Connect.Assignments.Data;

namespace Angelo.Connect.Assignments.UI.Controllers.Api
{

    public class AssignemtnsDataController : Controller
    {
        private PageManager _pageManager;
        private AssignmentManager _assignmentManager;
        private UserGroupManager _userGroupManager;
        private SiteManager _siteManager;
        private ClientManager _clientManager;
        private UserContext _userContext;
        private IOptions<RequestLocalizationOptions> _localizationOptions;

        public AssignemtnsDataController(PageManager pageManager, 
            SiteContext siteContext, 
            AssignmentManager assignmentManager,
            UserGroupManager userGroupManager,
            SiteManager siteManager, 
            ILogger<ClientManager> logger,
            ClientManager clientManager, 
            UserContext userContext,
            IOptions<RequestLocalizationOptions> localizationOptions)
            {
            _pageManager = pageManager;
            _assignmentManager = assignmentManager;
            _userGroupManager = userGroupManager;
            _siteManager = siteManager;
            _clientManager = clientManager;
            _userContext = userContext;
            _localizationOptions = localizationOptions;
        }

        [HttpPost, Authorize, Route("/api/assignments/data")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId, string categoryId)
        {
            OwnerLevel oLevel;
            if (!Enum.TryParse(ownerLevel, true, out oLevel))
                throw new Exception("Unknown OwnerLevel [" + ownerLevel ?? "" + "]");

            var list = await _assignmentManager.GetCategoriesAndAssignmentsOfOwnerQueryAsync(oLevel, ownerId);
            var result = list.ToDataSourceResult(request);
            result.Data = (result.Data as ICollection<AssignmentListItemViewModel>)
                .OrderByDescending(x => x.CategoryName)
                .ThenByDescending(x => x.DueDT)
                .ToList();
            return Json(result);
        }

        [HttpPost, Authorize, Route("/api/assignments/categories")]
        public JsonResult GetAllAssignmentCategories([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId)
        {
            var oLevel = ownerLevel.ToEnumOrDefault(OwnerLevel.User);
            var query = _assignmentManager.GetCategoriesOfOwnerIdQuery(oLevel, ownerId).OrderBy(c => c.Title);
            var result = query.ToDataSourceResult(request);
            return Json(result);
        }

        private JsonResult GetUserGroups([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId, UserGroupType userGroupType)
        {
            var oLevel = ownerLevel.ToEnum<OwnerLevel>();
            var accessLevels = new AccessLevel[] { AccessLevel.Contributor, AccessLevel.FullAccess };
            var userQuery = _userGroupManager.GetUserGroupsAssignedToUserWithAccessLevelQuery(ownerId, accessLevels, userGroupType);
            var ownerQuery = _userGroupManager.GetUserGroupsOfOwnerAndTypeQuery(oLevel, ownerId, userGroupType);

            var userResults = userQuery.ToDataSourceResult(request);
            var ownerResults = ownerQuery.ToDataSourceResult(request);

            // note that the ToDataSourceResult function has issues with IQuereiables resulting from a Union function.
            var list = new List<UserGroup>();
            list.AddRange(userResults.Data.Cast<UserGroup>());
            foreach (UserGroup item in ownerResults.Data)
                if (!list.Where(x => x.Id == item.Id).Any())
                    list.Add(item);

            var result = list.ToDataSourceResult(request);
            return Json(result);
        }

        [HttpPost, Authorize, Route("/api/assignments/connectiongroups")]
        public JsonResult GetConnectionGroups([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId)
        {
            return GetUserGroups(request, ownerLevel, ownerId, UserGroupType.ConnectionGroup);
        }

        [Authorize, HttpPost, Route("/api/assignments/category")]
        public async Task<ActionResult> SaveAssignmentCategory(AssignmentCategoryViewModel model)
        {
            //ModelState.AddModelError("EmailSubject", "Test Error.");

            if (ModelState.IsValid)
            {
                var category = new AssignmentCategory()
                {
                    Id = model.Id,
                    OwnerLevel = model.OwnerLevel,
                    OwnerId = model.OwnerId,
                    Title = model.Title
                };

                if (string.IsNullOrEmpty(model.Id))
                    await _assignmentManager.InsertCategoryAsync(category);
                else
                    await _assignmentManager.UpdateCategoryAsync(category);

                return Ok(category);
            }
            return BadRequest(ModelState);
        }

        [Authorize, HttpDelete, Route("/api/assignments/category")]
        public async Task<ActionResult> DeleteAssignmentCategory(AssignmentCategoryViewModel model)
        {
            try
            {
                await _assignmentManager.DeleteCategoryAsync(model.Id);
                return Ok(model);
            }
            catch
            {
                ModelState.AddModelError("", "Unable to delete catagory.");
                return BadRequest(ModelState);
            }
        }









        [Authorize, HttpPost, Route("/api/assignments/unpublish")]
        public async Task<ActionResult> UnpublishAssignment(string Id)
        {
            try
            {
                await _assignmentManager.UnpublishAssignmentAsync(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize, HttpPost, Route("/api/assignments")]
        public async Task<ActionResult> SaveAssignment(AssignmentDetailsViewModel model)
        {
            if (model.CategoryIds.Count == 0)
            {
                ModelState.AddModelError("CategoryIds", "You must select at least one Category for this Assignment.");
                return BadRequest(ModelState);
            }
            else
            {
                if (string.IsNullOrEmpty(model.Id))
                {
                    var assignment = model.CloneToAssignment();
                    assignment.CreatedBy = _userContext.UserId;
                    await _assignmentManager.InsertAssignmentAsync(assignment, model.CategoryIds, model.ConnectionGroupIds);
                    model.Id = assignment.Id;
                    model.CreatedUTC = assignment.CreatedUTC;
                }
                else
                {
                    var assignment = model.CloneToAssignment();
                    await _assignmentManager.UpdateAssignmentAsync(assignment, model.CategoryIds, model.ConnectionGroupIds);
                }
                return Ok(model);
            }
        }



        [Authorize, HttpDelete, Route("/api/assignments")]
        public async Task<ActionResult> DeleteAssignment(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest();

            var success = await _assignmentManager.DeleteAssignmentAsync(Id);
            if (success)
                return Ok(new { id = Id });
            else
                return BadRequest();
        }



       


    }
}
