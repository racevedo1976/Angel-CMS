using Angelo.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components.CorpLevel
{
    public class CorpLdapSecurityPoolMap : ViewComponent
    {
        SecurityPoolManager _poolManager;
        public CorpLdapSecurityPoolMap(SecurityPoolManager poolManager)
        {
            _poolManager = poolManager;
        }
        public async Task<IViewComponentResult> InvokeAsync(string id = null)
        {
            var poolObject = new Identity.Models.SecurityPool();

            if (!string.IsNullOrEmpty(id))
            {
                var objectId = id.Split('_');
                var resourceType = objectId[0];
                var poolId = objectId[1];
                var directoryId = objectId[2];

                ViewData["directoryId"] = directoryId;
                poolObject = await _poolManager.GetByIdAsync(poolId);
            }
            
            
            return View(poolObject);
        }
    }
}
