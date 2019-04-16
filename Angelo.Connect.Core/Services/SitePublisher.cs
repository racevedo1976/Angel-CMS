using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Identity;
using Angelo.Identity.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Rendering;
using Angelo.Connect.Documents;

namespace Angelo.Connect.Services
{
    public class SitePublisher
    {
        public const string ROLE_SITE_ADMIN = "Site Admins";
        public const string ROLE_SITE_STAFF = "Site Staff";     
        public const string ROLE_SITE_USERS = "Registered Users";

        public string Name { get; } = "Default";

        private ConnectDbContext _connectDb;
        private ConnectCoreOptions _coreOptions;
        private IdentityDbContext _identityDb;
        private SiteTemplateManager _templateManager;
        private ContentManager _contentManager;
        private PageManager _pageManager;
        private PageMasterManager _masterPageManager;
        private ClientManager _clientManager;
        private IFolderManager<FileDocument> _folderManager;

        private SiteTemplate _siteTemplate;
        private IList<PageTemplate> _pageTemplates;

        private string _defaultDataFolder;

        public SitePublisher
        (
            ConnectDbContext connectDb, 
            ConnectCoreOptions coreOptions,
            IdentityDbContext identityDb,
            SiteTemplateManager templateManager,
            PageMasterManager masterPageManager,
            PageManager pageManager,
            ContentManager contentManager,
            ClientManager clientManager,
            IFolderManager<FileDocument> folderManager
        )
        {
            _connectDb = connectDb;
            _identityDb = identityDb;
            _templateManager = templateManager;
            _contentManager = contentManager;
            _clientManager = clientManager;
            _folderManager = folderManager;
            _pageManager = pageManager;
            _masterPageManager = masterPageManager;
            _coreOptions = coreOptions;

            _defaultDataFolder = _coreOptions.FileSystemRoot + "\\data\\json\\seeddata";
        }

        public async Task CreateInitialVersion(Site site)
        {
            // Set the site's client object if needed
            if(site.Client == null)
                site.Client = await _clientManager.GetByIdAsync(site.ClientId);

            // Get the site template
            _siteTemplate = _templateManager.GetTemplate(site.SiteTemplateId);
            _pageTemplates = _siteTemplate.PageTemplates;

            if (_siteTemplate.Schema == SiteTemplateExporter.CURRENT_SCHEMA)
            {
                // Site Templates contain master pages - use new logic
                var masterPageMap = CreateMasterPagesV1(site);
                var sitePageMap = CreateSitePagesV1(site, masterPageMap);

                // Site Templates contain navigation menus
                CreateSiteNavigationV1(site, sitePageMap);
            }
            else
            {
                // Create Pages & Content
                var masterPages = CreateMasterPagesV0(site);
                var sitePages = CreateSitePagesV0(site, masterPages);

                // Seed Navigation
                CreateSiteNavigationV0(site, sitePages);
            }

            // Seed Site Roles
            CreateSiteRoles(site);

            // Seed Site Css
            CreateSiteCss(site);

            // Seed Document Library
            await CreateLibraryAndRootFolder(site);
        }

        public void CreateSysMasterPageV0(Site site)
        {
            PageTemplate sysTemplate = new PageTemplate
            {
                Id = "System.cshtml",
                Title = "System Page"
            };

            var masterPages = new List<PageMaster>();
            var pageTemplates = new List<PageTemplate>();
            pageTemplates.Add(sysTemplate);

            foreach (var template in pageTemplates)
            {
                var masterPage = new PageMaster
                {
                    Id = KeyGen.NewGuid().ToString(),
                    SiteId = site.Id,
                    TemplateId = template.Id,
                    Title = template.Title + " Master Page",
                    PreviewPath = "/img/admin/na.png",
                    IsDefault = false,
                    IsSystemPage = true
                };

                _masterPageManager.CreateAsync(masterPage).Wait();
                _masterPageManager.CreateInitialVersion(masterPage.Id, true).Wait();

                masterPages.Add(masterPage);
            }
        }

