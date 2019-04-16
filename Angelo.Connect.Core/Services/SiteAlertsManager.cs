using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Data;
using Angelo.Connect.Models;

namespace Angelo.Connect.Services
{
    public class SiteAlertsManager
    {
        private ContentManager _contentManager;
        private ConnectDbContext _db;
        private string CONTENT_TYPE_SITEALERTS = typeof(SiteAlert).Name;

        public SiteAlertsManager(ConnectDbContext connectDb, ContentManager contentManager)
        {
            _db = connectDb;
            _contentManager = contentManager;
        }

        public SiteAlert Get(string alertId)
        {
            return _db.SiteAlerts.FirstOrDefault(x => x.Id == alertId);
        }
        public SiteAlert Get(string alertId, string versionCode)
        {
          
            // otherwise get the data stored along with the version
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_SITEALERTS, alertId, versionCode).Result;

            if (versionInfo == null)
                throw new Exception($"Could not get the site alert. Missing version {versionCode}.");
            
            var versionData = _contentManager.GetStoredData<SiteAlert>(versionInfo);

            return versionData;
            
        }
        public SiteAlert GetModel()
        {
            return new SiteAlert()
            {
                Id = Guid.NewGuid().ToString("N"),
                VersionCode = DateTime.Now.ToString("yyyyMMdd-hhmmss"),
                Status = ContentStatus.Scratch,  //draft
                StartDate = DateTime.Now,
                EndDate = DateTime.Now
            };
        }
        public List<SiteAlert> GetSiteAlerts(string siteId)
        {
            return _db.SiteAlerts.Where(x => x.SiteId == siteId).ToList();
        }

        public List<SiteAlert> GetUserAlerts(string userId, string siteId)
        {
            return _db.SiteAlerts.Where(x => x.UserId == userId && x.SiteId == siteId).ToList();
        }

        public ContentVersion GetVersionInfo(SiteAlert model)
        {
            return _contentManager.GetVersionInfo(CONTENT_TYPE_SITEALERTS, model.Id, model.VersionCode).Result;
        }

        public async Task<SiteAlert> GetDraftAlertFromVersion(string id, string userId, string versionCode)
        {
            var currentAlert = Get(id);
            
           //Get the specific version or default to model version.
            versionCode = versionCode ?? currentAlert.VersionCode;

            var versionInfo = await _contentManager.GetVersionInfo(CONTENT_TYPE_SITEALERTS, id, versionCode);

            var model = _contentManager.GetStoredData<SiteAlert>(versionInfo);

            if (model.Status == ContentStatus.Published)
            {
                model = Clone(model, userId);
            }

            return model;
        }

        public async Task<SiteAlert> CreateSiteAlertAsync(string userId, string siteId)
        {
            
            var model = GetModel();
            model.UserId = userId;
            model.SiteId = siteId;

            var versionInfo = _contentManager.CreateDraftVersion(CONTENT_TYPE_SITEALERTS, model.Id, userId).Result;
            model.VersionCode = versionInfo.VersionCode;

            // Create a new contrent tree for this version and use tree builder to insert some content
            var contentTree = _contentManager.CreateContentTree(versionInfo).Result;
            var treeBuilder = _contentManager.CreateTreeBuilder(contentTree);

            // Add the html widget
            treeBuilder.AddRootContent("sitealert-body", settings => {
                settings.WidgetType = "html";
                settings.ModelName = "lorem-ipsum"; // from Angelo.Connect.Web.Data.Json, html.json
            });

            // save the tree
            treeBuilder.SaveChanges();

            model.ContentTreeId = contentTree.Id;

            _contentManager.SetVersionModelData(versionInfo, model).Wait();

            return await SaveAsync(model);
        }

        public async Task<SiteAlert> UpdateSiteAlertAsync(SiteAlert model, bool MustPublish, string NewVersionLabel)
        {
            var replaceWithPublishedVersionModel = false;
            var currentAlert = Get(model.Id);

            // Get the draft version info 
            var versionInfo = _contentManager.GetVersionInfo(CONTENT_TYPE_SITEALERTS, model.Id, model.VersionCode).Result;
            if (versionInfo == null)
                throw new Exception($"Cannot publish Site Alerts. SiteAlert {model.Id} does not have a draft version to publish.");

            if (!string.IsNullOrEmpty(NewVersionLabel))
            {
                _contentManager.UpdateVersionLabel(CONTENT_TYPE_SITEALERTS, model.Id, model.VersionCode, NewVersionLabel).Wait();
            }
            
            if (MustPublish)
            {
                model.Posted = DateTime.Now;
                model.Status = ContentStatus.Published;

                // Publish the new version / archive old version
                _contentManager.PublishDraftVersion(versionInfo).Wait();
                replaceWithPublishedVersionModel = true;
            }
            else
            {
                model.Status = ContentStatus.Draft;
            }

            await _contentManager.SetVersionModelData(versionInfo, model);

            if (model.VersionCode == currentAlert.VersionCode || replaceWithPublishedVersionModel)
                await SaveAsync(model);

            return model;
        }
        public async Task<SiteAlert> SaveAsync(SiteAlert siteAlert)
        {

            siteAlert.Posted = DateTime.Now;
            var model = _db.SiteAlerts.FirstOrDefault(x => x.Id == siteAlert.Id);

            if (model != null)
            {
                model.Title = siteAlert.Title;
                model.Posted = siteAlert.Posted;
                model.Status = siteAlert.Status;
                model.StartDate = siteAlert.StartDate;
                model.EndDate = siteAlert.EndDate;
                model.VersionCode = siteAlert.VersionCode;
                model.ContentTreeId = siteAlert.ContentTreeId;

                _db.SiteAlerts.Update(model);
            }
            else
            {
                //_db.Entry(siteAlert).State = EntityState.Added;
                _db.SiteAlerts.Add(siteAlert);
            }

            await _db.SaveChangesAsync();

            return siteAlert;
        }

        public SiteAlert Clone(SiteAlert model, string userId)
        {
            
            var versionInfo = _contentManager.CreateDraftVersion(CONTENT_TYPE_SITEALERTS, model.Id, userId).Result;
            var clonedContentTree = _contentManager.CloneContentTree(model.ContentTreeId, versionInfo.VersionCode).Result;
            
            var clone = new SiteAlert()
            {
                Id = model.Id,
                SiteId = model.SiteId,
                UserId = model.UserId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                Posted = model.Posted,
                Title = model.Title,
                Status = ContentStatus.Draft,
                VersionCode = versionInfo.VersionCode,
                ContentTreeId = clonedContentTree.Id
            };

            _contentManager.SetVersionModelData(versionInfo, clone).Wait();

            return clone;
        }

        public void Delete(string id)
        {
            Ensure.NotNullOrEmpty(id);

            var alert = _db.SiteAlerts.FirstOrDefault(x => x.Id == id);

            if (alert != null)
            {
                _contentManager.DeleteAllVersions(CONTENT_TYPE_SITEALERTS, alert.Id).Wait();
                _contentManager.DeleteAllContentTrees(CONTENT_TYPE_SITEALERTS, alert.Id).Wait();

                _db.Remove(alert);
                _db.SaveChanges();
            }
        }
    }
}
