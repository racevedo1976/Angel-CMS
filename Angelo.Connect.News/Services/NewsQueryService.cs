using System;
using System.Linq;

using Angelo.Connect.Security;
using Angelo.Connect.Abstractions;
using Angelo.Connect.News.Data;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.Security;

namespace Angelo.Connect.News.Services
{
    public class NewsQueryService 
    {
        private readonly NewsDbContext _newsDbContext;
        private readonly NewsSecurityService _newsSecurity;
      
        private readonly IContextAccessor<UserContext> _userContextAccessor;

        public NewsQueryService(NewsSecurityService newsSecurity, NewsDbContext newsDbContext, IContextAccessor<UserContext> userContextAccessor)
        {

            _newsDbContext = newsDbContext;
            _newsSecurity = newsSecurity;
            _userContextAccessor = userContextAccessor;
        }

        public IQueryable<NewsPost> QueryByWidget(string widgetId)
        {
            // get News posts by category or tag matching widget categories & tags
            var widgetCats = _newsDbContext.NewsWidgetCategories.Where(x => x.WidgetId == widgetId);
            var widgetTags = _newsDbContext.NewsWidgetTags.Where(x => x.WidgetId == widgetId);

            var newsPosts = _newsDbContext.NewsPosts
                .Where(x =>
                    x.Tags.Any(y => widgetTags.Any(z => z.TagId == y.TagId))
                    || x.Categories.Any(y => widgetCats.Any(z => z.CategoryId == y.CategoryId))
                );

            // filter to active & published posts
            newsPosts = newsPosts.Where(x => x.IsActive && x.Published);


            // if administrator, then no need to filter out private posts
            if(_newsSecurity.AuthorizeForReadAdmin())
            {
                return newsPosts;
            }

            // otherwise filter out private News posts that user doesn't have access to   
            var user = _userContextAccessor.GetContext();

            string[] validNewsPostIds = user.SecurityClaims
                .Where(x => x.Type == NewsClaimTypes.NewsPostRead)
                .Select(x => x.Value)
                .ToArray();

            // eg, determine posts that are marked as private that the user did not author and do not have explicit permissions to view
            var unauthorizedPosts = newsPosts.Where(x => x.IsPrivate && x.UserId != user.UserId && !validNewsPostIds.Contains(x.Id));


            return newsPosts.Where(x => !unauthorizedPosts.Any(y => y.Id == x.Id)).AsQueryable();
        }

        public IQueryable<NewsPost> QueryByAuthor(string userId)
        {
            return _newsDbContext.NewsPosts
                .Where(x => x.UserId == userId && x.Status != 0)
                .OrderByDescending(x => x.Posted).AsQueryable();
        }

        public IQueryable<NewsPost> QueryByAuthor(string userId, string categoryId)
        {
            return _newsDbContext.NewsPosts
                .Where(x => x.UserId == userId && x.Categories.Any(y => y.CategoryId == categoryId))
                .OrderByDescending(x => x.Posted).AsQueryable();
        }

        public IQueryable<NewsCategory> QueryCategoriesOwnedByUser(UserContext user, string text = null)
        {
            var query = _newsDbContext.NewsCategories.Where(x => x.UserId == user.UserId && x.IsActive);

            if (text != null)
                query = query.Where(x => x.Title.StartsWith(text, StringComparison.OrdinalIgnoreCase));

            return query.OrderBy(x => x.Title);
        }

        public IQueryable<NewsCategory> QueryCategoriesSharedWithUser(UserContext user, string text = null)
        {
            var sharedCategoryIds = user.SecurityClaims
                .Where(x => x.Type == NewsClaimTypes.NewsCategoryContribute)
                .Select(x => x.Value);

            var query = _newsDbContext.NewsCategories.Where(x => sharedCategoryIds.Contains(x.Id) && x.IsActive);

            if (text != null)
                query = query.Where(x => x.Title.StartsWith(text, StringComparison.OrdinalIgnoreCase));

            return query.OrderBy(x => x.Title);
        }
    }
}
