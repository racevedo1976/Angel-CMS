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

namespace Angelo.Connect.Web.UI.Components
{
    public class SiteNotificationDetailsView : ViewComponent
    {
        private NotificationManager _notificationManager;

        public SiteNotificationDetailsView(NotificationManager notificationManager)
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
            if (string.IsNullOrEmpty(id))
                return new ViewComponentPlaceholder();

            var note = await _notificationManager.GetNotificationAsync(id);
            if (note == null) 
                return new ViewComponentPlaceholder();

            var model = note.ProjectTo<NotificationDetailsViewModel>();
            await LoadUserGroupsAsync(model, id);

            model.TimeZoneName = TimeZoneHelper.NameOfTimeZoneId(model.TimeZoneId);

            ViewData["EmailHeaderSelectListItems"] = GetEmailHeaderSelectListItems(note.EmailHeaderId);
            ViewData["ownerLevel"] = ownerLevel ?? string.Empty;
            ViewData["ownerId"] = ownerId ?? string.Empty;

            return await Task.Run(() => {
                return View(model);
            });
        }
    }
}
