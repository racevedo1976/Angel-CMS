using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Extensions;
using Angelo.Common.Models;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Rendering;
using Angelo.Connect.Configuration;

using Angelo.Identity;
using Angelo.Identity.Services;

namespace Angelo.Connect.Services
{
    public class SiteManager
    {
        private ConnectDbContext _db;
        private TenantManager _oidcTenantManager;
        private DirectoryManager _oidcDirectoryManager;
        private SecurityPoolManager _oidcPoolManager;
        private SiteTemplateManager _templateManager;
        private ClientManager _clientManager;
        private ConnectCoreOptions _coreOptions;

        public SiteManager
        (
            ConnectDbContext dbContext, 
            TenantManager oidcTenantManager,
            DirectoryManager oidcDirectoryManager,
            SecurityPoolManager oidcPoolManager,
            SiteTemplateManager templateManager,
            ClientManager clientManager,
            ConnectCoreOptions coreOptions
        )
        {
            _db = dbContext;
            _templateManager = templateManager;
            _oidcTenantManager = oidcTenantManager;
            _oidcDirectoryManager = oidcDirectoryManager;
            _oidcPoolManager = oidcPoolManager;
            _clientManager = clientManager;
            _coreOptions = coreOptions;
        }

        public async Task<ICollection<Site>> GetAllAsync()
        {
            return await _db.Sites.Include(x => x.Client).ToListAsync();
        }

        protected void LoadSiteTemplates(List<Site> sites)
        {
            foreach (var site in sites)
            {
                site.SiteTemplate = _templateManager.GetTemplate(site.SiteTemplateId);
            }
        }

        public async Task<List<Site>> GetCorpSitesAsync()
        {
            var sites = await _db.Sites
                .AsNoTracking()
                .Include(x => x.Client)
                .Include(x => x.ClientProductApp)
                .ToListAsync();

            LoadSiteTemplates(sites);
            return sites;
        }

        public async Task<List<Site>> GetClientSitesAsync(string clientId)
        {
            var sites = await _db.Sites
                .AsNoTracking()
                .Include(x => x.Client)
                .Include(x => x.ClientProductApp)
                .Where(x => x.ClientId == clientId)
                .ToListAsync();

            LoadSiteTemplates(sites);
            return sites;
        }

        public async Task<List<Site>> GetAppSitesAsync(string appId)
        {
            var sites = await _db.Sites
                .AsNoTracking()
                .Include(x => x.Client)
                .Include(x => x.ClientProductApp)
                .Where(x => x.ClientProductAppId == appId)
                .ToListAsync();

            LoadSiteTemplates(sites);
            return sites;
        }

        protected IQueryable<Site> QueryByClientId(string clientId, string sortOrder)
        {
            var sites = _db.Sites
                .AsNoTracking()
                .Include(x => x.Client)
                .Include(x => x.Domains)
                .Include(x => x.SiteCollectionMaps).ThenInclude(x => x.SiteCollection)
                .Where(x => x.ClientId == clientId);

            foreach (var site in sites)
            {
                site.SiteTemplate = _templateManager.GetTemplate(site.SiteTemplateId);
            }


            if (string.IsNullOrEmpty(sortOrder) == false)
            {
                switch (sortOrder)
                {
                    case "title":
                        sites = sites.OrderBy(x => x.Title);
                        break;
                    case "title_desc":
                        sites = sites.OrderByDescending(x => x.Title);
                        break;
                    case "tenantKey":
                        sites = sites.OrderBy(x => x.TenantKey);
                        break;
                    case "tenantKey_desc":
                        sites = sites.OrderByDescending(x => x.TenantKey);
                        break;
                }
            }
            return sites;
        }

        public async Task<ICollection<Site>> GetByClientIdAsync(string clientId, string sortOrder = "title")
        {
            return await QueryByClientId(clientId, sortOrder).ToListAsync();
        }

        public async Task<PagedResult<Site>> GetPageByClientIdAsync(string clientId, int pageNumber, int pageSize, string sortOrder)
        {
            return await QueryByClientId(clientId, sortOrder).PagedResultAsync(pageNumber, pageSize);
        }

        public async Task<int> GetPageNumberOfSiteIdAsync(string clientId, string siteId, int pageSize, string sortOrder)
        {
            var sites = await QueryByClientId(clientId, sortOrder)
                .Select(x => new { Id = x.Id })
                .ToListAsync();

            var count1 = sites.Count();
            for (var index1 = 0; index1 < count1; index1++)
            {
                if (sites[index1].Id == siteId)
                {
                    return (index1 / pageSize) + 1;
                }
            }
            return 0;
        }

