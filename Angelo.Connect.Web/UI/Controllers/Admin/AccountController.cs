using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Angelo.Connect.Menus;
using Angelo.Connect.Configuration;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Web.UI.ViewModels.Extensions;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;

using Angelo.Identity;
using Angelo.Connect.Security;
using Kendo.Mvc.UI;
using Angelo.Identity.Models;
using Kendo.Mvc.Extensions;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Rendering;
using Angelo.Identity.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Angelo.Connect.Web.UI.Controllers.Admin
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class AccountController : BaseController
    {
        private AegisOptions _oidConfig;
        private ServerOptions _serverOptions;
        private DriveOptions _driveOptions;
        private TagManager _tags;
        private MenuProvider _menuProvider;
        private UserGroupManager _userGroupManager;
        private SecurityPoolManager _poolManager;
        private UserContextAccessor _userContextAccessor;
        private SiteContextAccessor _siteContextAccessor;
        private PageMasterManager _masterPageManager;

        private IFolderManager<FileDocument> _folderManager;
        private IDocumentService<FileDocument> _documentService;
        private UserManager _userManager;
        private GroupManager _groupManager;

        //TODO: Rename to account controller

        public AccountController(IOptions<AegisOptions> openIdOptions,
            IOptions<ServerOptions> serverOptions,
            IOptions<DriveOptions> driveOptions,
            IFolderManager<FileDocument> folderManager,
            IDocumentService<FileDocument> documentService,
            ILogger<AccountController> logger,
            TagManager tags,
            MenuProvider menuProvider,
            UserGroupManager userGroupManager,
            SiteContextAccessor siteContextAccessor,
            UserManager userManager,
            PageMasterManager masterPageManager,
            UserContextAccessor userContextAccessor,
            SecurityPoolManager poolManager,
            GroupManager groupManager) : base(logger)
        {
            _oidConfig = openIdOptions.Value;
            _serverOptions = serverOptions.Value;
            _driveOptions = driveOptions.Value;
            _tags = tags;
            _menuProvider = menuProvider;
            _folderManager = folderManager;
            _documentService = documentService;
            _userGroupManager = userGroupManager;
            _userManager = userManager;
            _poolManager = poolManager;
            _userContextAccessor = userContextAccessor;
            _siteContextAccessor = siteContextAccessor;
            _masterPageManager = masterPageManager;
            _groupManager = groupManager;
        }

        [HttpPost]
        public IActionResult Culture(string culture, string returnUrl)
        {
            _serverOptions.SetRequestCulture(this.HttpContext, culture);

            return LocalRedirect(returnUrl);
        }

        [Authorize]
        public IActionResult Login(string ru = null)
        {
            // this method is secured to force the openid middle ware to redirect user to our login provider
            // after the user logs in, they'll be redirected back here then we'll redirect them to their profile
            if (string.IsNullOrEmpty(ru))
            {
                return Redirect("/");
            }
            else
            {
                return Redirect(ru);
            }
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var siteContext = _siteContextAccessor.GetContext();

            var userId = User.GetUserId();
            var siteSecurityPoolId = siteContext.SecurityPoolId;
            var clientSecurityPoolId = siteContext.Client.SecurityPoolId;
            var thisUser = await _userManager.GetUserAsync(userId);
            var userSiteRoles = await _poolManager.GetUserRolesAsync(siteSecurityPoolId, userId);
            var userClientRoles = await _poolManager.GetUserRolesAsync(clientSecurityPoolId, userId);
            var userCorpRoles = await _poolManager.GetUserRolesAsync("MyCompany-corp-pool", userId);
            //TODO: pull in user claims (permissions)
            var model = thisUser.ProjectTo<UserProfileViewModel>();
            model.SiteRoles = userSiteRoles;
            model.ClientRoles = userClientRoles;
            model.CorpRoles = userCorpRoles;

            var providers = await _userManager.GetWirelessProvidersAsync();
            model.WirelessProviderId = model.WirelessProviderId == null ? "" : providers.FirstOrDefault(p => p.Id == model.WirelessProviderId).Name;


            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> ProfileEdit()
        {
            var siteContext = _siteContextAccessor.GetContext();

            var userId = User.GetUserId();
            var siteSecurityPoolId = siteContext.SecurityPoolId;
            var clientSecurityPoolId = siteContext.Client.SecurityPoolId;
            var thisUser = await _userManager.GetUserAsync(userId);
            var userSiteRoles = await _poolManager.GetUserRolesAsync(siteSecurityPoolId, userId);
            var userClientRoles = await _poolManager.GetUserRolesAsync(clientSecurityPoolId, userId);
            var userCorpRoles = await _poolManager.GetUserRolesAsync("MyCompany-corp-pool", userId);
            //TODO: pull in user claims (permissions)
            var model = thisUser.ProjectTo<UserProfileViewModel>();

            var providers = await _userManager.GetWirelessProvidersAsync();
            var providerSelectList = providers.Select(p => new SelectListItem() { Value = p.Id, Text = p.Name }).ToList();
            providerSelectList.Insert(0, new SelectListItem() { Value = "", Text = "------ Select -----" });
            ViewData["providerSelectList"] = providerSelectList;

            model.SiteRoles = userSiteRoles;
            model.ClientRoles = userClientRoles;
            model.CorpRoles = userCorpRoles;

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ProfileEdit(UserProfileViewModel userModel)
        {
            if (ModelState.IsValid)
            {
                var userId = userModel.Id;
                var thisUser = await _userManager.GetUserAsync(userModel.Id);

                thisUser.FirstName = userModel.FirstName;
                thisUser.LastName = userModel.LastName;
                thisUser.PhoneNumber = userModel.PhoneNumber;
                thisUser.BirthDate = userModel.BirthDate.Value;
                thisUser.WirelessProviderId = userModel.WirelessProviderId;
                thisUser.DisplayName = userModel.DisplayName;

                await _userManager.UpdateAsync(thisUser);
                
                return RedirectToAction("Profile");
            }

            return View(userModel);
        }


        [Authorize]
        public IActionResult Subscriptions()
        {
            var siteContext = _siteContextAccessor.GetContext();
            var userId = User.GetUserId();
            var siteId = siteContext.SiteId;

            ViewData["userId"] = userId;
            ViewData["siteId"] = siteId;

            return View();
        }

        [Authorize]
        public IActionResult Groups()
        {
            var siteContext = _siteContextAccessor.GetContext();
            var userId = User.GetUserId();
            var siteId = siteContext.SiteId;

            ViewData["userId"] = userId;
            ViewData["siteId"] = siteId;

            ViewData["ownerId"] = userId;
            ViewData["poolId"] = siteContext.SecurityPoolId;
            ViewData["userGroupTitle"] = "Connection Groups";

            return View("UserGroups");
        }

        [Authorize]
        public async Task<IActionResult> Documents()
        {
            ViewBag.DriveUrl = _driveOptions.Authority;
            ViewBag.DirectoryId = (await _userManager.GetUserAsync(User.GetUserId())).DirectoryId;

            var client = new HttpClient();

            //client.SetBearerToken(token);
            //Get the root folder as the current folder
            var model = (await _folderManager.GetRootFolderAsync(User.GetUserId())).ToFolderViewModel();

            // Since Folders and Documents are now disconnected, the FolderViewModel is no longer one-to-one with Folder, so the child items
            // need to be populated
            await PopulateDocumentsAsync(model);

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Library()
        {
            var userContext = _userContextAccessor.GetContext();
            var hasAccess = userContext.SecurityClaims.Any(x =>
                    (x.Type == UserClaimTypes.PersonalLibraryOwner) ||
                    (x.Type == ClientClaimTypes.PrimaryAdmin) ||
                    (x.Type == SiteClaimTypes.SitePrimaryAdmin) ||
                    (x.Type.StartsWith("corp-") && x.Value == ConnectCoreConstants.CorporateId)
                );
            if (hasAccess)
            {
                ViewBag.DriveUrl = _driveOptions.Authority;
                ViewBag.DirectoryId = (await _userManager.GetUserAsync(User.GetUserId())).DirectoryId;

                var client = new HttpClient();

                // Since Folders and Documents are now disconnected, the FolderViewModel is no longer one-to-one with Folder, so the child items
                // need to be populated
                //await PopulateDocumentsAsync(model);

                return View("ContentBrowser");
            }
            else
            {
                return Ok();
            }
        }

        public async Task<IActionResult> Logout()
        {
            var siteContext = _siteContextAccessor.GetContext();
            var logoutUrl = _oidConfig.EndPoints.Logout;

            var endPoint = string.Format(logoutUrl, siteContext.Client.TenantKey);
            var postLogoutRedirectUri = Request.Scheme + "://" + Request.Host + "/";
            endPoint += "?post_logout_redirect_uri=" + System.Uri.EscapeDataString(postLogoutRedirectUri);
            if (User.Identity.IsAuthenticated)
            {
                var authInfo = await HttpContext.Authentication.GetAuthenticateInfoAsync("oidc");
                var tokenId = authInfo.Properties.Items[".Token.id_token"];
                endPoint += "&id_token_hint=" + System.Uri.EscapeDataString(tokenId);
            }

            await HttpContext.Authentication.SignOutAsync("oidc");
            await HttpContext.Authentication.SignOutAsync("cookies");

            return Redirect(endPoint);
        }

        public async Task<IActionResult> LoggedOut()
        {
            await HttpContext.Authentication.SignOutAsync("cookies");
            return Redirect("/");
            //return Redirect("~/public/home");
        }

        [Authorize]
        public IActionResult Register(string ru = null)
        {
            // this method is secured to force the openid middle ware to redirect user to our login provider
            // after the user logs in, they'll be redirected back here then we'll redirect them to their profile
            if (ru != null)
            {
                return Redirect(ru);
            }
            else
            {
                return Redirect("~/admin/user/profile");
            }
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        public IActionResult Notifications()
        {
            var siteContext = _siteContextAccessor.GetContext();

            ViewData["ownerLevel"] = OwnerLevel.Site;
            ViewData["ownerId"] = siteContext.SiteId;


            return this.MasterPageView("~/UI/Views/Admin/Account/Notifications.cshtml", "Notifications");
        }


        [Authorize]
        public async Task<IEnumerable<Models.Tag>> Tags()
        {
            var result = await _tags.GetUserTags(User.GetUserId());
            return result;
        }

        [Authorize]
        public async Task<Models.Tag> AddTag(string tagName)
        {
            return await _tags.AddTag(User.GetUserId(), tagName);
        }

        [Authorize]
        public async Task DeleteTag(string tagName)
        {
            var tag = await _tags.GetByName(User.GetUserId(), tagName);

            if (tag != null)
                await _tags.DeleteTag(tag.Id);
        }



        #region Notifications API

        [HttpPost]
        public JsonResult GetSendToNotificationGroups([DataSourceRequest] DataSourceRequest request, OwnerLevel ownerLevel)
        {
            var userContext = _userContextAccessor.GetContext();
            var groups = GetUserGroupsOfUserContributor(request, UserGroupType.NotificationGroup, userContext, ownerLevel);
            var result = groups.ToDataSourceResult(request);
            return Json(result);
        }

        [HttpPost] 
        public async Task<JsonResult> GetSendToConnectionGroups([DataSourceRequest] DataSourceRequest request)
        {
            var userContext = _userContextAccessor.GetContext();

            IList<Group> userGroups = new List<Group>();
            userGroups.AddRange(await _groupManager.GetGroupsOwnedByUser(userContext.UserId));

            userGroups.AddRange(_groupManager.GetUserMemberships(userContext.UserId));

            var result = userGroups.ToDataSourceResult(request);
            return Json(result);
        }


        #endregion



        private async Task PopulateDocumentsAsync(FolderViewModel model)
        {
            var documents = (await _folderManager.GetDocumentsAsync(await _folderManager.GetFolderAsync(model.Id)))
                .ToArray()  // Can't call ToDocViewModel from within EF
                .Select(x => x.ToDocumentViewModel())
                ;

            model.Documents = model.Documents ?? new List<DocumentViewModel>();
            foreach (var document in documents)
            {
                model.Documents.Add(document);
            }
        }

        private void PopulateChildFolders(FolderViewModel model)
        {
            throw new NotImplementedException();
        }



        private IEnumerable<UserGroup> GetUserGroupsOfUserContributor(DataSourceRequest request, UserGroupType userGroupType, UserContext userContext, OwnerLevel viewLevel)
        {
            var siteContext = _siteContextAccessor.GetContext();
            var accessLevels = new AccessLevel[] { AccessLevel.Contributor, AccessLevel.FullAccess };
            var list = new List<UserGroup>();

            // Get the client and site groups that the user has a membership to.
            var userQuery = _userGroupManager.GetUserGroupsAssignedToUserWithAccessLevelQuery(userContext.UserId, accessLevels, userGroupType);
            var userResults = userQuery.ToDataSourceResult(request);
            list.AddRange(userResults.Data.Cast<UserGroup>());

            // Get the groups that have been created by this user
            var ownerQuery = _userGroupManager.GetUserGroupsOfOwnerAndTypeQuery(OwnerLevel.User, userContext.UserId, userGroupType);
            var ownerResults = ownerQuery.ToDataSourceResult(request);
            list.AddRange(ownerResults.Data.Cast<UserGroup>());

            var corpId = new CorpGlobalClaimValueResolver().Resolve();

            if (viewLevel.Equals(OwnerLevel.Client))
            {
                // Get the client groups if the user has that claim
                if (userContext.SecurityClaims.Find(ClientClaimTypes.AppNotificationsSend, siteContext.Client.Id) ||
                    userContext.SecurityClaims.Find(ClientClaimTypes.AppNotificationsSend, corpId) ||
                    userContext.SecurityClaims.Find(ClientClaimTypes.PrimaryAdmin, siteContext.Client.Id) ||
                    userContext.SecurityClaims.Find(ClientClaimTypes.PrimaryAdmin, corpId) ||
                    userContext.SecurityClaims.Find(CorpClaimTypes.CorpPrimaryAdmin, corpId))
                {
                    var clientQuery = _userGroupManager.GetUserGroupsOfOwnerAndTypeQuery(OwnerLevel.Client, siteContext.Client.Id, userGroupType);
                    var clientResults = clientQuery.ToDataSourceResult(request);
                    list.AddRange(clientResults.Data.Cast<UserGroup>());
                }
            }

            if (viewLevel.Equals(OwnerLevel.Site))
            {
                // Get the site groups if the user has that claim
                if (userContext.SecurityClaims.Find(SiteClaimTypes.SiteNotificationsSend, siteContext.Client.Id) ||
                    userContext.SecurityClaims.Find(SiteClaimTypes.SiteNotificationsSend, siteContext.SiteId) ||
                    userContext.SecurityClaims.Find(SiteClaimTypes.SiteNotificationsSend, corpId) ||
                    userContext.SecurityClaims.Find(SiteClaimTypes.SitePrimaryAdmin, siteContext.Client.Id) ||
                    userContext.SecurityClaims.Find(SiteClaimTypes.SitePrimaryAdmin, siteContext.SiteId) ||
                    userContext.SecurityClaims.Find(SiteClaimTypes.SitePrimaryAdmin, corpId) ||
                    userContext.SecurityClaims.Find(CorpClaimTypes.CorpPrimaryAdmin, corpId))
                {
                    var siteQuery = _userGroupManager.GetUserGroupsOfOwnerAndTypeQuery(OwnerLevel.Site, siteContext.SiteId, userGroupType);
                    var siteResults = siteQuery.ToDataSourceResult(request);
                    list.AddRange(siteResults.Data.Cast<UserGroup>());
                }
            }

            var list2 = list.GroupBy(x => x.Id).Select(x => x.First()); 
            return list2.OrderBy(x => x.Name).ToList();
        }

        private string GetDefaultMasterPageId()
        {
            var siteContext = _siteContextAccessor.GetContext();
            var masterPage = _masterPageManager.GetSiteDefaultAsync(siteContext.SiteId).Result;

            if (masterPage == null)
                throw new NullReferenceException("Could not get default master page for site");

            return masterPage.Id;
        }

    }
}
