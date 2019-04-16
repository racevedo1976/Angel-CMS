using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Angelo.Connect.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Angelo.Identity;
using Angelo.Identity.Services;
using Angelo.Common.Messaging;
using Microsoft.Extensions.Options;

namespace Angelo.Connect.Web.UI.Controllers
{
    public class UsersDataController : BaseController
    {
        private UserManager _userManager;
        private SiteManager _siteManager;
        private UserProfileManager _profileManager;
        private ClientManager _clientManager;
        private Identity.SecurityPoolManager _poolManager;
        private DirectoryManager _directoryManager;
        private TemplateEmailService _messaging;
        private AegisOptions _openIdOptions;

        public UsersDataController(
            SiteManager siteManager, 
            ClientManager clientManager, 
            UserProfileManager profileManager,
            UserManager userManager,
            Identity.SecurityPoolManager poolManager,
            ILogger<UsersDataController> logger,
            DirectoryManager directoryManager,
            TemplateEmailService messaging,
            IOptions<AegisOptions> openIdOptions

        ) : base(logger)
        {
            _siteManager = siteManager;
            _clientManager = clientManager;
            _profileManager = profileManager;
            _userManager = userManager;
            _poolManager = poolManager;
            _directoryManager = directoryManager;
            _messaging = messaging;
            _openIdOptions = openIdOptions.Value;
        }

        //TODO: Find out where these are being used and move

        //NOTE: All other user related api methods have been moved to respective ClientLevel & SiteLevel data controllers

        [Authorize]
        [HttpPost, Route("/api/pools/users/selectlist")]
        public ActionResult GetUsersSelectList([DataSourceRequest]DataSourceRequest request, string poolId)
        {
            var query = _poolManager.GetUsersQuery(poolId);
            var result = query.ToDataSourceResult(request);
            return Json(result);
        }

        [Authorize]
        [HttpPost, Route("/api/pools/roles/selectlist")]
        public ActionResult GetRolesSelectList([DataSourceRequest]DataSourceRequest request, string poolId)
        {
            var query = _poolManager.GetRolesQuery(poolId);
            var result = query.ToDataSourceResult(request);
            return Json(result);
        }

    }
}
