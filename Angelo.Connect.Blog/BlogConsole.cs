using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using Angelo.Connect.UserConsole;
using Angelo.Connect.Blog.Services;
using Angelo.Connect.Blog.Security;
using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.Blog
{
    public class BlogConsole : IUserConsoleCustomComponent
    {
        private BlogManager _blogManager;
        private BlogQueryService _blogQueries;
        private BlogSecurityService _blogSecurity;
        private IContextAccessor<UserContext> _userContextAccessor;

        public int ComponentOrder { get; } = 120;

        public string ComponentType { get; } = "blog";

        public string InitialRoute { get; } = "/sys/console/blog/posts";

        public BlogConsole
        (
          BlogManager blogManager,
          BlogQueryService blogQueries,
          BlogSecurityService blogSecurity,
          IContextAccessor<UserContext> userContextAccessor
        )
        {
            _blogManager = blogManager;
            _blogQueries = blogQueries;
            _blogSecurity = blogSecurity;
            _userContextAccessor = userContextAccessor;
        }

        public async Task<GenericViewResult> ComposeExplorer()
        {
            var user = _userContextAccessor.GetContext();
            var categories = _blogManager.GetBlogCategoriesOwnedByUser(user.UserId);

            categories = categories.OrderBy(x => x.Title).ToList();

            var result = new GenericViewResult
            {
                Title = "My Blogs",
                ViewPath = "/UI/Views/Console/BlogExplorer.cshtml",
                ViewModel = categories
            };

            return await Task.FromResult(result);
        }
    }
}
