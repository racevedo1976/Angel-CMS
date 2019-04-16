using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Common.Mvc.ActionResults;
using AutoMapper.Extensions;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System;

namespace Angelo.Connect.Web.UI.Components
{
    public class AccountNotificationDetailsEdit : ViewComponent
    {
        private NotificationManager _notificationManager;

        public AccountNotificationDetailsEdit(NotificationManager notificationManager)
        {
            _notificationManager = notificationManager;
        }

        private async Task LoadUserGroupsAsync(NotificationDetailsViewModel model, string id)
        {
            var userGroups = await _notificationManager.GetUserGroupsAssignedToNotificationAsync(id);
            foreach (var ug in userGroups)
            {
                if (ug.UserGroupType == UserGroupType.ConnectionGroup)
                    model.ConnectionGroups.Add(ug);
                else
                    model.NotificationGroups.Add(ug);
            }
        }

        private List<SelectListItem> GetEmailHeaderSelectListItems(string selectedEmailHeaderId)
        {
            var emailHeaders = _notificationManager.GetEmailHeaders();
            var list = emailHeaders.Select(h => new SelectListItem()
            {
                Value = h.Id,
                Text = h.Title,
                Selected = (h.Id == selectedEmailHeaderId)
            }).ToList();
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IViewComponentResult> InvokeAsync(string id, string ownerLevel, string ownerId)
        {
            if (string.IsNullOrEmpty(ownerLevel))
                return new ViewComponentPlaceholder();

            NotificationDetailsViewModel model;
            if (string.IsNullOrEmpty(id))
            {
                // Create new notification
                model = new NotificationDetailsViewModel()
                {
                    OwnerLevel = (OwnerLevel)Enum.Parse(typeof(OwnerLevel), ownerLevel),
                    OwnerId = ownerId,
                    Status = NotificationStatus.Draft,
                    ScheduleAction = NotificationDetailsViewModel.ScheduleActions.Draft,
                    SendEmail = true,
                    TimeZoneId = TimeZoneHelper.DefaultTimeZoneId // TO DO: Get the default time zone from the user context.
                };
                model.ScheduledDT = TimeZoneHelper.Now(model.TimeZoneId);
            }
            else
            {
                // Load existing notification
                var note = await _notificationManager.GetNotificationAsync(id);
                if (note == null)
                    return new ViewComponentPlaceholder();
                model = note.ProjectTo<NotificationDetailsViewModel>();
                model.ScheduleAction = NotificationDetailsViewModel.ScheduleActions.Draft;
                if (string.IsNullOrEmpty(model.TimeZoneId))
                {
                    model.TimeZoneId = TimeZoneHelper.DefaultTimeZoneId; // TO DO: Get the default time zone from the user context.
                    model.ScheduledDT = TimeZoneHelper.Now(model.TimeZoneId);
                }
                await LoadUserGroupsAsync(model, id);
            }

            ViewData["EmailHeaderSelectListItems"] = GetEmailHeaderSelectListItems(model.EmailHeaderId);
            ViewData["ownerLevel"] = ownerLevel ?? string.Empty;
            ViewData["ownerId"] = ownerId ?? string.Empty;
            ViewData["TimeZoneList"] = TimeZoneHelper.GetTimeZoneSelectList();

            return await Task.Run(() => {
                return View(model);
            });
        }
    }
}
