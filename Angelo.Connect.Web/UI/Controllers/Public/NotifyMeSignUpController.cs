using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Angelo.Connect.Extensions;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Microsoft.Extensions.Logging;
using Angelo.Connect.Security;
using Microsoft.AspNetCore.Authorization;
using Angelo.Connect.Configuration;
using Microsoft.Extensions.Options;
using Angelo.Connect.Rendering;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace Angelo.Connect.Web.UI.Controllers.Public
{
    [RequireFeature(FeatureId.Notifications)]
    public class NotifyMeSignUpController : BaseController
    {
        const String NOTIFY_ME_REDIRECT_URL = "NOTIFY_ME_REDIRECT_URL";

        private UserGroupManager _userGroupManager;
        private NotificationManager _notificationManager;
        private UserContext _userContext;
        private SiteContext _siteContext;
        private AegisOptions _aegisOptions;
        private PageMasterManager _masterPageManager;

        public NotifyMeSignUpController(ILogger<NotifyMeSignUpController> logger, 
            UserGroupManager userGroupManager,
            NotificationManager notificationManager,
            UserContext userContext,
            SiteContextAccessor siteContextAccessor,
            PageMasterManager masterPageManager,
            IOptions<AegisOptions> aegisOptions) : base(logger)
        {
            _userGroupManager = userGroupManager;
            _notificationManager = notificationManager;
            _userContext = userContext;
            _siteContext = siteContextAccessor.GetContext();
            _masterPageManager = masterPageManager;
            _aegisOptions = aegisOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Start(string redirectUrl)
        {
            if (string.IsNullOrEmpty(redirectUrl))
                redirectUrl = Request.Headers["Referer"].ToString();
            if (string.IsNullOrEmpty(redirectUrl))
                redirectUrl = "/";

            Response.Cookies.Append(NOTIFY_ME_REDIRECT_URL, redirectUrl);

            var subscribeLink = Url.Link("Notify Me", new { action = "Subscribe" });
            return Redirect(subscribeLink);
        }

        [HttpGet]
        public async Task<IActionResult> Subscribe()
        {
            var redirectUrl = Request.Cookies[NOTIFY_ME_REDIRECT_URL];

            if (string.IsNullOrEmpty(redirectUrl))
                redirectUrl = "/";

            ViewData["redirectUrl"] = redirectUrl;
            ViewData["siteId"] = _siteContext.SiteId;

            if (User.Identity.IsAuthenticated)
            {
                ViewData["userId"] = User.GetUserId();

                return this.MasterPageView("~/UI/Views/Public/NotifyMeSignUp/SubscribeAdmin.cshtml", "Notify Me");
            }
            else
            {
                var subscribeUrl = Url.Link("Notify Me", new { action = "Subscribe" });
                var loginUrl = "/sys/account/login?ru=" + System.Net.WebUtility.UrlEncode(subscribeUrl);
                var registerUrl = "/sys/account/register?ru=" + System.Net.WebUtility.UrlEncode(subscribeUrl);

                ViewData["userId"] = string.Empty;
                ViewData["loginUrl"] = loginUrl;
                ViewData["registerUrl"] = registerUrl;

                return this.MasterPageView("~/UI/Views/Public/NotifyMeSignUp/SubscribeRequest.cshtml", "Notify Me");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Unsubscribe(string eModel)
        {
            if (string.IsNullOrEmpty(eModel))
                return Redirect("/");

            NotificationUnsubscribe model = ModelEncryptor.Decrypt<NotificationUnsubscribe>(eModel);

            var userGroups = await _notificationManager.GetUserGroupsAssignedToNotificationAndUserAsync(model.NotificationId, model.UserId);

            if (userGroups.Count == 0)
            {
                return this.MasterPageView("~/UI/Views/Public/NotifyMeSignUp/UnsubscribeSuccess.cshtml", "Unsubscribe");
            }
            else
            {
                ViewData["UserGroups"] = userGroups;
                ViewData["eModel"] = eModel;

                return this.MasterPageView("~/UI/Views/Public/NotifyMeSignUp/UnsubscribeRequest.cshtml", "Unsubscribe");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UnsubscribeRequest()
        {
            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> UnsubscribeRequest(string eModel, string accept)
        {
            if ((accept ?? "No") == "Yes")
            {
                NotificationUnsubscribe model = ModelEncryptor.Decrypt<NotificationUnsubscribe>(eModel);

                var userGroups = await _notificationManager.GetUserGroupsAssignedToNotificationAndUserAsync(model.NotificationId, model.UserId);
                foreach (var userGroup in userGroups)
                {
                    await _userGroupManager.UnsubscribeFromNotificationGroupAsync(userGroup.Id, model.UserId);
                }

                return this.MasterPageView("/UI/Views/Public/NotifyMeSignUp/UnsubscribeSuccess.cshtml", model, "Unsubsrcribe");
            }
            else
            {
                return LocalRedirect("/");
            }
        }

        private string GetDefaultMasterPageId()
        {
            var masterPage = _masterPageManager.GetSiteDefaultAsync(_siteContext.SiteId).Result;

            if (masterPage == null)
                throw new NullReferenceException("Could not get default master page for site");

            return masterPage.Id;
        }

        //private string GetSubscribeLink(string redirectUrl = null)
        //{
        //    var selfLink = Url.Link("Notify Me", new { action = "Subscribe" });
        //    if (string.IsNullOrEmpty(redirectUrl))
        //        redirectUrl = Request.Headers["Referer"].ToString();
        //    if (string.IsNullOrEmpty(redirectUrl))
        //        redirectUrl = "/";
        //    selfLink += "?redirectUrl=" + System.Net.WebUtility.UrlEncode(redirectUrl);
        //    return selfLink;
        //}



    }
}
