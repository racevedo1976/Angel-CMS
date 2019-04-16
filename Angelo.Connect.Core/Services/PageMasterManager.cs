using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Models;

namespace Angelo.Connect.Services
{
    public class PageMasterManager
    {
        private ConnectDbContext _db;
        private ConnectCoreOptions _coreOptions;
        private ContentManager _contentManager;
        public SiteTemplateManager _templateManager;

        private string _contentType = typeof(PageMaster).FullName;

        public PageMasterManager(ContentManager contentManager, SiteTemplateManager templateManager, ConnectDbContext dbContext, ConnectCoreOptions coreOptions)
        {
            _db = dbContext;
            _contentManager = contentManager;
            _templateManager = templateManager;
            _coreOptions = coreOptions;
        }

        public async Task<ICollection<PageMaster>> GetMasterPagesAsync(string siteId)
        {
            return await _db.PageMasters
                        .Where(x => x.Site.Id == siteId)
                        .OrderBy(x => x.Title)
                        .ToListAsync();
        }

        public async Task<PageMaster> GetSiteDefaultAsync(string siteId)
        {
            // check for system page first
            var masterPage = await _db.PageMasters.FirstOrDefaultAsync(x => x.SiteId == siteId && x.IsSystemPage == true);

            // If no system page, get the default
            if(masterPage == null)
            {
                masterPage = await _db.PageMasters.FirstOrDefaultAsync(x => x.SiteId == siteId && x.IsDefault == true);
            }
            else  // If no default is configured, just return the first.
            {
                if(masterPage == null)
                {
                    masterPage = await _db.PageMasters.FirstOrDefaultAsync(x => x.SiteId == siteId);
                }
            }

            

            return masterPage;
        }

        public async Task<PageMaster> GetByIdAsync(string pageMasterId)
        {
            return await _db.PageMasters
                        .Where(x => x.Id == pageMasterId)
                        .FirstOrDefaultAsync();
        }

