using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.UserConsole;
using Angelo.Connect.UI.ViewModels;
using Angelo.Connect.Web.Data.Mock;

namespace Angelo.Connect.Web.UI.UserConsole.Components
{
    public class UserMessageComponent : IUserConsoleCustomComponent
    {
        public string ComponentType { get; } = "messages";

        public int ComponentOrder { get; } = 0;

        public string InitialRoute { get; } = "/sys/uc/messages";

        public UserMessageComponent()
        {

        }

        public async Task<GenericViewResult> ComposeExplorer()
        {
            var view = new GenericViewResult
            {
                Title = "Messages",
                ViewPath = "~/UI/UserConsole/Views/Messages/Summary.cshtml",
                ViewModel = MockData.MessageCategories
            };

            return await Task.FromResult(view);
        }

    }

}
