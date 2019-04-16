using Angelo.Connect.UserConsole;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.Web.UI.UserConsole.Components
{
    public class SiteAlertsComponents : IUserConsoleCustomComponent
    {
        public int ComponentOrder { get; } = 250;
        public string ComponentType { get; } = "alerts";

        public string InitialRoute { get; } = "/sys/console/sitealerts/list";

        public string TreeTitle { get; } = "My Site Alerts";

        public async Task<GenericViewResult> ComposeExplorer()
        {
            var result = new GenericViewResult
            {
                Title = "My Alerts",
                ViewPath = "/UI/Views/Console/Alerts/AlertExplorer.cshtml",
                ViewModel = null
            };

            return await Task.FromResult(result);
        }
    }
}