        public async Task<string> CreateAsync(PageMaster pageMaster)
        {
            Ensure.Argument.NotNull(pageMaster);

            try
            {
                if(string.IsNullOrEmpty(pageMaster.Id))
                    pageMaster.Id = KeyGen.NewGuid();

                _db.PageMasters.Add(pageMaster);

                await _db.SaveChangesAsync();
                return pageMaster.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }
      
        public async Task<bool> UpdateAsync(PageMaster pageMaster)
        {
            Ensure.Argument.NotNull(pageMaster);

            var tempPageMaster = await _db.PageMasters.FirstOrDefaultAsync(x => x.Id == pageMaster.Id);
           
            if (tempPageMaster == null)
            {
                return false;
            }

            tempPageMaster.Title = pageMaster.Title;
            tempPageMaster.TemplateId = pageMaster.TemplateId;
            tempPageMaster.PreviewPath = pageMaster.PreviewPath;

            //NOTE: Currently the "Default" master page is set during seeding with no user option to change.
            //      Undo the following code block once we allow the user to set this value         
            /* 
             * 
                tempPageMaster.IsDefault = pageMaster.IsDefault;

                // Logic to change the existing "Default" master page when "IsDefault" changes.
                if(tempPageMaster.IsDefault == true)
                {
                    var oldDefault = await GetDefaultMasterPage(pageMaster.SiteId);

                    if (oldDefault != null && oldDefault.Id != tempPageMaster.Id)
                    {
                        oldDefault.IsDefault = false;
                    }
                }
            */

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

        public async Task<ContentVersion> GetPublishedVersion(string pageMasterId)
        {
            return await _contentManager.GetPublishedVersionInfo(_contentType, pageMasterId);
        }

        public async Task<ContentVersion> GetLatestDraftVersion(string pageMasterId)
        {
            var drafts = await _contentManager.GetVersionHistory(_contentType, pageMasterId, ContentStatus.Draft);

            return drafts.OrderByDescending(x => x.Created).FirstOrDefault();
        }

        public async Task<ContentVersion> GetVersion(string pageMasterId, string versionCode)
        {
            return await _contentManager.GetVersionInfo(_contentType, pageMasterId, versionCode);
        }

        public async Task<IEnumerable<ContentVersion>> GetVersions(string pageMasterId)
        {
            // TODO: Decouple version information from content collections
            return await _contentManager.GetVersionHistory(_contentType, pageMasterId);
        }

        public async Task<ContentVersion> CreateDraftVersion(string pageMasterId, string userId, string versionLabel, string fromVersionCode = null)
        {
            // TODO: Fix using user name instead of ID so won't have to lookup in disconnected identity db context. 
            var draftVersion = await _contentManager.CreateDraftVersion(_contentType, pageMasterId, userId, versionLabel);

            var sourceVersion = (fromVersionCode == null)
                ? await _contentManager.GetPublishedVersionInfo(_contentType, pageMasterId)
                : await _contentManager.GetVersionInfo(_contentType, pageMasterId, fromVersionCode);

            if (sourceVersion != null)
            {
                var treeId = await _contentManager.GetContentTreeId(_contentType, pageMasterId, sourceVersion.VersionCode);
                await _contentManager.CloneContentTree(treeId, draftVersion.VersionCode);
            }
            else
            {
                await _contentManager.CreateContentTree(_contentType, pageMasterId, draftVersion.VersionCode);
            }

            return draftVersion;
        }

        public async Task<ContentVersion> CreateInitialVersion(string pageMasterId, string[] seedFiles)
        {
            var masterPage = await _db.PageMasters
                .Include(x => x.Site)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pageMasterId);

            if (masterPage == null)
                throw new NullReferenceException($"Cannot locate MasterPage {pageMasterId}");

            var publishedVersion = new ContentVersion
            {
                ContentId = masterPage.Id,
                ContentType = _contentType,
                VersionLabel = "Initial Version",
                Status = ContentStatus.Published,
                UserId = "system",
            };

            var masterPageContent = new ContentTree(publishedVersion);

            _db.ContentVersions.Add(publishedVersion);
            _db.ContentTrees.Add(masterPageContent);

            await _db.SaveChangesAsync();

            if (seedFiles != null && seedFiles.Length > 0)
            {
                var treeBuilder = _contentManager.CreateTreeBuilder(masterPageContent);

                foreach (var filePath in seedFiles)
                    treeBuilder.SeedFromFile(filePath);

                treeBuilder.SaveChanges();
            }

            return publishedVersion;
        }


        //TODO: Depricate this old version - use new version that accepts a list of files to decomple
        //      from the template
        public async Task<ContentVersion> CreateInitialVersion(string pageMasterId, bool seedFromTemplate)
        {
            var masterPage = await _db.PageMasters
                .Include(x => x.Site)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == pageMasterId);

            if (masterPage == null)
                throw new NullReferenceException($"Cannot locate MasterPage {pageMasterId}");

            var siteTemplate = _templateManager.GetTemplate(masterPage.Site.SiteTemplateId);
            var pageTemplate = siteTemplate.PageTemplates.FirstOrDefault(x => x.Id == masterPage.TemplateId);

            var seedFiles = pageTemplate.SeedData?.Select(file => 
                _coreOptions.FileSystemRoot + "\\" + siteTemplate.DataFolder + "\\" + file
            );

            return await CreateInitialVersion(pageMasterId, seedFiles.ToArray());      
        }

        public async Task PublishVersion(string pageMasterId, string versionCode)
        {
            var master = await _db.PageMasters.FirstOrDefaultAsync(x => x.Id == pageMasterId);

            if (master == null)
                throw new Exception($"Cannot publish Master Page {pageMasterId}. The page does not exist");

            await _contentManager.PublishDraftVersion(_contentType, pageMasterId, versionCode);

            //master.PublishedVersionCode = versionCode;

            await _db.SaveChangesAsync();
        }

        public async Task DeleteVersion(string pageMasterId, string versionCode)
        {
            //TODO: Transaction to ensure data consistency
            await _contentManager.DeleteVersion(_contentType, pageMasterId, versionCode);
            await _contentManager.DeleteContentTree(_contentType, pageMasterId, versionCode);
        }

        public async Task UpdateVersionLabel(string pageMasterId, string versionCode, string versionLabel)
        {
            await _contentManager.UpdateVersionLabel(_contentType, pageMasterId, versionCode, versionLabel);
        }

    }
}