        public async Task QueueSearchIndex(Site site)
        {
            if (site == null)
                throw new ArgumentException(nameof(site));

            var client = site.Client ?? await _clientManager.GetByIdAsync(site.ClientId);

            if (client == null)
                throw new NullReferenceException($"Cannot queue search index. Null client. Site: {site.Id}");

            var domain = await _connectDb.SiteDomains
                .Where(x => x.SiteId == site.Id)
                .OrderByDescending(x => x.IsDefault)
                .FirstOrDefaultAsync();

            if (domain == null)
                throw new Exception($"Cannot queue search index. Null domain. Site: {site.Id}");

            // else write to the crawler's queue folder to trigger a search index to 
            // be built on this site as soon as possible
            try
            {
                // using tenant keys for file name because is easier to read
                // and crawler doesn't care about the file name
                var fileName = client.TenantKey + "_" + site.TenantKey + ".json";
                var folderPath = Path.Combine(_coreOptions.SearchIndexRoot, "Queue");
                var filePath = Path.Combine(folderPath, fileName);

                // skip writing if the site is already in the queue to prevent bumping it 
                // down in processing order
                if (!File.Exists(filePath))
                {
                    if (!System.IO.Directory.Exists(folderPath))
                        System.IO.Directory.CreateDirectory(folderPath);

                    // crawler needs a public url
                    var protocol = _coreOptions.UseHttpsForAbsoluteUris ? "https://" : "http://";
                    var siteUrl = protocol + domain.DomainKey;

                    var queueData = new { siteId = site.Id, siteUrl = siteUrl };
                    var contents = JsonConvert.SerializeObject(queueData, Formatting.Indented);

                    File.WriteAllText(filePath, contents);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot queue search index. An exception occurred writing the data file.", ex);
            }

        }

        private async Task CreateLibraryAndRootFolder(Site site)
        {
            var locationResolver = new DocumentPhysicalLocationResolver("Site", site.ClientId, site.Id, "");

            //creates the document library
            var documentLibrary = await _folderManager.CreateDocumentLibrary(site.Id, "Site", locationResolver.Resolve());

            //create root folder / ensure there is a root folder
            await _folderManager.GetRootFolderAsync(site.Id);
        }

        private IEnumerable<PageMaster> CreateMasterPagesV0(Site site)
        {
            var masterPages = new List<PageMaster>();

            foreach(var template in _pageTemplates)
            {
                var masterPage = new PageMaster
                {
                    Id = KeyGen.NewGuid().ToString(),
                    SiteId = site.Id,
                    TemplateId = template.Id,
                    Title = template.Title + " Master Page",
                    PreviewPath = "/img/admin/na.png",
                    IsDefault = _pageTemplates[0].Id == template.Id
                };

                _masterPageManager.CreateAsync(masterPage).Wait();
                _masterPageManager.CreateInitialVersion(masterPage.Id, true).Wait();

                masterPages.Add(masterPage);
            }

            return masterPages;
        }

        private Dictionary<string, PageMaster> CreateMasterPagesV1(Site site)
        {
            var index = 0;
            var masterPageMap = new Dictionary<string, PageMaster>();
            var dataFolder = _coreOptions.FileSystemRoot + "\\" + _siteTemplate.DataFolder;

            foreach (var item in _siteTemplate.MasterPages)
            {
                var pageTemplate = _pageTemplates.FirstOrDefault(x => x.Id == item.Template);

                if (pageTemplate == null)
                    throw new Exception($"Could not create MasterPage {item.Id}. Invalid page template {item.Template}");

                var masterPage = new PageMaster
                {
                    Id = KeyGen.NewGuid().ToString(),
                    SiteId = site.Id,
                    TemplateId = item.Template,
                    Title = pageTemplate.Title + " Master Page",
                    PreviewPath = "/img/admin/na.png",
                    IsDefault = index == 0
                };

                var seedFiles = item.SeedData?.Select(file => dataFolder + "\\" + file).ToArray();

                _masterPageManager.CreateAsync(masterPage).Wait();
                _masterPageManager.CreateInitialVersion(masterPage.Id, seedFiles).Wait();

                masterPageMap.Add(item.Id, masterPage);
                index++;
            }

            return masterPageMap;
        }

        private IEnumerable<Page> CreateSitePagesV0(Site site, IEnumerable<PageMaster> masterPages)
        {
            var sitePages = new List<Page>();
            string dataFolder = null;
            IList<SiteTemplatePage> pagesToSeed = null;

            // Setting pages & data path either from template (if supplied) or using defaults
            if (_siteTemplate.Pages != null)
            {
                pagesToSeed = _siteTemplate.Pages;
                dataFolder = _coreOptions.FileSystemRoot + "\\" + _siteTemplate.DataFolder;
            }
            else
            { 
                dataFolder = _defaultDataFolder;
                pagesToSeed = GetDefaultPagesToSeed();
            }

            // Create Pages
            foreach(var seedInfo in pagesToSeed)
            {
                var master = masterPages.First(x => x.TemplateId == seedInfo.Template);
                var seedFiles = seedInfo.SeedData?.Select(file => dataFolder + "\\" + file);

                var sitePage = new Page
                {
                    Id = KeyGen.NewGuid(),
                    SiteId = site.Id,
                    PageMasterId = master.Id,
                    Title = seedInfo.Title,
                    Path = seedInfo.Path,
                    Type = PageType.Normal,
                    IsHomePage = seedInfo.Path == "/"
                };

                _connectDb.Pages.Add(sitePage);
                _connectDb.SaveChanges();

                var version = _pageManager.CreateInitialVersion(sitePage, seedFiles).Result;

                // manually seed if no seed files were included
                if(seedFiles == null)
                {
                    if (_pageTemplates[0].Id == master.TemplateId)
                    {
                        ManuallySeedHomePage(sitePage, version);
                    }
                    else
                    {
                        ManuallySeedSecondaryPage(sitePage, version);
                    }
                }

                sitePages.Add(sitePage);
            }

            return sitePages;
        }

        private Dictionary<string, Page> CreateSitePagesV1(Site site, Dictionary<string, PageMaster> masterPageMap)
        {
            var sitePageMap = new Dictionary<string, Page>();

            string dataFolder = null;
            IList<SiteTemplatePage> pagesToSeed = null;

            // Setting pages & data path either from template (if supplied) or using defaults
            if (_siteTemplate.Pages != null)
            {
                pagesToSeed = _siteTemplate.Pages;
                dataFolder = _coreOptions.FileSystemRoot + "\\" + _siteTemplate.DataFolder;
            }
            else
            {
                dataFolder = _defaultDataFolder;
                pagesToSeed = GetDefaultPagesToSeed();
            }

            // Create Pages
            foreach (var item in pagesToSeed)
            {
                if (!masterPageMap.ContainsKey(item.Master))
                    throw new Exception($"Could not create SitePage {item.Id}. Invalid master page {item.Master}");

                var master = masterPageMap[item.Master];
                var seedFiles = item.SeedData?.Select(file => dataFolder + "\\" + file);

                var sitePage = new Page
                {
                    Id = KeyGen.NewGuid(),
                    SiteId = site.Id,
                    PageMasterId = master.Id,
                    Title = item.Title,
                    Path = item.Path,
                    Type = PageType.Normal,
                    IsHomePage = item.Path == "/"
                };

                _connectDb.Pages.Add(sitePage);
                _connectDb.SaveChanges();

                var version = _pageManager.CreateInitialVersion(sitePage, seedFiles).Result;

                // manually seed if no seed files were included
                if (seedFiles == null)
                {
                    if (_pageTemplates[0].Id == master.TemplateId)
                    {
                        ManuallySeedHomePage(sitePage, version);
                    }
                    else
                    {
                        ManuallySeedSecondaryPage(sitePage, version);
                    }
                }

                sitePageMap.Add(item.Id, sitePage);
            }

            return sitePageMap;
        }

        private void CreateSiteNavigationV0(Site site, IEnumerable<Page> sitePages)
        {
            // Create the default site menu (scope = main)
            var navigationMenu = new NavigationMenu()
            {
                Id = KeyGen.NewGuid().ToString(),
                Title = "Top Navigation Menu",
                SiteId = site.Id,
                Scope = "main",
                MenuItems = new List<NavigationMenuItem>()
            };

            // create a link for each top level public page (not private)
            int order = 1;
            var orderedPages = sitePages.Where(x => 
                    x.ParentPageId == null && x.IsPrivate == false
                )
                .OrderBy(x => x.Path)
                .ThenBy(x => x.Title)
                .ToList();


            foreach (var page in orderedPages)
            {
                var menuItem = new NavigationMenuItem
                {
                    Id = KeyGen.NewGuid().ToString(),
                    Title = page.Title,
                    ContentType = "PageLink",
                    ContentId = page.Id,
                    Order = order
                };

                navigationMenu.MenuItems.Add(menuItem);
                order += 1;
            }

            var isNotificationsEnabled = GetProductContext(site.ClientId).Features.Get(FeatureId.Notifications)?.GetSettingValue<bool>("enabled") ?? false;

            if (isNotificationsEnabled)
            {
                // adding a notify me link here temporarily
                var notifyMeLink = new NavigationMenuItem()
                {
                    Id = KeyGen.NewGuid().ToString(),
                    Title = "NotifyMe",
                    ContentType = NavigationMenuItemType.ExternalURL,
                    ExternalURL = "/sys/notifyme/start",
                    Order = order
                };

                navigationMenu.MenuItems.Add(notifyMeLink);
            }

            _connectDb.NavigationMenu.Add(navigationMenu);
            _connectDb.SaveChanges();
        }

        private void CreateSiteNavigationV1(Site site, IDictionary<string, Page> sitePageMap)
        {
            // TODO: Export and read menus from template. Ran out of time per deadline due
            //       so just creating the main menu using the old process
            var sitePages = sitePageMap.Select(x => x.Value).ToList();

            CreateSiteNavigationV0(site, sitePages);
        }


        private void CreateSiteRoles(Site site)
        {
            // Exit if no security pool
            if (site.SecurityPoolId == null)
                return;

            if (!_identityDb.SecurityPools.Any(x => x.PoolId == site.SecurityPoolId))
                return;


            // create the roles
            var roles = new List<Role>
            {
                new Role()
                {
                    PoolId = site.SecurityPoolId,
                    Name = ROLE_SITE_ADMIN,
                    IsLocked = true,
                    RoleClaims = new RoleClaim[]
                    {
                        new RoleClaim { ClaimType = SiteClaimTypes.SitePrimaryAdmin, ClaimValue = site.Id },
                        new RoleClaim { ClaimType = SiteClaimTypes.SiteLibraryOwner, ClaimValue = site.Id },
                    }
                },
                new Role()
                {
                    PoolId = site.SecurityPoolId,
                    Name = ROLE_SITE_STAFF,
                    IsLocked = false,
                    RoleClaims = new RoleClaim[]
                    {
                        new RoleClaim { ClaimType = SiteClaimTypes.SiteLibraryReader, ClaimValue = site.Id },
                        new RoleClaim { ClaimType = UserClaimTypes.PersonalPageAuthor, ClaimValue = site.Id },
                        new RoleClaim { ClaimType = UserClaimTypes.PersonalPagePublish, ClaimValue = site.Id },
                        new RoleClaim { ClaimType = UserClaimTypes.PersonalGroupOwner, ClaimValue = site.Id }
                    }
                },
                new Role()
                {
                    PoolId = site.SecurityPoolId,
                    Name = ROLE_SITE_USERS,
                    IsLocked = false,
                    IsDefault = true,
                    RoleClaims = new RoleClaim[]
                    {
                        new RoleClaim { ClaimType = UserClaimTypes.NotificationSubscriber, ClaimValue = site.ClientId }
                    }
                },
            };

            _identityDb.Roles.AddRange(roles);
            _identityDb.SaveChanges();

        }

        private void CreateSiteCss(Site site)
        {
            var seedFilePath = _coreOptions.FileSystemRoot + "\\" + _siteTemplate.DataFolder + "\\site.css";
            string content = "/* empty */";

            if(File.Exists(seedFilePath))
                content = File.ReadAllText(seedFilePath);

            _connectDb.SiteSettings.Add(new SiteSetting
            {
                SiteId = site.Id,
                FieldName = SiteSettingKeys.SITE_CSS,
                Value = content
            });

            _connectDb.SaveChanges();
        }

        private void EnsureSecurityPoolExists(Site site)
        {
            Identity.Models.SecurityPool securityPool = null;

            if(site.SecurityPoolId != null)
            {
                securityPool = _identityDb.SecurityPools.FirstOrDefault(x => x.PoolId == site.SecurityPoolId);
            }

            if(securityPool == null)
            {
                var directory = _identityDb.Directories.FirstOrDefault(x => x.TenantId == site.Client.TenantKey);

                securityPool = new Identity.Models.SecurityPool()
                {
                    PoolId = site.SecurityPoolId ?? KeyGen.NewGuid(),
                    PoolType = PoolType.Site,
                    Name = site.Title + " Security Pool",
                    TenantId = site.Client.TenantKey,
                    ParentPoolId = site.Client.SecurityPoolId,
                    DirectoryMap = new DirectoryMap[]
                    {
                        new DirectoryMap { DirectoryId = directory.Id  }
                    }
                };

                _identityDb.SecurityPools.Add(securityPool);
                _identityDb.SaveChanges();
             
                site.SecurityPoolId = securityPool.PoolId;
                _connectDb.SaveChanges();
            }

        }

        private void ManuallySeedHomePage(Page page, IContentVersion versionInfo)
        {
            var contentTree = _connectDb.ContentTrees.First(x =>
                x.ContentId == page.Id
                && x.VersionCode == versionInfo.VersionCode
            );

            var treeBuilder = _contentManager.CreateTreeBuilder(contentTree);

            treeBuilder.AddContent(x => {
                x.WidgetType = "image";
                x.ModelName = "seed1.image1";
                x.Style = new ContentStyle
                {
                    FullWidth = true,
                    BackgroundClass = SiteTemplateConstants.Backgrounds.Default,
                };
             });

            // 4 images in a grid
            treeBuilder
                .AddRootLayout(x => {
                    x.LayoutType = "zone-4";
                    x.Style = new ContentStyle
                    {
                        BackgroundClass = SiteTemplateConstants.Backgrounds.Default,
                        PaddingTop = SiteTemplateConstants.RootContentPaddingTop,
                        PaddingBottom = SiteTemplateConstants.RootContentPaddingBottom
                    };
                })
                .AddChildContent("cell-1", x => {
                    x.WidgetType = "image";
                    x.ModelName = "seed1.thumb1";
                })
                .AddChildContent("cell-2", x => {
                    x.WidgetType = "image";
                    x.ModelName = "seed1.thumb2";
                })
                .AddChildContent("cell-3", x => {
                    x.WidgetType = "image";
                    x.ModelName = "seed1.thumb3";
                })
                .AddChildContent("cell-4", x => {
                    x.WidgetType = "image";
                    x.ModelName = "seed1.thumb4";
                });


            // Hero unit
            treeBuilder
               .AddRootLayout(x => {
                   x.LayoutType = "zone-1";
                   x.Style = new ContentStyle
                   {
                       BackgroundClass = SiteTemplateConstants.Backgrounds.Fancy,
                       PaddingTop = SiteTemplateConstants.RootContentPaddingTop,
                       PaddingBottom = SiteTemplateConstants.RootContentPaddingBottom
                   };
               })
               .AddChildContent("cell-1", x => {
                    x.WidgetType = "hero";
                    x.ModelName = "seed1.welcome";
                });

            // 4 more images in a grid
            treeBuilder
               .AddRootLayout(x => {
                   x.LayoutType = "zone-4";
                   x.Style = new ContentStyle
                   {
                       BackgroundClass = SiteTemplateConstants.Backgrounds.Contrast,
                       PaddingTop = SiteTemplateConstants.RootContentPaddingTop,
                       PaddingBottom = SiteTemplateConstants.RootContentPaddingBottom
                   };
               })
               .AddChildContent("cell-1", x => {
                    x.WidgetType = "image";
                    x.ModelName = "seed1.round1";
                })
               .AddChildContent("cell-2", x => {
                    x.WidgetType = "image";
                    x.ModelName = "seed1.round2";
                })
               .AddChildContent("cell-3", x => {
                    x.WidgetType = "image";
                    x.ModelName = "seed1.round3";
                })
               .AddChildContent("cell-4", x => {
                    x.WidgetType = "image";
                    x.ModelName = "seed1.round4";
                });

            treeBuilder.SaveChanges();
        }

        private void ManuallySeedSecondaryPage(Page page, IContentVersion versionInfo)
        {

            var contentTree = _connectDb.ContentTrees.First(x =>
                x.ContentId == page.Id
                && x.VersionCode == versionInfo.VersionCode
            );

            var treeBuilder = _contentManager.CreateTreeBuilder(contentTree);

            treeBuilder.AddRootContent(content => {
                content.WidgetType = "title";
                content.ModelName = "seed1.pagetitle";
            });

            treeBuilder.AddRootContent(content => {
                content.WidgetType = "alert";
                content.ModelName = "seed1.pagealert";
            });

            treeBuilder.AddRootContent(content => {
                content.WidgetType = "html";
                content.ModelName = "seed1.pagetext";
            });

            treeBuilder.SaveChanges();

        }

        private IList<SiteTemplatePage> GetDefaultPagesToSeed()
        {
            var jsonPagesPath = _defaultDataFolder + "\\pages.json";

            if (System.IO.File.Exists(jsonPagesPath))
            {
                return DeserializeJsonFile<List<SiteTemplatePage>>(jsonPagesPath);
            }

            // else
            return new List<SiteTemplatePage>
            {
                new SiteTemplatePage
                {
                    Title = "Home",
                    Path = "/",
                    Template = _pageTemplates[0].Id,
                },
                new SiteTemplatePage
                {
                    Title = "News",
                    Path = "/news",
                    Template = _pageTemplates[1].Id,
                },
                new SiteTemplatePage
                {
                    Title = "About",
                    Path = "/about",
                    Template = _pageTemplates[1].Id,
                },
                new SiteTemplatePage
                {
                    Title = "Contact",
                    Path = "/contact",
                    Template = _pageTemplates[1].Id,
                },
            };
        }

        private TDestination DeserializeJsonFile<TDestination>(string filepath)
        {
            if (!File.Exists(filepath))
                throw new FileNotFoundException(filepath);

            var stringJson = File.ReadAllText(filepath);

            if (string.IsNullOrEmpty(stringJson.Trim()))
                throw new NullReferenceException($"Could not deserialize empty json file {filepath}");

            return JsonConvert.DeserializeObject<TDestination>(stringJson);
        }

        private ProductContext GetProductContext(string clientId)
        {
            return _clientManager.GetDefaultProductContextAsync(clientId).Result;
        }
    }
}