        public async Task<IEnumerable<KeyValuePair<string, string>>> GetSiteTitleSuggestions(string clientId, string query)
        {
            var sites = await _db.Sites.AsNoTracking()
                .Where(x => x.Title.Contains(query) && x.ClientId == clientId)
                .Select(x => new { Key = x.Id, Value = x.Title })
                .ToListAsync();

            return sites.Select(x => new KeyValuePair<string, string>(key: x.Key, value: x.Value));
        }

        public async Task<Site> GetByTenantKeyAsync(string tenantKey)
        {
            var site = await _db.Sites
                .AsNoTracking()
                .Include(x => x.Domains)
                .Include(x => x.SiteCollectionMaps).ThenInclude(x => x.SiteCollection)
                .FirstOrDefaultAsync(x => x.TenantKey == tenantKey);

            site.SiteTemplate = _templateManager.GetTemplate(site.SiteTemplateId);
     
            return site;
        }

        public async Task<Site> GetByIdAsync(string siteId)
        {
            var site = await _db.Sites
                .Include(x => x.Domains)
                .Include(x => x.Cultures)
                .Include(x => x.SiteCollectionMaps).ThenInclude(x => x.SiteCollection)
                .Include(x => x.SiteDirectories)
                .FirstOrDefaultAsync(x => x.Id == siteId);

            if (site != null)
                site.SiteTemplate = _templateManager.GetTemplate(site.SiteTemplateId);

            return site;
        }

        public async Task<Site> GetByTitleAsync(string clientId, string title)
        {
            var site = await _db.Sites.AsNoTracking()
                .Where(x => x.Title == title && x.ClientId == clientId)
                .FirstOrDefaultAsync();
            return site;
        }

        public async Task<Site> GetByTenantKeyAsync(string clientId, string tenantKey)
        {
            var site = await _db.Sites.AsNoTracking()
                .Where(x => x.TenantKey == tenantKey && x.ClientId == clientId)
                .FirstOrDefaultAsync();
            return site;
        }

        public async Task<Client> GetClientAsync(string siteId)
        {
            Ensure.Argument.NotNull(siteId);

            var site = await _db.Sites
                .AsNoTracking()
                .Include(x => x.Client)
                .FirstOrDefaultAsync(x => x.Id == siteId);

            if(site == null)
                throw new NullReferenceException($"Could not locate Client for site. Site {siteId} does not exist.");

            return site.Client;              
        }

        public async Task<ICollection<Site>> GetSitesAsnyc()
        {
            var sites = await _db.Sites
                .AsNoTracking()
                .Include(x => x.Domains)
                .Include(x => x.SiteCollectionMaps).ThenInclude(x => x.SiteCollection)
                .ToListAsync();

            foreach (var site in sites)
            {
                site.SiteTemplate = _templateManager.GetTemplate(site.SiteTemplateId);
            }

            return sites;
        }

        public async Task<ICollection<Site>> GetSiblingSitesAsnyc(Site site)
        {
            Ensure.Argument.NotNull(site, "site");

            var sites = await _db.Sites
                .AsNoTracking()
                .Include(x => x.Domains)
                .ToListAsync();

            foreach (var siteObject in sites)
            {
                siteObject.SiteTemplate = _templateManager.GetTemplate(siteObject.SiteTemplateId);
            }

            return sites;
        }

        public async Task<ICollection<SiteDomain>> GetDomainsAsync(string siteId)
        {
            Ensure.Argument.NotNull(siteId, "siteId");
            return await _db.SiteDomains
                .AsNoTracking()
                .Where(x => x.SiteId == siteId)
                .ToListAsync();
        }

        public async Task<SiteDomain> GetDefaultDomainAsync(string siteId)
        {
            Ensure.Argument.NotNull(siteId, "siteId");

            // get the preferred domain
            var defaultDomain = await _db.SiteDomains
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SiteId == siteId && x.IsDefault);

            // otherwise just get any mapped domain
            if(defaultDomain == null)
                defaultDomain = await _db.SiteDomains
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SiteId == siteId);

