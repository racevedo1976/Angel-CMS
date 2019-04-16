using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components.CorpLevel
{
    public class CorpLdapClientRolesHierarchy : ViewComponent
    {
        public CorpLdapClientRolesHierarchy()
        {

        }

        public async Task<IViewComponentResult> InvokeAsync(string clientId = null)
        {
            ViewData["clientId"] = clientId;
            return View();
        }
    }
}
