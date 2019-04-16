using System;
using System.Collections.Generic;
using System.Linq;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Services;
using Angelo.Connect.Widgets;
using Angelo.Connect.NavMenu.Models;
using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.NavMenu.Services
{
    public class AutoNavViewService : IWidgetService
    {
        private ConnectDbContext _connectDb;
        private PageManager _pageManager;
        private IContextAccessor<SiteContext> _siteContextAccessor;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AutoNavViewService
        (
            ConnectDbContext connectDb, 
            PageManager pageManager,
            IContextAccessor<SiteContext> siteContextAccessor,
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _connectDb = connectDb;
            _pageManager = pageManager;
            _siteContextAccessor = siteContextAccessor;
            _userContextAccessor = userContextAccessor;            
        }


        public IEnumerable<Page> GetChildPages(string pageId)
        {
            var userContext = _userContextAccessor.GetContext();
            var siteContext = _siteContextAccessor.GetContext();

            var childPages = GetChildPagesAuthorizedForView(pageId, userContext);

            return childPages.OrderBy(x => x.Title);
        }

        public Page GetPage(string pageId)
        {
            return _pageManager.GetByIdAsync(pageId).Result;
        }


        public string GetParentPageId(string pageId)
        {
            var page = _pageManager.GetByIdAsync(pageId).Result;

            return page?.ParentPageId;
        }


        private IEnumerable<Page> GetChildPagesAuthorizedForView(string pageId, UserContext userContext)
        {
            //TODO: Move to PageManager
            var parentPage = _pageManager.GetByIdAsync(pageId).Result;
            var parentSite = _connectDb.Sites.FirstOrDefault(x => x.Id == parentPage.SiteId);
            var childPages = _connectDb.Pages.Where(x => x.ParentPageId == pageId).ToList();

            var authorizedPrivatePageIds = userContext.SecurityClaims
                   .Where(x => x.Type == PageClaimTypes.ViewPrivatePage)
                   .Select(x => x.Value);

            return childPages.Where(page =>
                page.IsPrivate == false || IsPageAdmin(userContext, page) || authorizedPrivatePageIds.Contains(page.Id)
            );
        }

     
        private bool IsPageAdmin(UserContext userContext, Page page)
        {
            if (userContext == null || userContext.IsAuthenticated == false || userContext.SecurityClaims == null || page == null)
                return false;

            var site = page.Site;
            var client = page.Site?.Client;

            if (site == null)
                site = _connectDb.Sites.FirstOrDefault(x => x.Id == page.SiteId);

            if (client == null)
                client = _connectDb.Clients.FirstOrDefault(x => x.Id == site.ClientId);

            if (IsSiteAdmin(userContext, site))
                return true;

            // else 
            return userContext.SecurityClaims.Any(x =>
              (x.Type == SiteClaimTypes.SitePagesPublish && x.Value == site.Id)
              || (x.Type == SiteClaimTypes.SitePagesCreate && x.Value == site.Id)
              || (x.Type == SiteClaimTypes.SitePagesEdit && x.Value == site.Id)
              || (x.Type == SiteClaimTypes.SitePagesDelete && x.Value == site.Id)
              || (x.Type == PageClaimTypes.PageOwner && x.Value == page.Id)
           );


        }

        private bool IsSiteAdmin(UserContext userContext, Site site)
        {
            if (userContext == null || userContext.SecurityClaims == null || userContext.IsAuthenticated == false || site == null)
                return false;

            return userContext.SecurityClaims.Any(x =>
               (x.Type == SiteClaimTypes.SitePrimaryAdmin && x.Value == site.Id)
               || (x.Type == SiteClaimTypes.SitePrimaryAdmin && x.Value == site.ClientId)
               || (x.Type == SiteClaimTypes.SitePrimaryAdmin && x.Value == ConnectCoreConstants.CorporateId)
               || (x.Type == ClientClaimTypes.PrimaryAdmin && x.Value == site.ClientId)
               || (x.Type == ClientClaimTypes.PrimaryAdmin && x.Value == ConnectCoreConstants.CorporateId)
            );
        }

        private bool IsClientAdmin(UserContext userContext, Client client)
        {
            if (userContext == null || userContext.SecurityClaims == null || userContext.IsAuthenticated == false || client == null)
                return false;

            return userContext.SecurityClaims.Any(x =>
               (x.Type == ClientClaimTypes.PrimaryAdmin && x.Value == client.Id)
               || (x.Type == ClientClaimTypes.PrimaryAdmin && x.Value == ConnectCoreConstants.CorporateId)
            );
        }

    }
}
