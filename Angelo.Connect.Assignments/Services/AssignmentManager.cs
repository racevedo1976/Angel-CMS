using Angelo.Connect.Assignments.Data;
using Angelo.Connect.Assignments.Models;
using Angelo.Connect.Assignments.UI.ViewModels;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Services;
using Angelo.Identity;
using Angelo.Identity.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Assignments.Services
{
    public class AssignmentManager
    {
        private AssignmentsDbContext _assignmentsDb;
        private ConnectDbContext _connectDb;
        private IdentityDbContext _identityDb;

        public AssignmentManager(AssignmentsDbContext assignmentsDb, ConnectDbContext connectDb, IdentityDbContext identityDb)
        {
            _assignmentsDb = assignmentsDb;
            _connectDb = connectDb;
            _identityDb = identityDb;
        }

        public IQueryable<Assignment> GetAssignmentsOfOwnerQuery(OwnerLevel ownerLevel, string ownerId)
        {
            return _assignmentsDb.Assignments.Where(x => (x.OwnerLevel == ownerLevel) && (x.OwnerId == ownerId));
        }

        public IQueryable<Assignment> GetAssignmentsOfOwnerAndCategoryQuery(OwnerLevel ownerLevel, string ownerId, string categoryId)
        {
            return _assignmentsDb.Assignments.AsNoTracking()
                .Where(x => (x.OwnerLevel == ownerLevel) && (x.OwnerId == ownerId))
                .Join(_assignmentsDb.AssignmentCategoryLinks.Where(l => l.CategoryId == categoryId),
                    a => a.Id,
                    l => l.AssignmentId,
                    (a, l) => a
                    );
        }

        public async Task<Assignment> GetAssignmentAsync(string id)
        {
            var note = await _assignmentsDb.Assignments.Where(x => x.Id == id).FirstOrDefaultAsync();
            return note;
        }

        public async Task<AssignmentCategory> GetCategoryAsync(string categoryId)
        {
            return await _assignmentsDb.AssignmentCategories
                .Where(c => c.Id == categoryId)
                .FirstOrDefaultAsync();
        }

        public async Task<ICollection<AssignmentListItemViewModel>> GetCategoriesAndAssignmentsOfOwnerQueryAsync(OwnerLevel ownerLevel, string ownerId)
        {
            var categoryLinks = await _assignmentsDb.AssignmentCategories.AsNoTracking()
                .Where(l => (l.OwnerLevel == ownerLevel) && (l.OwnerId == ownerId))
                .Join(_assignmentsDb.AssignmentCategoryLinks,
                    c => c.Id,
                    l => l.CategoryId,
                    (c, l) => new
                    {
                        l.AssignmentId,
                        l.CategoryId,
                        CategoryName = c.Title
                    })
                .ToListAsync();

            var assignments = await _assignmentsDb.Assignments.AsNoTracking()
                .Where(x => (x.OwnerLevel == ownerLevel) && (x.OwnerId == ownerId))
                .ToListAsync();

            var items = assignments
                .Join(categoryLinks,
                    a => a.Id,
                    c => c.AssignmentId,
                    (a, c) => new
                    {
                        c.AssignmentId,
                        a.CreatedUTC,
                        a.DueUTC,
                        a.TimeZoneId,
                        a.Status,
                        a.Title,
                        c.CategoryId,
                        c.CategoryName
                    })
                    .ToList();

            var list = new List<AssignmentListItemViewModel>();
            foreach (var item in items)
            {
                list.Add(new UI.ViewModels.AssignmentListItemViewModel()
                {
                    Id = item.AssignmentId,
                    TimeZoneId = item.TimeZoneId,
                    CreatedDT = TimeZoneHelper.ConvertFromUTC(item.CreatedUTC, item.TimeZoneId),
                    DueDT = TimeZoneHelper.ConvertFromUTC(item.DueUTC, item.TimeZoneId),
                    Title = item.Title,
                    Status = item.Status,
                    CategoryId = item.CategoryId,
                    CategoryName = item.CategoryName
                });
            }
            return list;
        }

        public async Task InsertCategoryAsync(AssignmentCategory category)
        {
            if (string.IsNullOrEmpty(category.Id))
                category.Id = Guid.NewGuid().ToString("N");
            _assignmentsDb.AssignmentCategories.Add(category);
            await _assignmentsDb.SaveChangesAsync();
        }

        public async Task UpdateCategoryAsync(AssignmentCategory category)
        {
            var oldCategory = await _assignmentsDb.AssignmentCategories.Where(x => x.Id == category.Id).FirstOrDefaultAsync();
            if (oldCategory == null)
                throw new Exception($"Unable to find Assignment Category (Id: {category.Id}).");
            oldCategory.OwnerLevel = category.OwnerLevel;
            oldCategory.OwnerId = category.OwnerId ?? oldCategory.OwnerId;
            oldCategory.Title = category.Title;
            await _assignmentsDb.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(string categoryId)
        {
            var oldCategory = await _assignmentsDb.AssignmentCategories.Where(x => x.Id == categoryId).FirstOrDefaultAsync();
            if (oldCategory != null)
            {
                var count = await _assignmentsDb.AssignmentCategoryLinks.Where(l => l.CategoryId == categoryId).CountAsync();
                if (count > 0)
                    throw new Exception($"Unable to delete Assignment Categories that are currently assigned to Assignments (CategoryId:{categoryId}).");

                _assignmentsDb.AssignmentCategories.Remove(oldCategory);
                await _assignmentsDb.SaveChangesAsync();
            }
        }

        public async Task<List<AssignmentCategory>> GetCategoriesAssignedToAssignmentAsync(string assignmentId)
        {
            var query = _assignmentsDb.AssignmentCategories.AsNoTracking()
                .Join(_assignmentsDb.AssignmentCategoryLinks.Where(x => x.AssignmentId == assignmentId),
                    c => c.Id,
                    l => l.CategoryId,
                    (c, l) => c);

            var results = await query.ToListAsync();

            return results;
        }

        public IQueryable<AssignmentCategory> GetCategoriesOfOwnerIdQuery(OwnerLevel ownerLevel, string ownerId)
        {
            var query = _assignmentsDb.AssignmentCategories.AsNoTracking()
                .Where(c => (c.OwnerLevel == ownerLevel) && (c.OwnerId == ownerId));
            return query;
        }

        public async Task<List<UserGroup>> GetUserGroupsAssignedToAssignmentAsync(string assignmentId)
        {
            var assignmentGroups = await _assignmentsDb.AssignmentUserGroups.AsNoTracking()
                .Where(g => g.AssignmentId == assignmentId)
                .ToListAsync();

            var results = await _connectDb.UserGroups.AsNoTracking()
                .Join(assignmentGroups,
                    ug => ug.Id,
                    ng => ng.UserGroupId,
                    (ug, ng) => ug)
                .ToListAsync();

            return results;
        }

        public async Task UnpublishAssignmentAsync(string assignmentId)
        {
            var assignment = await _assignmentsDb.Assignments
                .Where(x => x.Id == assignmentId)
                .FirstOrDefaultAsync();
            if (assignment == null)
                throw new Exception("Unable to unpublish assignment (assignment not found [" + assignmentId + "])");
            if (assignment.Status == AssignmentStatus.Published)
            {
                assignment.Status = AssignmentStatus.Draft;
                await _assignmentsDb.SaveChangesAsync();
            }
            else if (assignment.Status != AssignmentStatus.Draft)
            {
                throw new Exception("Unable to unpublish assignment (Id:" + assignmentId + ", Status:" + assignment.Status + ")");
            }
        }

        protected async Task UpdateAssignmentCategoriesAsync(string assignmentId, List<string> categoryIds)
        {
            var currentCategories = await _assignmentsDb.AssignmentCategoryLinks
                .Where(x => x.AssignmentId == assignmentId)
                .ToListAsync();

            foreach (var currentCategory in currentCategories)
            {
                if (!categoryIds.Contains(currentCategory.CategoryId))
                    _assignmentsDb.AssignmentCategoryLinks.Remove(currentCategory);
            }
            foreach (var categoryId in categoryIds)
            {
                if (!currentCategories.Where(x => x.CategoryId == categoryId).Any())
                    _assignmentsDb.AssignmentCategoryLinks.Add(new AssignmentCategoryLink() { AssignmentId = assignmentId, CategoryId = categoryId });
            }
        }

        protected async Task UpdateAssignmentConnectionGroupsAsync(string assignmentId, List<string> userGroupIds)
        {
            var currentGroups = await _assignmentsDb.AssignmentUserGroups
                .Where(x => x.AssignmentId == assignmentId).ToListAsync();
            foreach (var group in currentGroups)
            {
                if (!userGroupIds.Contains(group.UserGroupId))
                    _assignmentsDb.AssignmentUserGroups.Remove(group);
            }
            foreach (var groupId in userGroupIds)
            {
                if (!currentGroups.Where(x => x.UserGroupId == groupId).Any())
                    _assignmentsDb.AssignmentUserGroups.Add(new AssignmentUserGroup() { AssignmentId = assignmentId, UserGroupId = groupId });
            }
        }

        public async Task UpdateAssignmentAsync(Assignment assignment, List<string> categoryIds, List<string> connectionGroupIds)
        {
            var oldAssignment = await _assignmentsDb.Assignments.Where(x => x.Id == assignment.Id).FirstOrDefaultAsync();
            if (oldAssignment == null)
                throw new Exception("Unable to find Assignment (Id = " + assignment.Id + ")");

            if (oldAssignment.Status != NotificationStatus.Draft)
                throw new Exception($"Unable to modify assignments that are not in draft status (id={assignment.Id}, status={assignment.Status}).");

            oldAssignment.OwnerLevel = assignment.OwnerLevel;
            oldAssignment.OwnerId = assignment.OwnerId ?? oldAssignment.OwnerId;
            oldAssignment.Title = assignment.Title;
            oldAssignment.AssignmentBody = assignment.AssignmentBody;
            oldAssignment.DueUTC = assignment.DueUTC;
            oldAssignment.TimeZoneId = assignment.TimeZoneId;
            oldAssignment.Status = assignment.Status;
            oldAssignment.Title = assignment.Title;
            oldAssignment.AllowComments = assignment.AllowComments;
            oldAssignment.SendNotification = assignment.SendNotification;

            await UpdateAssignmentCategoriesAsync(assignment.Id, categoryIds);
            await UpdateAssignmentConnectionGroupsAsync(assignment.Id, connectionGroupIds);

            await _assignmentsDb.SaveChangesAsync();
        }

        public async Task InsertAssignmentAsync(Assignment assignment, List<string> categoryIds, List<string> connectionGroupIds)
        {
            if (string.IsNullOrEmpty(assignment.Id))
                assignment.Id = Guid.NewGuid().ToString("N");

            assignment.CreatedUTC = DateTime.UtcNow;
            _assignmentsDb.Assignments.Add(assignment);

            await UpdateAssignmentCategoriesAsync(assignment.Id, categoryIds);
            await UpdateAssignmentConnectionGroupsAsync(assignment.Id, connectionGroupIds);

            await _assignmentsDb.SaveChangesAsync();
        }

        public async Task<bool> DeleteAssignmentAsync(string assignmentId)
        {
            var assignment = await _assignmentsDb.Assignments
                .Where(x => (x.Id == assignmentId) && (x.Status != AssignmentStatus.Published))
                .FirstOrDefaultAsync();

            if (assignment == null)
                return false;

            var links = await _assignmentsDb.AssignmentCategoryLinks.Where(x => x.AssignmentId == assignmentId).ToListAsync();
            foreach (var link in links)
                _assignmentsDb.AssignmentCategoryLinks.Remove(link);

            var groups = await _assignmentsDb.AssignmentUserGroups.Where(x => x.AssignmentId == assignmentId).ToListAsync();
            foreach (var group in groups)
                _assignmentsDb.AssignmentUserGroups.Remove(group);

            _assignmentsDb.Assignments.Remove(assignment);
            await _assignmentsDb.SaveChangesAsync();
            return true;
        }

        public async Task<List<User>> GetAssignedUsersAsync(string notificationId, bool? allowEmailMessaging = null, bool? allowSmsMessaging = null)
        {
            var userGroups = _connectDb.UserGroups.AsNoTracking()
                .Join(
                    _connectDb.NotificationUserGroups.Where(x => x.NotificationId == notificationId),
                    ug => ug.Id,
                    nug => nug.UserGroupId,
                    (ug, nug) => ug
                );

            var memberships = _connectDb.UserGroupMemberships.AsNoTracking();
            if (allowEmailMessaging != null)
                memberships = memberships.Where(x => x.AllowEmailMessaging == allowEmailMessaging);
            if (allowSmsMessaging != null)
                memberships = memberships.Where(x => x.AllowSmsMessaging == allowSmsMessaging);
            
            var userIds = await memberships
                .Join(
                    userGroups,
                    m => m.UserGroupId,
                    g => g.Id,
                    (m, g) => m.UserId
                ).Distinct()
                .ToListAsync();

            var users = await _identityDb.Users.Where(u => userIds.Contains(u.Id))
                .AsNoTracking()
                .ToListAsync();

            return users;
        }




    }
}
