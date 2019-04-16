using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Angelo.Identity;
using Angelo.Identity.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public class NotificationManager
    {
        private ConnectDbContext _connectDb;
        private IdentityDbContext _identityDb;

        public NotificationManager(ConnectDbContext connectDb, IdentityDbContext identityDb)
        {
            _connectDb = connectDb;
            _identityDb = identityDb;
        }

        public IQueryable<Notification> GetNotificationsOfOwnerQuery(OwnerLevel ownerLevel, string ownerId)
        {
            var query = _connectDb.Notifications.Where(x => (x.OwnerLevel == ownerLevel) && (x.OwnerId == ownerId));
            return query;
        }

        public async Task<Notification> GetNotificationAsync(string id)
        {
            var note = await _connectDb.Notifications.Where(x => x.Id == id).FirstOrDefaultAsync();
            return note;
        }

        public async Task<List<UserGroup>> GetUserGroupsAssignedToNotificationAsync(string notificationId)
        {
            var query = _connectDb.UserGroups.AsNoTracking()
                .Join(_connectDb.NotificationUserGroups.Where(x => x.NotificationId == notificationId),
                    ug => ug.Id,
                    ng => ng.UserGroupId,
                    (ug, ng) => ug);

            var results = await query.ToListAsync();

            return results;
        }

        public async Task<List<UserGroup>> GetUserGroupsAssignedToNotificationAndUserAsync(string notificationId, string userId)
        {
            var query = _connectDb.UserGroups.AsNoTracking()
                .Join(_connectDb.NotificationUserGroups.Where(x => x.NotificationId == notificationId),
                    ug => ug.Id,
                    ng => ng.UserGroupId,
                    (ug, ng) => ug)
                .Join(_connectDb.UserGroupMemberships.Where(x => x.UserId == userId),
                    ug => ug.Id,
                    m => m.UserGroupId,
                    (ug, m) => ug
                   );

            var results = await query.ToListAsync();

            return results;
        }

        public List<NotificationEmailHeader> GetEmailHeaders()
        {
            var list = _connectDb.NotificationEmailHeaders.ToList();
            return list;
        }

        public async Task UnscheduleNotificationAsync(string Id)
        {
            var note = await _connectDb.Notifications.Where(x => x.Id == Id).FirstOrDefaultAsync();
            if (note == null)
                throw new Exception("Unable to unschedule notification (notification not found [" + Id + "])");
            if ((note.Status == NotificationStatus.Scheduled))
            {
                note.Status = NotificationStatus.Draft;
                await _connectDb.SaveChangesAsync();
            }
            else if (note.Status != NotificationStatus.Draft)
            {
                throw new Exception("Unable to unschedule notification (Id:" + Id + ", Status:" + note.Status + ")");
            }
        }

        public async Task UpdateNotificationAsync(Notification note, List<string> userGroupIds)
        {
            var oldNote = await _connectDb.Notifications.Where(x => x.Id == note.Id).FirstOrDefaultAsync();
            if (oldNote == null)
                throw new Exception("Unable to find Notification (Id = " + note.Id + ")");

            if (oldNote.Status != NotificationStatus.Draft)
                throw new Exception($"Unable to modify notifications that are not in draft status (id={oldNote.Id}, status={oldNote.Status}).");

            oldNote.OwnerLevel = note.OwnerLevel;
            oldNote.OwnerId = note.OwnerId;
            oldNote.SendEmail = note.SendEmail;
            oldNote.SendSms = note.SendSms;
            oldNote.EmailHeaderId = note.EmailHeaderId;
            oldNote.EmailSubject = note.EmailSubject;
            oldNote.EmailBody = note.EmailBody;
            oldNote.SmsMessage = note.SmsMessage;
            oldNote.ScheduledUTC = note.ScheduledUTC;
            oldNote.TimeZoneId = note.TimeZoneId;
            oldNote.Status = note.Status;
            oldNote.Title = note.Title;

            var nuGroups = await _connectDb.NotificationUserGroups.Where(x => x.NotificationId == note.Id).ToListAsync();
            foreach (var nuGroup in nuGroups)
            {
                if (!userGroupIds.Contains(nuGroup.UserGroupId))
                    _connectDb.NotificationUserGroups.Remove(nuGroup);
            }
            foreach (var ugId in userGroupIds)
            {
                if (!nuGroups.Where(x => x.UserGroupId == ugId).Any())
                    _connectDb.NotificationUserGroups.Add(new NotificationUserGroup() { NotificationId = note.Id, UserGroupId = ugId });
            }
            await _connectDb.SaveChangesAsync();
        }

        public async Task InsertNotificationAsync(Notification note, List<string> userGroupIds)
        {
            _connectDb.Notifications.Add(note);
            foreach (var groupId in userGroupIds)
                _connectDb.NotificationUserGroups.Add(new Models.NotificationUserGroup() { NotificationId = note.Id, UserGroupId = groupId });
            await _connectDb.SaveChangesAsync();
        }

        public async Task<bool> DeleteNotification(string notificationId)
        {
            var note = await _connectDb.Notifications
                .Where(x => (x.Id == notificationId) && (x.Status != NotificationStatus.Processing))
                .FirstOrDefaultAsync();

            if (note == null)
                return false;

            var groups = await _connectDb.NotificationUserGroups.Where(x => x.NotificationId == notificationId).ToListAsync();
            foreach (var group in groups)
                _connectDb.NotificationUserGroups.Remove(group);
            _connectDb.Notifications.Remove(note);
            await _connectDb.SaveChangesAsync();
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

        public async Task<List<NotificationEmailLog>> GetNotificationEmailLogsAsync(string notificationId)
        {
            var logs = await _connectDb.NotificationEmailLog.AsNoTracking()
                .Where(log => log.NotificationId == notificationId)
                .ToListAsync();
            return logs;
        }

        public async Task InsertNotificationEmailLogAsync(NotificationEmailLog logEntry)
        {
            _connectDb.NotificationEmailLog.Add(logEntry);
            await _connectDb.SaveChangesAsync();
        }

        public async Task<List<NotificationSmsLog>> GetNotificationSmsLogsAsync(string notificationId)
        {
            var logs = await _connectDb.NotificationSmsLog.AsNoTracking()
                .Where(log => log.NotificationId == notificationId)
                .ToListAsync();
            return logs;
        }

        public async Task<NotificationEmailHeader> GetEmailHeaderAsync(string emailHeaderId)
        {
            var emailHeader = await _connectDb.NotificationEmailHeaders.Where(x => x.Id == emailHeaderId).FirstOrDefaultAsync();
            return emailHeader;
        }



    }
}
