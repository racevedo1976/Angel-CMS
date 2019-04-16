using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using Angelo.Connect.UserConsole;
using Angelo.Connect.News.Services;
using Angelo.Connect.News.Security;
using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.News
{
    public class NewsConsole : IUserConsoleCustomComponent
    {
        private NewsManager _newsManager;
        private NewsQueryService _newsQueries;
        private NewsSecurityService _newsSecurity;
        private IContextAccessor<UserContext> _userContextAccessor;

        public int ComponentOrder { get; } = 120;

        public string ComponentType { get; } = "news";

        public string InitialRoute { get; } = "/sys/console/News/posts";

        public NewsConsole
        (
          NewsManager newsManager,
          NewsQueryService newsQueries,
          NewsSecurityService newsSecurity,
          IContextAccessor<UserContext> userContextAccessor
        )
        {
            _newsManager = newsManager;
            _newsQueries = newsQueries;
            _newsSecurity = newsSecurity;
            _userContextAccessor = userContextAccessor;
        }

        public async Task<GenericViewResult> ComposeExplorer()
        {
            var user = _userContextAccessor.GetContext();
            var categories = _newsManager.GetNewsCategoriesOwnedByUser(user.UserId);

            categories = categories.OrderBy(x => x.Title).ToList();

            var result = new GenericViewResult
            {
                Title = "My News",
                ViewPath = "/UI/Views/Console/News/NewsExplorer.cshtml",
                ViewModel = categories
            };

            return await Task.FromResult(result);
        }
    }
}
