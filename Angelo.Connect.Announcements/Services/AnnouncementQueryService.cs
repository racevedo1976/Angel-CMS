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
using Angelo.Connect.Announcement.Data;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Announcement.Services;
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.Models;

namespace Angelo.Connect.Announcement.Services
{
    public class AnnouncementQueryService 
    {
        private AnnouncementDbContext _announcementDbContext;
        private AnnouncementSecurityService _announcementSecurity;
        private ConnectDbContext _connectDbContext;
        private SiteContext _siteContext;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementQueryService(SiteContext siteContext, AnnouncementSecurityService announcementSecurity, ConnectDbContext connectDbContext, AnnouncementDbContext announcementDbContext, IContextAccessor<UserContext> userContextAccessor)
        {
            _siteContext = siteContext;
            _connectDbContext = connectDbContext;
            _announcementDbContext = announcementDbContext;
            _announcementSecurity = announcementSecurity;
            _userContextAccessor = userContextAccessor;
        }

        public IQueryable<AnnouncementPost> QueryByWidget(string widgetId)
        {
            // get announcement posts by category or tag matching widget categories & tags
            var widgetCats = _announcementDbContext.AnnouncementWidgetCategories.Where(x => x.WidgetId == widgetId);
            var widgetTags = _announcementDbContext.AnnouncementWidgetTags.Where(x => x.WidgetId == widgetId);

            var announcementPosts = _announcementDbContext.AnnouncementPosts
                .Where(x =>
                    x.Tags.Any(y => widgetTags.Any(z => z.TagId == y.TagId))
                    || x.Categories.Any(y => widgetCats.Any(z => z.CategoryId == y.CategoryId))
                );

            // filter to active & published posts
            announcementPosts = announcementPosts.Where(x => x.IsActive && x.Published);


            // if administrator, then no need to filter out private posts
            if(_announcementSecurity.AuthorizeForReadAdmin())
            {
                return announcementPosts;
            }

            // otherwise filter out private announcement posts that user doesn't have access to   
            var user = _userContextAccessor.GetContext();

            string[] validAnnouncementPostIds = user.SecurityClaims
                .Where(x => x.Type == AnnouncementClaimTypes.AnnouncementPostRead)
                .Select(x => x.Value)
                .ToArray();

            // eg, determine posts that are marked as private that the user did not author and do not have explicit permissions to view
            var unauthorizedPosts = announcementPosts.Where(x => x.IsPrivate && x.UserId != user.UserId && !validAnnouncementPostIds.Contains(x.Id));


            return announcementPosts.Where(x => !unauthorizedPosts.Any(y => y.Id == x.Id)).AsQueryable();
        }

        public IQueryable<AnnouncementPost> QueryByAuthor(string userId)
        {
            return _announcementDbContext.AnnouncementPosts
                .Where(x => x.UserId == userId && x.Status != 0)
                .OrderByDescending(x => x.Posted).AsQueryable();
        }

        public IQueryable<AnnouncementPost> QueryByAuthor(string userId, string categoryId)
        {
            return _announcementDbContext.AnnouncementPosts
                .Where(x => x.UserId == userId && x.Categories.Any(y => y.CategoryId == categoryId))
                .OrderByDescending(x => x.Posted).AsQueryable();
        }

        public IQueryable<AnnouncementCategory> QueryCategoriesOwnedByUser(UserContext user, string text = null)
        {
            var query = _announcementDbContext.AnnouncementCategories.Where(x => x.UserId == user.UserId && x.IsActive);

            if (text != null)
                query = query.Where(x => x.Title.StartsWith(text, StringComparison.OrdinalIgnoreCase));

            return query.OrderBy(x => x.Title);
        }

        public IQueryable<AnnouncementCategory> QueryCategoriesSharedWithUser(UserContext user, string text = null)
        {
            var sharedCategoryIds = user.SecurityClaims
                .Where(x => x.Type == AnnouncementClaimTypes.AnnouncementCategoryContribute)
                .Select(x => x.Value);

            var query = _announcementDbContext.AnnouncementCategories.Where(x => sharedCategoryIds.Contains(x.Id) && x.IsActive);

            if (text != null)
                query = query.Where(x => x.Title.StartsWith(text, StringComparison.OrdinalIgnoreCase));

            return query.OrderBy(x => x.Title);
        }
    }
}
