using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Configuration;

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public class NavMenuPageProvider : INavMenuContentProvider
    {
        private ConnectDbContext _connectDb;
        private UserContext _userContext;
        private SiteContext _siteContext;
        private PageManager _pageManager;

        public string Name
        {
            get
            {
                return "PageLink";
            }
        }

        public string Title
        {
            get
            {
                return "Page Link";
            }
        }

        public NavMenuPageProvider(ConnectDbContext connectDbContext, UserContext userContext, SiteContext siteContext, PageManager pageManager)
        {
            _connectDb = connectDbContext;
            _userContext = userContext;
            _siteContext = siteContext;
            _pageManager = pageManager;
        }

        public IEnumerable<NavMenuItemContent> GetRootItems(string siteId)
        {
            var items = _connectDb.Pages.AsNoTracking()
                .Where(x =>
                (x.SiteId == siteId) &&
                ((x.ParentPageId == null) || (x.ParentPageId == string.Empty)))
                .Select(page => new NavMenuItemContent()
                {
                    Id = page.Id,
                    ParentId = string.Empty,
                    Title = page.Title,
                    Description = page.Title,
                    HasChildren = (page.ChildPages.Count > 0),
                    Link = page.Path
                })
                .ToList();

            return items;
        }

        public IEnumerable<NavMenuItemContent> GetChildItems(string siteId, string parentContentId)
        {
            var items = _connectDb.Pages.AsNoTracking()
                .Where(x =>
                    (x.SiteId == siteId) && (x.ParentPageId == parentContentId))
                .Include(x => x.ChildPages)
                .Select(page => new NavMenuItemContent()
                {
                    Id = page.Id,
                    ParentId = string.Empty,
                    Title = page.Title,
                    Description = page.Title,
                    HasChildren = (page.ChildPages.Count > 0),
                    Link = page.Path
                })
                .ToList();

            return items;
        }

        public NavMenuItemContent GetContentItem(string contentId)
        {
            var page = _connectDb.Pages.AsNoTracking()
                .Where(x => x.Id == contentId)
                .Include(x => x.ChildPages)
                .FirstOrDefault();

            if (page == null)
                return null;

            var item = new NavMenuItemContent();
            item.ParentId = page.ParentPageId;
            item.Title = page.Title;
            item.Description = page.Title;
            item.HasChildren = (page.ChildPages.Count > 0);
            item.Link = page.Path;
            return item;
        }

        public async Task<bool> Authorize(string contentId)
        {
            var page = await _pageManager.GetByIdAsync(contentId);
            bool result = false;
            var isPagePrivate = false;

            if (page == null)
                result = false;
            else
                isPagePrivate = page.IsPrivate;

            if (isPagePrivate)
                result =  _userContext.SecurityClaims.FindAny(_validViewClaims(contentId));
            else
                result = true;

            return await Task.FromResult(result);
        }

        private SecurityClaim[] _validViewClaims(string pageId)
        {
            return new[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _siteContext.Client.Id),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, _siteContext.SiteId),
                new SecurityClaim(PageClaimTypes.ViewPrivatePage, pageId),
            };
        }
    }
}


