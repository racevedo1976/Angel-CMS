using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Angelo.Common.Extensions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using AutoMapper.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using System;
using Angelo.Connect.Security;
using System.Collections.Generic;
using Angelo.Jobs;
using Angelo.Identity.Models;
using Angelo.Identity;
using Microsoft.AspNetCore.Authorization;

namespace Angelo.Connect.Web.UI.Controllers.Api.ClientLevel
{
    [RequireFeature(FeatureId.Notifications)]
    public class ClientNotificationsDataController : ClientControllerBase
    {
        private PageManager _pageManager;
        private NotificationManager _notificationManager;
        private UserGroupManager _userGroupManager;
        private SiteManager _siteManager;
        private ClientManager _clientManager;
        private UserContext _userContext;
        private IOptions<RequestLocalizationOptions> _localizationOptions;
        private IJobsManager _jobs;

        public ClientNotificationsDataController(PageManager pageManager, 
            SiteContext siteContext, 
            NotificationManager notificationManager, 
            UserGroupManager userGroupManager,
            SiteManager siteManager, 
            ILogger<SecurityPoolManager> logger,
            IJobsManager jobs,
            ClientManager clientManager, 
            UserContext userContext,
            ClientAdminContextAccessor clientContextAccessor,
            IAuthorizationService authorizationService,
            IOptions<RequestLocalizationOptions> localizationOptions) : base(clientContextAccessor, authorizationService, logger)
            {
            _pageManager = pageManager;
            _notificationManager = notificationManager;
            _userGroupManager = userGroupManager;
            _siteManager = siteManager;
            _clientManager = clientManager;
            _jobs = jobs;
            _userContext = userContext;
            _localizationOptions = localizationOptions;
        }

        
        [Authorize(policy: PolicyNames.ClientNotificationGroupRead)]
        [HttpPost, Route("/sys/clients/{tenant}/api/notifications/data")]
        public async Task<JsonResult> Data([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId)
        {
            OwnerLevel oLevel;
            
            if (!Enum.TryParse(ownerLevel, true, out oLevel))
                throw new Exception("Unknown OwnerLevel [" + ownerLevel ?? "" + "]");

            var query = _notificationManager.GetNotificationsOfOwnerQuery(oLevel, ownerId);
            var result = query.OrderByDescending(x => x.CreatedUTC).ToDataSourceResult(request);
            result.Data = result.Data.ProjectTo<NotificationListItemViewModel>();
            return Json(result);
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupRead)]
        [HttpPost, Route("/sys/clients/{tenant}/api/notifications/groups")]
        public JsonResult GetNotificationGroups([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId, string tenant)
        {
            return GetUserGroups(request, ownerLevel, ownerId, UserGroupType.NotificationGroup);
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupRead)]
        [HttpPost, Route("/sys/clients/{tenant}/api/notifications/connectiongroups")]
        public JsonResult GetConnectionGroups([DataSourceRequest] DataSourceRequest request, string ownerLevel, string ownerId, string tenant)
        {
            return GetUserGroups(request, ownerLevel, ownerId, UserGroupType.ConnectionGroup);
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupEdit)]
        [HttpPost, Route("/sys/clients/{tenant}/api/notifications/unschedule")]
        public async Task<ActionResult> UnscheduleNotification(string Id)
        {
            try
            {
                await _notificationManager.UnscheduleNotificationAsync(Id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupEdit)]
        [HttpPost, Route("/sys/clients/{tenant}/api/notifications")]
        public async Task<ActionResult> SaveNotification(NotificationDetailsViewModel model)
        {
            //ModelState.AddModelError("EmailSubject", "Test Error.");

            if (ModelState.IsValid)
            {
                model.Title = BuildNotificationTitle(model);

                if (model.ScheduleAction == NotificationDetailsViewModel.ScheduleActions.Draft)
                {
                    model.Status = NotificationStatus.Draft;
                }
                else if (model.ScheduleAction == NotificationDetailsViewModel.ScheduleActions.SendNow)
                {
                    model.Status = NotificationStatus.Scheduled;
                    model.TimeZoneId = TimeZoneHelper.DefaultTimeZoneId; // TO DO: Get the time zone from the user context.
                    model.ScheduledDT = TimeZoneHelper.Now(model.TimeZoneId);
                }
                else
                    model.Status = NotificationStatus.Scheduled;

                var userGroupIds = new List<string>();
                userGroupIds.AddRange(model.NotificationGroupIds);
                userGroupIds.AddRange(model.ConnectionGroupIds);

                if (string.IsNullOrEmpty(model.Id))
                {
                    model.Id = Guid.NewGuid().ToString("N");
                    model.CreatedUTC = DateTime.UtcNow;
                    model.CreatedBy = User.GetUserId();
                    var insertNote = model.ProjectTo<Angelo.Connect.Models.Notification>();
                    await _notificationManager.InsertNotificationAsync(insertNote, userGroupIds);
                }
                else
                {
                    var updateNote = model.ProjectTo<Angelo.Connect.Models.Notification>();
                    await _notificationManager.UpdateNotificationAsync(updateNote, userGroupIds);
                }

                if (model.ScheduleAction == NotificationDetailsViewModel.ScheduleActions.SendNow)
                {
                    await _jobs.EnqueueAsync<NotificationProcessor>(proc => proc.ExecuteNotification(model.Id));
                    model.Status = NotificationStatus.Processing;
                }

                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupRead)]
        [HttpPost, Route("/sys/clients/{tenant}/api/notifications/{id}")]
        public async Task<JsonResult> GetNotification(string id)
        {
            var note = await _notificationManager.GetNotificationAsync(id);
            if (note == null)
                throw new Exception("Unable to find NotificationId:" + id);
            return Json(note);
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupDelete)]
        [HttpDelete, Route("/sys/clients/{tenant}/api/notifications")]
        public async Task<ActionResult> DeleteNotification(string Id)
        {
            if (string.IsNullOrEmpty(Id))
                return BadRequest();

            await _notificationManager.DeleteNotification(Id);
            return Ok(new { id = Id });
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupEdit)]
        [HttpPost, Route("/sys/clients/{tenant}/api/notifications/statuschange")]
        public async Task<JsonResult> GetStatusChange(IEnumerable<NotificationListItemViewModel> data)
        {
            var rData = new List<NotificationListItemViewModel>();
            foreach (var item in data)
            {
                var note = await _notificationManager.GetNotificationAsync(item.Id);
                if (note != null)
                    if (item.Status != note.Status)
                        rData.Add(note.ProjectTo<NotificationListItemViewModel>());
            }
            return Json(rData);
        }

        [Authorize(policy: PolicyNames.ClientNotificationGroupRead)]
        [HttpPost, Route("/sys/clients/{tenant}/api/notifications/log")]
        public async Task<JsonResult> GetNotificationLogList([DataSourceRequest] DataSourceRequest request, string notificationId)
        {
            var logs = new List<NotificationLogEntryViewModel>();

            // Add the email log entires
            var emailLogs = await _notificationManager.GetNotificationEmailLogsAsync(notificationId);
            foreach (var emailLog in emailLogs)
            {
                logs.Add(new NotificationLogEntryViewModel()
                {
                    Id = emailLog.Id,
                    SentDT = TimeZoneHelper.ConvertFromUTC(emailLog.SentUTC, TimeZoneHelper.DefaultTimeZoneId),
                    ToAddress = emailLog.EmailAddress,
                    LogEntryType = NotificationType.Email
                });
            }

            // Add the sms log entries
            var smsLogs = await _notificationManager.GetNotificationSmsLogsAsync(notificationId);
            foreach (var smsLog in smsLogs)
            {
                logs.Add(new NotificationLogEntryViewModel()
                {
                    Id = smsLog.Id,
                    SentDT = TimeZoneHelper.ConvertFromUTC(smsLog.SentUTC, TimeZoneHelper.DefaultTimeZoneId),
                    ToAddress = smsLog.MobileNumber,
                    LogEntryType = NotificationType.SMS
                });
            }

            var result = logs.ToDataSourceResult(request); 
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

        protected string BuildNotificationTitle(NotificationDetailsViewModel model)
        {
            if (!string.IsNullOrEmpty(model.EmailSubject))
                return (model.EmailSubject.Length > 30) ? model.EmailSubject.Substring(0, 30) : model.EmailSubject;
            else if (!String.IsNullOrEmpty(model.SmsMessage))
                return (model.SmsMessage.Length > 30) ? model.SmsMessage.Substring(0, 30) : model.SmsMessage;
            else
                return model.Id;
        }

    }
}
