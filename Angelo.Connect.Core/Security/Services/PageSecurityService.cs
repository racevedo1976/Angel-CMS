using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Services;

namespace Angelo.Connect.Security.Services
{
    public class PageSecurityService 
    {
        private SiteManager _siteManager;
        private PageManager _pageManager;

        public PageSecurityService(SiteManager siteManager, PageManager pageManager)
        {
            _siteManager = siteManager;
            _pageManager = pageManager;
        }

        public bool CanDesignPage(UserContext user, Page page)
        {
            var site = _siteManager.GetByIdAsync(page.SiteId).Result;

            var validClaims =  new SecurityClaim[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id),
                new SecurityClaim(SiteClaimTypes.SitePagesDesign, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePagesDesign, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePagesDesign, site.Id),
                new SecurityClaim(PageClaimTypes.PageOwner, page.Id),
                new SecurityClaim(PageClaimTypes.DesignPage, page.Id),
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public bool CanViewPage(UserContext user, Page page)
        {
            if (!page.IsPrivate)
                return true;

            var site = _siteManager.GetByIdAsync(page.SiteId).Result;

            var validClaims = new[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id),
                new SecurityClaim(PageClaimTypes.PageOwner, page.Id),
                new SecurityClaim(PageClaimTypes.ViewPrivatePage, page.Id),
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public bool CanPublishPage(UserContext user, Page page)
        {
            var site = _siteManager.GetByIdAsync(page.SiteId).Result;

            var validClaims = new SecurityClaim[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id),
                new SecurityClaim(SiteClaimTypes.SitePagesPublish, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePagesPublish, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePagesPublish, site.Id),
                new SecurityClaim(PageClaimTypes.PublishPage, page.Id),
                new SecurityClaim(PageClaimTypes.PageOwner, page.Id),
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public bool CanDeletePage(UserContext user, Page page)
        {
            var site = _siteManager.GetByIdAsync(page.SiteId).Result;

            var validClaims = new SecurityClaim[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id),
                new SecurityClaim(SiteClaimTypes.SitePagesDelete, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePagesDelete, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePagesDelete, site.Id),
                new SecurityClaim(PageClaimTypes.PageOwner, page.Id),
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public bool CanEditPage(UserContext user, Page page)
        {
            var site = _siteManager.GetByIdAsync(page.SiteId).Result;

            var validClaims = new SecurityClaim[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id),
                new SecurityClaim(SiteClaimTypes.SitePagesEdit, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePagesEdit, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePagesEdit, site.Id),
                new SecurityClaim(PageClaimTypes.PageOwner, page.Id),
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public bool CanCreatePage(UserContext user, Site site)
        {
            var validClaims = new SecurityClaim[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id),
                new SecurityClaim(SiteClaimTypes.SitePagesCreate, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePagesCreate, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePagesCreate, site.Id)
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public bool CanCreateChildPage(UserContext user, Page parentPage)
        {
            var site = _siteManager.GetByIdAsync(parentPage.SiteId).Result;

            var validClaims = new SecurityClaim[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id),
                new SecurityClaim(SiteClaimTypes.SitePagesCreate, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePagesCreate, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePagesCreate, site.Id),
                new SecurityClaim(PageClaimTypes.PageOwner, parentPage.Id),
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public bool CanMovePage(UserContext user, Page page, Page parentPage = null)
        {
            var site = _siteManager.GetByIdAsync(page.SiteId).Result;

            // these are the valid claims when the parentPage is null
            var validParentClaims = new List<SecurityClaim> {
                    new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                    new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                    new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id)
            };

            if(parentPage != null)
            {
                // allow page owners to move pages around in their section
                validParentClaims.Add(
                    new SecurityClaim(PageClaimTypes.PageOwner, parentPage.Id)
                );
            }

            var validChildClaims = new SecurityClaim[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.ClientId),
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, site.Id),
                new SecurityClaim(PageClaimTypes.PageOwner, page.Id),
            };


            return user.SecurityClaims.FindAny(validParentClaims)
                && user.SecurityClaims.FindAny(validChildClaims);
        }

        public bool CanDesignMaster(UserContext user, PageMaster master)
        {
            var site = _siteManager.GetByIdAsync(master.SiteId).Result;

            var validClaims = new SecurityClaim[]
            {
                // HOTFIX: Only corporate users can perform this activity
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SiteMasterPagesDesign, ConnectCoreConstants.CorporateId),
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public bool CanPublishMaster(UserContext user, PageMaster master)
        {
            var site = _siteManager.GetByIdAsync(master.SiteId).Result;

            // HOTFIX: Only corporate users can perform this activity
            var validClaims = new SecurityClaim[]
            {
                new SecurityClaim(SiteClaimTypes.SitePrimaryAdmin, ConnectCoreConstants.CorporateId),
                new SecurityClaim(SiteClaimTypes.SiteMasterPagesPublish, ConnectCoreConstants.CorporateId),
            };

            return user.SecurityClaims.FindAny(validClaims);
        }

        public async Task<IEnumerable<Page>> GetSitePagesOwnedByUser(UserContext user, string siteId)
        {
            var sitePages = await _pageManager.GetPagesAsync(siteId);

            var userPageIds = user.SecurityClaims
                .Where(x => x.Type == Security.KnownClaims.PageClaimTypes.PageOwner)
                .Select(x => x.Value)
                .ToList();

            // all applicable user pages for this site
            return sitePages.Where(x => userPageIds.Contains(x.Id));
        }
    }
}
