using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Identity;

namespace Angelo.Connect.Services
{
    public class PageManager
    {
        private string _pageContentType = typeof(Page).FullName;

        private ConnectDbContext _db;
        private ContentManager _contentManager;
        private UserManager _userManager;

        public PageManager(ConnectDbContext dbContext, ContentManager contentManger, UserManager userManager)
        {
            _db = dbContext;
            _contentManager = contentManger;
            _userManager = userManager;
        }

        public async Task<ICollection<Page>> GetPagesAsync(string siteId)
        {
            return await _db.Pages
                        .Include(x => x.MasterPage)
                        .Where(x => x.Site.Id == siteId)
                        .OrderBy(x => x.Title)
                        .ToListAsync();
        }


        public async Task<ICollection<Page>> GetByAuthorAsync(string siteId, string userId)
        {
            return await _db.Pages
                        .Include(x => x.MasterPage)
                        .Where(x => x.SiteId == x.SiteId && x.UserId == userId)
                        .OrderBy(x => x.Title)
                        .ToListAsync();
        }

        public async Task<ICollection<Page>> GetByAuthorAsync(string userId)
        {
            // regardless of site
            return await _db.Pages
                        .Include(x => x.MasterPage)
                        .Where(x => x.UserId == userId)
                        .OrderBy(x => x.Title)
                        .ToListAsync();
        }

        public async Task<Page> GetByIdAsync(string id)
        {
            Ensure.Argument.NotNullOrEmpty(id);

            return await _db.Pages
                         .Where(x => x.Id == id)
                         .FirstOrDefaultAsync();
        }

        public async Task<Page> GetSiteByPageIdAsync(string id)
        {
            Ensure.Argument.NotNullOrEmpty(id);

            return await _db.Pages
                         .Where(x => x.Id == id)
                         .Include(x => x.Site)
                         .FirstOrDefaultAsync();
        }

        public async Task<Page> GetByRouteAsync(string siteId, string route)
        {
            if (route == null || route.Trim() == "")
            {
                route = "/";
            }

            var page = await _db.Pages.FirstOrDefaultAsync(x => x.SiteId == siteId && x.Path == route);

            // only do this check if we are searching for the home page.
            // Most likely the home page was renamed by user and no longer a "/"
            // if not looking for home page then return default. It will be a 404.
            if ((page == null) && (route == "/"))
            {
                return await _db.Pages.FirstOrDefaultAsync(x => x.SiteId == siteId && x.IsHomePage);
            }

            return page;
        }

