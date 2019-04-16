using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Blog.Data;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.Services;
using Angelo.Connect.Blog.Security;
using Angelo.Connect.Models;

namespace Angelo.Connect.Blog.Services
{
    public class BlogQueryService 
    {
        private BlogDbContext _blogDbContext;
        private BlogSecurityService _blogSecurity;
        private ConnectDbContext _connectDbContext;
        private SiteContext _siteContext;
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogQueryService(SiteContext siteContext, BlogSecurityService blogSecurity, ConnectDbContext connectDbContext, BlogDbContext blogDbContext, IContextAccessor<UserContext> userContextAccessor)
        {
            _siteContext = siteContext;
            _connectDbContext = connectDbContext;
            _blogDbContext = blogDbContext;
            _blogSecurity = blogSecurity;
            _userContextAccessor = userContextAccessor;
        }

        public IQueryable<BlogPost> QueryByWidget(string widgetId)
        {
            // get blog posts by category or tag matching widget categories & tags
            var widgetCats = _blogDbContext.BlogWidgetCategories.Where(x => x.WidgetId == widgetId);
            var widgetTags = _blogDbContext.BlogWidgetTags.Where(x => x.WidgetId == widgetId);

            var blogPosts = _blogDbContext.BlogPosts
                .Where(x =>
                    x.Tags.Any(y => widgetTags.Any(z => z.TagId == y.TagId))
                    || x.Categories.Any(y => widgetCats.Any(z => z.CategoryId == y.CategoryId))
                );

            // filter to active & published posts
            blogPosts = blogPosts.Where(x => x.IsActive && x.Published);


            // if administrator, then no need to filter out private posts
            if(_blogSecurity.AuthorizeForReadAdmin())
            {
                return blogPosts;
            }

            // otherwise filter out private blog posts that user doesn't have access to   
            var user = _userContextAccessor.GetContext();

            string[] validBlogPostIds = user.SecurityClaims
                .Where(x => x.Type == BlogClaimTypes.BlogPostRead)
                .Select(x => x.Value)
                .ToArray();

            // eg, determine posts that are marked as private that the user did not author and do not have explicit permissions to view
            var unauthorizedPosts = blogPosts.Where(x => x.IsPrivate && x.UserId != user.UserId && !validBlogPostIds.Contains(x.Id));


            return blogPosts.Where(x => !unauthorizedPosts.Any(y => y.Id == x.Id)).AsQueryable();
        }

        public IQueryable<BlogPost> QueryByAuthor(string userId)
        {
            return _blogDbContext.BlogPosts
                .Where(x => x.UserId == userId && x.Status != 0)
                .OrderByDescending(x => x.Posted).AsQueryable();
        }

        public IQueryable<BlogPost> QueryByAuthor(string userId, string categoryId)
        {
            return _blogDbContext.BlogPosts
                .Where(x => x.UserId == userId && x.Categories.Any(y => y.CategoryId == categoryId))
                .OrderByDescending(x => x.Posted).AsQueryable();
        }

        public IQueryable<BlogCategory> QueryCategoriesOwnedByUser(UserContext user, string text = null)
        {
            var query = _blogDbContext.BlogCategories.Where(x => x.UserId == user.UserId && x.IsActive);

            if (text != null)
                query = query.Where(x => x.Title.StartsWith(text, StringComparison.OrdinalIgnoreCase));

            return query.OrderBy(x => x.Title);
        }

        public IQueryable<BlogCategory> QueryCategoriesSharedWithUser(UserContext user, string text = null)
        {
            var sharedCategoryIds = user.SecurityClaims
                .Where(x => x.Type == BlogClaimTypes.BlogCategoryContribute)
                .Select(x => x.Value);

            var query = _blogDbContext.BlogCategories.Where(x => sharedCategoryIds.Contains(x.Id) && x.IsActive);

            if (text != null)
                query = query.Where(x => x.Title.StartsWith(text, StringComparison.OrdinalIgnoreCase));

            return query.OrderBy(x => x.Title);
        }
    }
}
