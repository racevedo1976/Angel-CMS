using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Components.CorpLevel
{
    public class CorpLdapClients : ViewComponent
    {
        public CorpLdapClients()
        {

        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