        public async Task<bool> UpdateAsync(Page page)
        {
            Ensure.Argument.NotNull(page);

            var tempPage = _db.Pages.FirstOrDefault(x => x.Id == page.Id);

            if (tempPage == null)
            {
                return false;
            }


            tempPage.Title = page.Title;
            tempPage.Keywords = page.Keywords;
            tempPage.Summary = page.Summary;
            tempPage.Path = CleanRoute(page.Path);
            tempPage.PageMasterId = page.PageMasterId;
            tempPage.IsPrivate = page.IsPrivate;
            tempPage.ParentPageId = page.ParentPageId;

            // Trimming strings
            if (!string.IsNullOrEmpty(tempPage.Title))
                tempPage.Title = tempPage.Title.Trim();

            if (!string.IsNullOrEmpty(tempPage.Keywords))
                tempPage.Keywords = tempPage.Keywords.Trim();

            if (!string.IsNullOrEmpty(tempPage.Summary))
                tempPage.Summary = tempPage.Summary.Trim();

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateParentAsync(string pageId, string parentPageId)
        {
            Ensure.Argument.NotNull(pageId);

            var page = _db.Pages.FirstOrDefault(x => x.Id == pageId);

            if (page == null )
                return false;

            if (parentPageId != null)
            {
                var parentPage = await _db.Pages.FirstOrDefaultAsync(x => x.Id == parentPageId);

                if (parentPage == null || parentPage.Id == page.Id)
                    return false;
            }
                      

            page.ParentPageId = parentPageId;
           
            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> CreateAsync(Page page, string userId)
        {
            Ensure.Argument.NotNull(page);

            try
            {
                page.Id = KeyGen.NewGuid();
                page.Path = CleanRoute(page.Path);
                page.UserId = userId;

                if(!string.IsNullOrEmpty(page.Title))
                    page.Title = page.Title.Trim();

                if (!string.IsNullOrEmpty(page.Keywords))
                    page.Keywords = page.Keywords.Trim();

                if (!string.IsNullOrEmpty(page.Summary))
                    page.Summary = page.Summary.Trim();

                _db.Pages.Add(page);
                   
                await _db.SaveChangesAsync();

                //TODO: Transaction to ensure data consistency   
                await _userManager.AddClaimAsync(userId, new Claim(PageClaimTypes.PageOwner, page.Id));
                await CreateDraftVersion(page.Id, userId, "Initial Empty Draft Version");

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
   
        public async Task<bool> RemoveAsync(string pageId)
        {
            Ensure.Argument.NotNull(pageId);

            var page = await _db.Pages.FirstOrDefaultAsync(x => x.Id == pageId);

            if (page.IsHomePage)
            {
                throw new NullReferenceException($"Cannot delete this page. It is the default home page.");
            }

            var childPageCount = await _db.Pages.Where(x => x.ParentPageId == pageId).CountAsync();

            if(childPageCount > 0)
            {
                throw new InvalidOperationException($"Cannot delete this page. It has {childPageCount} child pages.");
            }

            if(page != null)
            {
                _db.Pages.Remove(page);

                await _db.SaveChangesAsync();
                await _contentManager.DeleteAllVersions(_pageContentType, pageId);
                await _contentManager.DeleteAllContentTrees(_pageContentType, pageId);
                return true;
            }

            throw new NullReferenceException($"Cannot delete this page. Page {pageId} does not exist.");
        }

        public async Task<string> GetDefaultDomain(string siteId)
        {           
            var domain = await _db.SiteDomains.FirstOrDefaultAsync(x => x.SiteId == siteId && x.IsDefault == true);

            return domain?.DomainKey;
        }

        public async Task<bool> UpdatePageMasterAsync(string id, string pageMasterId)
        {
            Ensure.Argument.NotNull(id);

            var tempPage = _db.Pages.FirstOrDefault(x => x.Id == id);

            if (tempPage == null)
            {
                return false;
            }

            tempPage.PageMasterId = pageMasterId;

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<ContentVersion> GetPublishedVersion(string pageId)
        {
            return await _contentManager.GetPublishedVersionInfo(_pageContentType, pageId);
        }

        public async Task<ContentVersion> GetLatestDraftVersion(string pageId)
        {
            var drafts = await _contentManager.GetVersionHistory(_pageContentType, pageId, ContentStatus.Draft);

            return drafts.OrderByDescending(x => x.Created).FirstOrDefault();
        }

        public async Task<ContentVersion> GetVersion(string pageId, string versionCode)
        {
            return await _contentManager.GetVersionInfo(_pageContentType, pageId, versionCode);
        }

        public async Task<IEnumerable<ContentVersion>> GetVersions(string pageId)
        {
            // TODO: Decouple version information from content collections
            return await _contentManager.GetVersionHistory(_pageContentType, pageId);
        }

        public async Task<ContentVersion> CreateInitialVersion(Page page, IEnumerable<string> seedFiles = null)
        {
            var initialVersion = new ContentVersion
            {
                ContentId = page.Id,
                ContentType = _pageContentType,
                VersionLabel = "Initial Version",
                Status = ContentStatus.Published,
                UserId = "system",
            };

            var contentTree = new ContentTree(initialVersion);

            _db.ContentVersions.Add(initialVersion);
            _db.ContentTrees.Add(contentTree);

            await _db.SaveChangesAsync();

            if (seedFiles != null && seedFiles.Count() > 0)
            {
                var treeBuilder = _contentManager.CreateTreeBuilder(contentTree);

                foreach (var filePath in seedFiles)
                    treeBuilder.SeedFromFile(filePath);

                treeBuilder.SaveChanges();
            }

            return initialVersion;
        }

        public async Task<ContentVersion> CreateDraftVersion(string pageId, string userId, string versionLabel, string fromVersionCode = null)
        {
            // TODO: Fix using user name instead of ID so won't have to lookup in disconnected identity db context. 
            var draftVersion = await _contentManager.CreateDraftVersion(_pageContentType, pageId, userId, versionLabel);

            var sourceVersion = (fromVersionCode == null)
                ? await _contentManager.GetPublishedVersionInfo(_pageContentType, pageId)
                : await _contentManager.GetVersionInfo(_pageContentType, pageId, fromVersionCode);

            if (sourceVersion != null)
            {
                var treeId = await _contentManager.GetContentTreeId(_pageContentType, pageId, sourceVersion.VersionCode);
                await _contentManager.CloneContentTree(treeId, draftVersion.VersionCode);
            }
            else
            {
                await _contentManager.CreateContentTree(_pageContentType, pageId, draftVersion.VersionCode);
            }

            return draftVersion;
        }

        public async Task PublishVersion(string pageId, string versionCode)
        {
            var page = await _db.Pages.FirstOrDefaultAsync(x => x.Id == pageId);

            if (page == null)
                throw new Exception($"Cannot publish PageID {pageId}. The page does not exist");


            await _contentManager.PublishDraftVersion(_pageContentType, pageId, versionCode);

            page.PublishedVersionCode = versionCode;
          
            await _db.SaveChangesAsync();
        }

        public async Task DeleteVersion(string pageId, string versionCode)
        {
            //TODO: Transaction to ensure data consistency
            await _contentManager.DeleteVersion(_pageContentType, pageId, versionCode);
            await _contentManager.DeleteContentTree(_pageContentType, pageId, versionCode);
        }

        public async Task UpdateVersionLabel(string pageId, string versionCode, string versionLabel)
        {
            await _contentManager.UpdateVersionLabel(_pageContentType, pageId, versionCode, versionLabel);
        }

        private string CleanRoute(string route)
        {
            if (string.IsNullOrEmpty(route))
            {
                route = "/";
            }
            else if (!route.Trim().StartsWith("/"))
            {
                route = "/" + route;
            }

            return route.Trim();
        }
    }
}