            return defaultDomain;
        }

        public async Task<SiteDomain> GetDefaultDomainWithProtocolAsync(string siteId)
        {
            Ensure.Argument.NotNull(siteId, "siteId");
            var protocol = _coreOptions.UseHttpsForAbsoluteUris ? "https://" : "http://";

            // get the preferred domain
            var defaultDomain = await _db.SiteDomains
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SiteId == siteId && x.IsDefault);

            // otherwise just get any mapped domain
            if (defaultDomain == null)
                defaultDomain = await _db.SiteDomains
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.SiteId == siteId);

            defaultDomain.DomainKey = protocol + defaultDomain.DomainKey;

            return defaultDomain;
        }

        public async Task<ICollection<SiteCollection>> GetSiteCollectionsAsync(string siteId)
        {
            Ensure.Argument.NotNull(siteId, "siteId");
            return await _db.SiteCollectionMaps
                .AsNoTracking()
                .Include(x => x.SiteCollection)
                .Where(x => x.SiteId == siteId)
                .Select(sm => sm.SiteCollection)
                .ToListAsync();
        }

        public async Task RemoveSiteCollectionsAsync(string siteId, IList<string> siteCollectionIds)
        {
            var siteCollections = await _db.SiteCollectionMaps
                .Where(x => x.SiteId == siteId)
                .Where(x => siteCollectionIds.Contains(x.SiteCollectionId))
                .ToListAsync();
            _db.SiteCollectionMaps.RemoveRange(siteCollections);
            await _db.SaveChangesAsync();
        }

        public async Task AddSiteCollectionsAsync(string siteId, IList<string> siteCollectionIds)
        {
            var existingIds = await _db.SiteCollectionMaps.AsNoTracking()
                .Where(x => x.SiteId == siteId)
                .Where(x => siteCollectionIds.Contains(x.SiteCollectionId))
                .Select(x => x.SiteCollectionId)
                .ToListAsync();

            foreach (var siteCollectionId in siteCollectionIds)
            {
                if (existingIds.Contains(siteCollectionId) == false)
                {
                    var newItem = new SiteCollectionMap();
                    newItem.Site = await _db.Sites.Where(x => x.Id == siteId).FirstOrDefaultAsync();
                    newItem.SiteCollection = await _db.SiteCollections.Where(x => x.Id == siteCollectionId).FirstOrDefaultAsync();
                    if ((newItem.Site != null) && (newItem.SiteCollection != null))
                        _db.SiteCollectionMaps.Add(newItem);
                }
            }
            await _db.SaveChangesAsync();
        }

        public SiteTemplate GetTemplateByIdAsync(string templateId)
        {
            Ensure.Argument.NotNull(templateId, "templateId");

            return _templateManager.GetTemplate(templateId);
                
        }

        public async Task UpdateTemplateTheme(string siteId, string templateId, string themeId)
        {
            Ensure.Argument.NotNull(siteId, "siteId");

            var site = await _db.Sites
                .Where(x => x.Id == siteId)
                .FirstOrDefaultAsync();
            if (site != null)
            {
                site.ThemeId = themeId;
                site.SiteTemplateId = templateId;
                await _db.SaveChangesAsync();
            }

        }

        public async Task<Site> GetByDomainKeyAsync(string domainKey)
        {
            var site = await _db.Sites
                .AsNoTracking()
                .Include(x => x.Client)
                .Include(x => x.SiteCollectionMaps).ThenInclude(x => x.SiteCollection)
                .Include(x => x.ClientProductApp).ThenInclude(y => y.Product)
                .FirstOrDefaultAsync(x =>
                    x.Domains.Any(y => y.DomainKey == domainKey)
                );

            site.SiteTemplate = _templateManager.GetTemplate(site.SiteTemplateId);


            return site;
        }

        public async Task<bool> DeleteAsync(string siteId)
        {
            var site = await _db.Sites.Where(x => x.Id == siteId).FirstOrDefaultAsync();
            var client = await GetClientAsync(siteId);
            var cultures = await _db.SiteCultures.Where(x => x.SiteId == siteId).ToListAsync();
            var siteCol = await _db.SiteCollectionMaps.Where(x => x.SiteId == siteId).ToListAsync();
            var domains = await _db.SiteDomains.Where(x => x.SiteId == siteId).ToListAsync();
            var navMenus = await _db.NavigationMenu.Where(x => x.SiteId == siteId).ToListAsync();
            var pageMaster = await _db.PageMasters.Where(x => x.SiteId == siteId).ToListAsync();

            if (site == null)
                return false;

            _db.SiteCultures.RemoveRange(cultures);
            _db.SiteCollectionMaps.RemoveRange(siteCol);
            _db.Sites.Remove(site);

            await _db.SaveChangesAsync();

            // remove oidcUris          
            foreach (var domain in domains)
            {
                await _oidcTenantManager.RemoveUrisMatchingAsync(client.TenantKey, domain.DomainKey);
            };

            return true;
        }

        public async Task<string> CreateAsync(Site site)
        {
            if (string.IsNullOrEmpty(site.Id))
                site.Id = KeyGen.NewGuid();

            // Configure the OIDC security pool for the site
            var client = await _clientManager.GetByIdAsync(site.ClientId);
            var directories = await _oidcDirectoryManager.GetDirectoriesAsync(client.TenantKey);

            var sitePool = await _oidcPoolManager.CreateChildPoolAsync
            (
                client.SecurityPoolId,
                site.Title,
                directories
            );
            site.SecurityPoolId = sitePool.PoolId;


            // Create the site
            _db.Sites.Add(site);

            await _db.SaveChangesAsync();     

            return site.Id;
        }
  
        public async Task UpdateAsync(Site site)
        {
            var entity = _db.Sites
                .Include(x => x.Cultures)
                //.Include(x => x.SiteDirectories)
                .FirstOrDefault(x => x.Id == site.Id);

            if (entity == null)
                throw new KeyNotFoundException($"Could not locate site id {site.Id}");

            if (!string.IsNullOrEmpty(site.SecurityPoolId)) entity.SecurityPoolId = site.SecurityPoolId;
            if (!string.IsNullOrEmpty(site.TenantKey)) entity.TenantKey = site.TenantKey;
            if (!string.IsNullOrEmpty(site.Banner)) entity.Banner = site.Banner;
            if (!string.IsNullOrEmpty(site.Title)) entity.Title = site.Title;
            if (!string.IsNullOrEmpty(site.SiteTemplateId)) entity.SiteTemplateId = site.SiteTemplateId;
            if (!string.IsNullOrEmpty(site.ThemeId)) entity.ThemeId = site.ThemeId;
            if (!string.IsNullOrEmpty(site.DefaultCultureKey)) entity.DefaultCultureKey = site.DefaultCultureKey;

            entity.Published = site.Published;

            if (site.Cultures != null)
            {
                // Remove Cultures
                var keys = site.Cultures.Select(x => x.CultureKey).ToList();
                _db.SiteCultures.RemoveRange(entity.Cultures.Where(x => keys.Contains(x.CultureKey) == false));

                // Add Cultures
                foreach (var item in site.Cultures)
                    if (entity.Cultures.Any(x => x.CultureKey == item.CultureKey) == false)
                        entity.Cultures.Add(item);

                // Set the DefaultSiteCulture if need be.
                if (site.Cultures.Any(x => x.CultureKey == site.DefaultCultureKey) == false)
                    entity.DefaultCultureKey = site.Cultures.FirstOrDefault()?.CultureKey;
            }

            // SiteDirectories is no longer used. This will need to be refactored to properly update Directories for a Site.
            /*
            //Remove Directories and Add selected ones
            foreach (var dir in entity.SiteDirectories.ToList())
            {
                if (!site.SiteDirectories.Any(x => x.DirectoryId == dir.DirectoryId))
                {
                    entity.SiteDirectories.Remove(dir);
                }
            }
            //Add new Dir only
            foreach(var newDir in site.SiteDirectories)
            {
                if (!entity.SiteDirectories.Any(x => x.DirectoryId == newDir.DirectoryId))
                {
                    entity.SiteDirectories.Add(newDir);
                }
            }
            */
           
            await _db.SaveChangesAsync();
        }    

        public async Task<bool> DomainExistsAsync(string domainKey)
        {
            domainKey = NormalizeDomain(domainKey);

            return await _db.SiteDomains.AsNoTracking().Where(x => x.DomainKey == domainKey).AnyAsync();
        }

        public async Task<bool> AddSiteDomainAsync(string siteId, string domainKey, bool isDefault)
        {
            Ensure.NotNull(siteId);
            Ensure.NotNull(domainKey);

            var client = await GetClientAsync(siteId);
            var domainExists = await DomainExistsAsync(domainKey);

            if (domainExists)
            {
                return false;
            }
            else
            {
                //If IsDefault True, then clear any other default domains
                //as this domain will be the default
                var defaultDomain = await GetDefaultDomainAsync(siteId);
                if (defaultDomain != null)
                {
                    defaultDomain.IsDefault = false;
                    _db.SiteDomains.Update(defaultDomain);
                    _db.SaveChanges();
                }


                // Save the site domain
                var siteDomain = new SiteDomain()
                {
                    SiteId = siteId,
                    DomainKey = NormalizeDomain(domainKey),
                    IsDefault = isDefault
                };

                _db.SiteDomains.Add(siteDomain);
                await _db.SaveChangesAsync();


                // Register OIDC Redirect Uris 
                await _oidcTenantManager.AddUriAsync(
                    client.TenantKey,
                    siteDomain.ToSignInRedirectUri(),
                    Identity.Models.TenantUriType.OidcSignin
                );

                await _oidcTenantManager.AddUriAsync(
                   client.TenantKey,
                   siteDomain.ToPostLogoutRedirectUri(),
                   Identity.Models.TenantUriType.OidcPostLogout
               );

                return true;
            }
        }

        public async Task<bool> RemoveSiteDomainAsync(string siteId, string domainKey)
        {
            Ensure.NotNull(siteId);
            Ensure.NotNull(domainKey);

            domainKey = NormalizeDomain(domainKey);

            var client = await GetClientAsync(siteId);
            var domain = await _db.SiteDomains
                .Where(x => x.SiteId == siteId && x.DomainKey == domainKey)
                .FirstOrDefaultAsync();

            if (domain == null)
                return false;
            else
            {
                // Remove site domain record
                _db.SiteDomains.Remove(domain);
                await _db.SaveChangesAsync();

                // Remove OIDC Uris 
                await _oidcTenantManager.RemoveUrisMatchingAsync(client.TenantKey, domainKey);

                return true;
            }
        }

        public async Task<bool> SaveSiteCulturesAsync(string siteId, IEnumerable<SiteCulture> cultures)
        {
            var site = await _db.Sites
                .Where(x => x.Id == (siteId))
                .Include(x => x.Cultures)
                .FirstOrDefaultAsync();

            if (site == null)
                return false;
            else
            {

                // Remove Cultures
                var keys = cultures.Select(x => x.CultureKey).ToList();
                _db.SiteCultures.RemoveRange(site.Cultures.Where(x => keys.Contains(x.CultureKey) == false));

                // Add Cultures
                foreach (var item in cultures)
                    if (site.Cultures.Any(x => x.CultureKey == item.CultureKey) == false)
                        site.Cultures.Add(item);

                // Set the DefaultSiteCulture if need be.
                if (cultures.Any(x => x.CultureKey == site.DefaultCultureKey) == false)
                    site.DefaultCultureKey = cultures.FirstOrDefault()?.CultureKey;

                await _db.SaveChangesAsync();
                return true;
            }
        }

        public async Task<ICollection<SiteSetting>> GetSiteSettingsAsync(string siteId)
        {
            var settings = await _db.SiteSettings
                .AsNoTracking()
                .Where(x => x.SiteId == siteId)
                .OrderBy(x => x.FieldName)
                .ToListAsync();

            return settings;
        }

        public async Task<SiteSetting> GetSiteSettingAsync(string siteId, string name)
        {
            return await _db.SiteSettings
                .FirstOrDefaultAsync(x =>
                    x.SiteId == siteId && x.FieldName == name
                );
        }

        public async Task SaveSiteSettingAsync(string siteId, string name, string value)
        {
            var setting = await GetSiteSettingAsync(siteId, name);

            if(setting != null)
            {
                setting.Value = value;
            }
            else
            {
                _db.SiteSettings.Add(new SiteSetting
                {
                    SiteId = siteId,
                    FieldName = name,
                    Value = value
                });
            }

            await _db.SaveChangesAsync();
        }

        public async Task<bool> SaveSiteSettingsAsync(string siteId, IEnumerable<SiteSetting> settings)
        {
            Ensure.Argument.NotNull(siteId, "siteId");
            
            foreach (var setting in settings)
            {
                var storedSetting = await _db.SiteSettings.FirstOrDefaultAsync(x => x.FieldName == setting.FieldName && x.SiteId == siteId);
                if (storedSetting != null)
                    storedSetting.Value = setting.Value;
                else
                {
                    setting.SiteId = siteId;
                    _db.SiteSettings.Add(setting);
                }
                    
                await _db.SaveChangesAsync();
            }
            
            return true;
            
        }

        private string NormalizeDomain(string domain)
        {
            domain = domain.Replace("https://", "");
            domain = domain.Replace("http://", "");

            if (domain.EndsWith("/"))
                domain = domain.Substring(0, domain.Length - 1);

            return domain.ToLower();
        }

    }
}
