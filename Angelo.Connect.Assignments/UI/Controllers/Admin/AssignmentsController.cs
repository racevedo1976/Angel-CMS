using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Assignments.UI.Controllers.Admin
{
    [Authorize]
    public class AssignmentsController : Controller
    {
        protected SiteContext _siteContext;

        public AssignmentsController(SiteContext siteContext, ILogger<ClientManager> logger)
        {
            _siteContext = siteContext;

        }

        public async Task<ActionResult> UserAssignments()
        {
            var userId = User.GetUserId();
            var siteId = _siteContext.SiteId;

            return await Index(OwnerLevel.User, userId);
        }

        public async Task<ActionResult> Index(OwnerLevel ownerLevel, string ownerId)
        {
            ViewData["ownerLevel"] = ownerLevel;
            ViewData["ownerId"] = ownerId;

            return View("Index");
        }
    }
}
