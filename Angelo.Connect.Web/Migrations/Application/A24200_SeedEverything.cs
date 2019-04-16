using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Abstractions;
using Angelo.Common.Migrations;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Documents;
using Angelo.Connect.Web.Data.Mock;
using Angelo.Identity;
using Angelo.Identity.Models;

namespace Angelo.Connect.Web.Migrations.Application
{
    public class A24200_SeedEverything : IAppMigration
    {
        public string Id { get; } = "A24200";

        public string Migration { get; } = "Seed Initial Data";

        private ConnectDbContext _connectDb;
        private IdentityDbContext _identityDb;
        private SitePublisher _sitePublisher;
        private UserManager _userManager;
        private bool _QA = false;
        private bool _Dev = false;
        private bool _UAT = false;
        private bool _Demo = false;

        public A24200_SeedEverything
        (
            ConnectDbContext connectDb,
            IdentityDbContext identityDb,
            SitePublisher sitePublisher,
            UserManager userManager,
            IHostingEnvironment environment
        )
        {
            _connectDb = connectDb;
            _identityDb = identityDb;
            _sitePublisher = sitePublisher;
            _userManager = userManager;

            _QA = environment.IsEnvironment("qa");
            _Dev = environment.IsEnvironment("Development");
            _UAT = environment.IsEnvironment("uat");
            _Demo = environment.IsEnvironment("demo");
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var dataExists = await _connectDb.Clients.AnyAsync();

            // Skip seeding if any customers exist
            if (dataExists)
                return MigrationResult.Skipped("Customer data detected. Exiting now.");

            // Root Configuration
            SeedProductInformation();
            SeedWirelessProviders();

            // Clients, Sites, Content, & Roles
            SeedPcMacClientAndSites();
            SeedBocaClientAndSites();
            SeedWemoClientAndSites();

            SeedCorpSecurityPool();


            // Test Users
            SeedPcMacUsers();
            SeedBocaUsers();
            SeedWemoUsers();


            return MigrationResult.Success("Initial data seeded");
        }

        private void SeedCorpSecurityPool()
        {

            var tenant = _identityDb.Tenants.First(x => x.Key == DbKeys.ClientTenantKeys.PcMac);
            var corpClaimValue = ConnectCoreConstants.CorporateId;

            // Create Security Pool for Corporate Roles
            _identityDb.SecurityPools.Add(new Angelo.Identity.Models.SecurityPool
            {
                PoolId = DbKeys.PoolIds.PcMacCorp,
                Name = "PcMac CMS Owner Pool",
                TenantId = tenant.Id,
                PoolType = PoolType.Corporate,
            });

            _identityDb.SaveChanges();

            // Create the Roles & Claims
            _identityDb.Roles.AddRange(new Role[]
            {
                new Role()
                {
                    PoolId = DbKeys.PoolIds.PcMacCorp, Name = DbKeys.RoleNames.CorpAdmins, IsLocked = true,
                    RoleClaims = new RoleClaim[]
                    {
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpPrimaryAdmin, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = ClientClaimTypes.PrimaryAdmin, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = SiteClaimTypes.SitePrimaryAdmin, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = UserClaimTypes.PersonalLibraryOwner, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = UserClaimTypes.PersonalGroupOwner, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpUser, ClaimValue = corpClaimValue }
                    }
                },
                new Role()
                {
                    PoolId = DbKeys.PoolIds.PcMacCorp, Name = DbKeys.RoleNames.CorpSupport, IsLocked = true,
                    RoleClaims = new RoleClaim[]
                    {
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpProductsRead, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpCustomersRead, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = ClientClaimTypes.PrimaryAdmin, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = SiteClaimTypes.SitePrimaryAdmin, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpUser, ClaimValue = corpClaimValue }
                    }
                },
                new Role()
                {
                    PoolId = DbKeys.PoolIds.PcMacCorp, Name = DbKeys.RoleNames.CorpAccounting, IsLocked = true,
                    RoleClaims = new RoleClaim[]
                    {
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpProductsRead, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpProductsAssign, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpCustomersCreate, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpCustomersRead, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpCustomersEdit, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpCustomersDelete, ClaimValue = corpClaimValue },
                        new RoleClaim { ClaimType = CorpClaimTypes.CorpUser, ClaimValue = corpClaimValue }
                    }
                },
            });

            _identityDb.SaveChanges();
        }

        private void SeedProductInformation()
        {
            //
            // Product Categories
            //
            _connectDb.ProductCategories.AddRange(
                new List<ProductCategory>()
                {
                    new ProductCategory(){ Id = DbKeys.ProductCategoryIds.Angelo, Name = "Angelo"},
                    new ProductCategory(){ Id = DbKeys.ProductCategoryIds.Business, Name = "Business"},
                    new ProductCategory(){ Id = DbKeys.ProductCategoryIds.Education, Name = "Education"},
                    new ProductCategory(){ Id = DbKeys.ProductCategoryIds.Faith, Name = "Faith"},
                    new ProductCategory(){ Id = DbKeys.ProductCategoryIds.ChildCare, Name = "Child Care"},
                }
            );
            _connectDb.SaveChanges();


            //
            // Base Products
            //
            _connectDb.Products.Add(new Product
            {
                Id = DbKeys.ProductIds.Silver,
                CategoryId = DbKeys.ProductCategoryIds.Angelo,
                Name = "Angelo Silver Plan",
                Description = "Great introduction to our ultra-modern Angelo product.",
                SchemaFile = "/schemas/products/Product-1-Silver.json",
                Active = true
            });
            _connectDb.Products.Add(new Product
            {
                Id = DbKeys.ProductIds.Gold,
                CategoryId = DbKeys.ProductCategoryIds.Angelo,
                Name = "Angelo Gold Plan",
                Description = "Our ultra-modern Angelo product with some extra stuff.",
                SchemaFile = "/schemas/products/Product-2-Gold.json",
                Active = true
            });
            _connectDb.Products.Add(new Product
            {
                Id = DbKeys.ProductIds.Platinum,
                CategoryId = DbKeys.ProductCategoryIds.Angelo,
                Name = "Angelo Platinum Plan",
                Description = "Our ultra-modern Angelo product with even more stuff.",
                SchemaFile = "/schemas/products/Product-3-Platinum.json",
                Active = true
            });
            _connectDb.Products.Add(new Product
            {
                Id = DbKeys.ProductIds.Diamond,
                CategoryId = DbKeys.ProductCategoryIds.Angelo,
                Name = "Angelo Diamond Plan",
                Description = "Our ultra-modern Angelo product with some unlimited awesomeness.",
                SchemaFile = "/schemas/products/Product-4-Diamond.json",
                Active = true
            });
            _connectDb.SaveChanges();


            //
            // Product Add-ons & Availabiliy Mappings
            //
            _connectDb.ProductAddOns.Add(new ProductAddOn
            {
                Id = DbKeys.ProductAddOnIds.ExtraStorage,
                Name = "Storage: +1000MB",
                Description = "Adds an additional 1000MB shared storage.",
                SchemaFile = "/schemas/products/Addon-2-ExtraStorage.json",
                Active = true,
                ProductAddOnLinks = new ProductAddOnLink[]
                {
                    new ProductAddOnLink { ProductId = DbKeys.ProductIds.Silver },
                    new ProductAddOnLink { ProductId = DbKeys.ProductIds.Gold },
                    new ProductAddOnLink { ProductId = DbKeys.ProductIds.Platinum },
                    new ProductAddOnLink { ProductId = DbKeys.ProductIds.Diamond },
                }
            });
            _connectDb.ProductAddOns.Add(new ProductAddOn
            {
                Id = DbKeys.ProductAddOnIds.ExtraTemplate,
                Name = "Site Template: Empty Template",
                Description = "Adds site template: Empty Template",
                SchemaFile = "/schemas/products/Addon-1-EmptyTemplate.json",
                Active = true,
                ProductAddOnLinks = new ProductAddOnLink[]
                {
                    new ProductAddOnLink { ProductId = DbKeys.ProductIds.Silver },
                    new ProductAddOnLink { ProductId = DbKeys.ProductIds.Gold },
                    new ProductAddOnLink { ProductId = DbKeys.ProductIds.Platinum },
                    new ProductAddOnLink { ProductId = DbKeys.ProductIds.Diamond },
                }

            });
            _connectDb.SaveChanges();

        }

        private void SeedWirelessProviders()
        {
            // Wireless Providers
            _identityDb.WirelessProviders.AddRange(new WirelessProvider[]{
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.Verizon,        Name = "Verizon Wireless",      SmsDomain = "vtext.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.Sprint,         Name = "Sprint",                SmsDomain = "messaging.sprintpcs.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.TMobile,        Name = "T-Mobile",              SmsDomain = "tmomail.net" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.ATT,            Name = "AT&T",                  SmsDomain = "txt.att.net" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.Virgin,         Name = "Virgin Mobile",         SmsDomain = "vmobl.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.Trackfone,      Name = "Tracfone",              SmsDomain = "mmst5.tracfone.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.MetroPCS,       Name = "Metro PCS",             SmsDomain = "mymetropcs.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.BoostMobile,    Name = "Boost Mobile",           SmsDomain = "myboostmobile.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.Cricket,        Name = "Cricket",               SmsDomain = "mms.cricketwireless.net" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.PTel,           Name = "PTel",                  SmsDomain = "ptel.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.RepublicWireless, Name = "Republic Wireless",  SmsDomain = "text.republicwireless.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.GoogleFi,       Name = "Google FI",             SmsDomain = "msg.fi.google.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.Suncom,         Name = "Suncom",                SmsDomain = "tms.suncom.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.Ting,           Name = "Ting",                  SmsDomain = "message.ting.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.USCellular,     Name = "US Cellular",           SmsDomain = "email.uscc.net" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.ConsumerCellular, Name = "Consume Cellular",   SmsDomain = "cingularme.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.CSpire,         Name = "C Spire",               SmsDomain = "cspire1.com" },
                new WirelessProvider() { Id = DbKeys.WirelessProviderIds.PagePlus,       Name = "Page Plus",             SmsDomain = "vtext.com" }
            });

            _identityDb.SaveChanges();
        }

        private void SeedPcMacClientAndSites()
        {
            //
            // Create as a standard client
            //
            var client_PcMac = CreateClient(DbKeys.DirectoryIds.PcMac, new Client()
            {
                Id = DbKeys.ClientIds.PcMac,
                Name = "PC|Mac",
                ShortName = "PcMac",
                Active = true,
                AnniversaryDate = DateTime.UtcNow,
                SecurityPoolId = DbKeys.PoolIds.PcMacClient,
                TenantKey = DbKeys.ClientTenantKeys.PcMac,
                ClientProductApps = new List<ClientProductApp>()
                {
                    new ClientProductApp()
                    {
                        Id = DbKeys.ClientProductAppIds.PcMacApp1,
                        ProductId = DbKeys.ProductIds.Silver,
                        Title = LookupProductTitle(DbKeys.ProductIds.Silver),
                        SubscriptionStartUTC = new DateTime(2016,1,1),
                        SubscriptionType = ProductSubscriptionType.Monthly,
                        MaxSiteCount = 5,
                        AddOns = new List<ClientProductAddOn>
                        {
                            new ClientProductAddOn { ProductAddOnId = DbKeys.ProductAddOnIds.ExtraTemplate }
                        }
                    }
                }
            });

            // 
            // Configure Ldap
            // 
            _identityDb.LdapDomains.Add(new LdapDomain()
            {
                Id = Guid.NewGuid().ToString("N"),
                DirectoryId = DbKeys.DirectoryIds.PcMac,
                Domain = "mcpss2",
                Host = "10.0.1.81",
                LdapBaseDn = "dc=mcpss2,dc=com",
                Password = "n0b0dykn0w$",
                User = "ldap"
            });
            _identityDb.SaveChanges();

            //
            // Create and seed sites
            //

            // inspiration
            CreateSite(DbKeys.DirectoryIds.PcMac, client_PcMac, new Site()
            {
                Id = DbKeys.SiteIds.PcMac_Site1,
                ClientId = DbKeys.ClientIds.PcMac,
                ClientProductAppId = DbKeys.ClientProductAppIds.PcMacApp1,
                SiteTemplateId = DbKeys.SiteTemplateIds.Inspiration,
                SecurityPoolId = DbKeys.PoolIds.PcMacSite1,
                TenantKey = DbKeys.SiteTenantKeys.PcMac_Site1,
                Title = "Inspiration Site",
                Banner = "/img/sis.png",
                ThemeId = DbKeys.ThemeIds.Default,
                Published = true,
                Domains = new List<SiteDomain>()
                {
                    new SiteDomain(){ DomainKey = "schoolinsites.com" },
                    new SiteDomain(){ DomainKey = "sis.pcmac.org" },
                    new SiteDomain(){ DomainKey = "localhost:60000", IsDefault = _Dev },
                    new SiteDomain(){ DomainKey = "connect.qa.pcmac.org"},
                    new SiteDomain(){ DomainKey = "inspirationsdqa.schoolinsites.com", IsDefault = _QA },
                    new SiteDomain(){ DomainKey = "inspirationsduat.schoolinsites.com", IsDefault = _UAT },
                    new SiteDomain(){ DomainKey = "inspirationsddemo.schoolinsites.com", IsDefault = _Demo }
                }
            });

            // guidance
            CreateSite(DbKeys.DirectoryIds.PcMac, client_PcMac, new Site()
            {
                Id = DbKeys.SiteIds.PcMac_Site2,
                ClientId = DbKeys.ClientIds.PcMac,
                ClientProductAppId = DbKeys.ClientProductAppIds.PcMacApp1,
                SiteTemplateId = DbKeys.SiteTemplateIds.Guidance,
                SecurityPoolId = DbKeys.PoolIds.PcMacSite2,
                TenantKey = DbKeys.SiteTenantKeys.PcMac_Site2,
                Title = "Guidance High School",
                Banner = "/img/sis.png",
                ThemeId = DbKeys.ThemeIds.Default,
                Published = true,
                Domains = new List<SiteDomain>()
                {
                    new SiteDomain(){ DomainKey = "schoolinsites2.com" },
                    new SiteDomain(){ DomainKey = "guidancehsqa.schoolinsites.com", IsDefault = _QA },
                    new SiteDomain(){ DomainKey = "localhost:60001", IsDefault = _Dev},
                    new SiteDomain(){ DomainKey = "guidancehsuat.schoolinsites.com", IsDefault = _UAT},
                    new SiteDomain(){ DomainKey = "guidancehsdemo.schoolinsites.com", IsDefault = _Demo }
                }
            });

            // cityscape
            CreateSite(DbKeys.DirectoryIds.PcMac, client_PcMac, new Site()
            {
                Id = DbKeys.SiteIds.PcMac_Site3,
                ClientId = DbKeys.ClientIds.PcMac,
                ClientProductAppId = DbKeys.ClientProductAppIds.PcMacApp1,
                SiteTemplateId = DbKeys.SiteTemplateIds.CityScape,
                SecurityPoolId = DbKeys.PoolIds.PcMacSite3,
                TenantKey = DbKeys.SiteTenantKeys.PcMac_Site3,
                Title = "City Scape",
                Banner = "/img/sis.png",
                ThemeId = DbKeys.ThemeIds.Default,
                Published = true,
                Domains = new List<SiteDomain>()
                {
                    new SiteDomain(){ DomainKey = "schoolinsites3.com" },
                    new SiteDomain(){ DomainKey = "sis3.pcmac.org" },
                    new SiteDomain(){ DomainKey = "sis3qa.pcmac.org", IsDefault = _QA },
                    new SiteDomain(){ DomainKey = "localhost:60002", IsDefault = _Dev},
                    new SiteDomain(){ DomainKey = "sis3uat.pcmac.org", IsDefault = _UAT},
                    new SiteDomain(){ DomainKey = "sis3demo.pcmac.org", IsDefault = _Demo}
                }
            });
        }

        private void SeedBocaClientAndSites()
        {
            //
            // Create a standard client
            //
            var client_Boca = CreateClient(DbKeys.DirectoryIds.Boca, new Client()
            {
                Id = DbKeys.ClientIds.Boca,
                Name = "Boca Raton Public Schools",
                ShortName = "Boca Schools",
                Active = true,
                AnniversaryDate = DateTime.UtcNow,
                SecurityPoolId = DbKeys.PoolIds.BocaClient,
                TenantKey = DbKeys.ClientTenantKeys.Boca,
                ClientProductApps = new List<ClientProductApp>()
                {
                    new ClientProductApp()
                    {
                        Id = DbKeys.ClientProductAppIds.BocaApp1,
                        ProductId = DbKeys.ProductIds.Gold,
                        Title = LookupProductTitle(DbKeys.ProductIds.Gold),
                        SubscriptionStartUTC = new DateTime(2010,9,1),
                        SubscriptionType = ProductSubscriptionType.Annual,
                        MaxSiteCount = 5,
                        AddOns = new List<ClientProductAddOn>
                        {
                            new ClientProductAddOn { ProductAddOnId = DbKeys.ProductAddOnIds.ExtraTemplate },
                            new ClientProductAddOn { ProductAddOnId = DbKeys.ProductAddOnIds.ExtraStorage },
                        }
                    }
                }
            });


            // 
            // Create and seed sites
            //

            // District Site
            CreateSite(DbKeys.DirectoryIds.Boca, client_Boca, new Site()
            {
                Id = DbKeys.SiteIds.Boca_District,
                ClientId = DbKeys.ClientIds.Boca,
                ClientProductAppId = DbKeys.ClientProductAppIds.BocaApp1,
                SiteTemplateId = DbKeys.SiteTemplateIds.Inspiration,
                SecurityPoolId = DbKeys.PoolIds.BocaDistrict,
                TenantKey = DbKeys.SiteTenantKeys.Boca_District,
                Title = "Boca Raton School District",
                Banner = "/img/banner/mcpss.jpg",
                ThemeId = DbKeys.ThemeIds.Default,
                Published = true,
                Domains = new List<SiteDomain>()
                {
                    new SiteDomain(){ DomainKey = "localhost:60010", IsDefault = _Dev},
                    new SiteDomain(){ DomainKey = "bocadqa.schoolinsites.com", IsDefault = _QA },
                    new SiteDomain(){ DomainKey = "bocaduat.schoolinsites.com", IsDefault = _UAT },
                    new SiteDomain(){ DomainKey = "bocaddemo.schoolinsites.com", IsDefault = _Demo }
                }
            });


        }

        private void SeedWemoClientAndSites()
        {
            //
            // Create a standard client
            //
            var client_Wemo = CreateClient(DbKeys.DirectoryIds.Wemo, new Client()
            {
                Id = DbKeys.ClientIds.Wemo,
                Name = "West Mobile Baptist Church",
                ShortName = "Wemo Baptist",
                Active = true,
                AnniversaryDate = DateTime.UtcNow,
                SecurityPoolId = DbKeys.PoolIds.WemoClient,
                TenantKey = DbKeys.ClientTenantKeys.Wemo,
                ClientProductApps = new List<ClientProductApp>()
                {
                    new ClientProductApp()
                    {
                        Id = DbKeys.ClientProductAppIds.WemoApp1,
                        ProductId = DbKeys.ProductIds.Gold,
                        Title = LookupProductTitle(DbKeys.ProductIds.Gold),
                        SubscriptionStartUTC = new DateTime(2015,1,1),
                        SubscriptionType = ProductSubscriptionType.Monthly,
                        MaxSiteCount = 5,
                        AddOns = new List<ClientProductAddOn>
                        {
                            new ClientProductAddOn { ProductAddOnId = DbKeys.ProductAddOnIds.ExtraStorage },
                        }
                    },
                    new ClientProductApp()
                    {
                        Id = DbKeys.ClientProductAppIds.WemoApp2,
                        ProductId = DbKeys.ProductIds.Silver,
                        Title = LookupProductTitle(DbKeys.ProductIds.Silver),
                        SubscriptionStartUTC = new DateTime(2017,1,1),
                        SubscriptionType = ProductSubscriptionType.Monthly,
                        MaxSiteCount = 10
                    }
                }
            });

            //
            // Create and Seed Sites
            //
            CreateSite(DbKeys.DirectoryIds.Wemo, client_Wemo, new Site()
            {
                Id = DbKeys.SiteIds.Wemo_Church,
                ClientId = DbKeys.ClientIds.Wemo,
                ClientProductAppId = DbKeys.ClientProductAppIds.WemoApp1,
                SiteTemplateId = DbKeys.SiteTemplateIds.Essential,
                SecurityPoolId = DbKeys.PoolIds.WemoChurch,
                TenantKey = DbKeys.SiteTenantKeys.Wemo_Church,
                Title = "West Mobile Baptist Church",
                Banner = "/img/banner/wemo-church.png",
                ThemeId = DbKeys.ThemeIds.Default,
                Published = true,
                Domains = new List<SiteDomain>()
                {
                    new SiteDomain(){ DomainKey = "wemobaptist.com" },
                    new SiteDomain(){ DomainKey = "localhost:60030", IsDefault = true }
                },
            });

            CreateSite(DbKeys.DirectoryIds.Wemo, client_Wemo, new Site()
            {
                Id = DbKeys.SiteIds.Wemo_Kids,
                ClientId = DbKeys.ClientIds.Wemo,
                ClientProductAppId = DbKeys.ClientProductAppIds.WemoApp2,
                SiteTemplateId = DbKeys.SiteTemplateIds.CityScape,
                SecurityPoolId = DbKeys.PoolIds.WemoKids,
                TenantKey = DbKeys.SiteTenantKeys.Wemo_Kids,
                Title = "West Mobile Child Development Center",
                ThemeId = DbKeys.ThemeIds.Hero,
                Published = true,
                Domains = new List<SiteDomain>()
                    {
                        new SiteDomain(){ DomainKey = "wemocdc.com" },
                        new SiteDomain(){ DomainKey = "cdc.wemobaptist.com" },
                        new SiteDomain(){ DomainKey = "localhost:60031", IsDefault = true }
                    }
            });

        }

        private void SeedPcMacUsers()
        {
            var tenant = _identityDb.Tenants.First(x => x.Key == DbKeys.ClientTenantKeys.PcMac);

            #region Admin
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Admin,
                    UserName = "admin",
                    Email = "admin@admin.com",
                    PasswordHash = "Admin.1",
                    FirstName = "Corporate",
                    LastName = "Admin",
                    DisplayName = "Corporate Admin",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "999-999-9999"
                },
                new Claim[] {
                        new Claim(ClaimType.FirstName, "Corporate"),
                        new Claim(ClaimType.LastName, "Admin"),
                        new Claim(ClaimType.DisplayName, "Corporate Admin"),
                        new Claim(ClaimType.Gender, "M"),
                        new Claim(ClaimType.AltId, "010-02-3456"),
                        new Claim("hire_date", "12/14/2015"),

                        // library claims
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site1),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site2),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site3),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, ConnectCoreConstants.CorporateId),
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp,  DbKeys.RoleNames.CorpAdmins)
                }
            );
            #endregion

            #region Support
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.AdminSupport,
                    UserName = "admin_support",
                    Email = "admin_support@admin.com",
                    PasswordHash = "Admin.2",
                    FirstName = "Admin",
                    LastName = "Support",
                    DisplayName = "Admin Support",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "999-999-9999"
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Admin"),
                    new Claim(ClaimType.LastName, "Support"),
                    new Claim(ClaimType.DisplayName, "Admin Support"),
                    new Claim(ClaimType.Gender, "F"),
                    new Claim(ClaimType.Birthday, "03/24/1977")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpSupport)
                }
            );

            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.AdminSupport2,
                    UserName = "admin_support2",
                    Email = "admin_support2@admin.com",
                    PasswordHash = "Admin.4",
                    FirstName = "Admin",
                    LastName = "Support 2",
                    DisplayName = "Admin Support 2",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "999-999-9999"
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Admin"),
                    new Claim(ClaimType.LastName, "Support 2"),
                    new Claim(ClaimType.DisplayName, "Admin Support 2"),
                    new Claim(ClaimType.Gender, "F"),
                    new Claim(ClaimType.Birthday, "03/24/1977")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpSupport)
                }
            );
            #endregion

            #region Accounting
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.AdminAccounting,
                    UserName = "admin_Accounting",
                    Email = "admin_Accounting@admin.com",
                    PasswordHash = "Admin.3",
                    FirstName = "Admin",
                    LastName = "Accounting",
                    DisplayName = "Admin Accounting",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "999-999-9999",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Admin"),
                    new Claim(ClaimType.LastName, "Accounting"),
                    new Claim(ClaimType.DisplayName, "Admin Accounting"),
                    new Claim(ClaimType.Gender, "F"),
                    new Claim(ClaimType.Birthday, "03/24/1977")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpAccounting),
                }
           );

            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.AdminAccounting2,
                    UserName = "admin_Accounting2",
                    Email = "admin_Accounting2@admin.com",
                    PasswordHash = "Admin.5",
                    FirstName = "Admin",
                    LastName = "Accounting 2",
                    DisplayName = "Admin Accounting 2",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "999-999-9999",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Admin"),
                    new Claim(ClaimType.LastName, "Accounting 2"),
                    new Claim(ClaimType.DisplayName, "Admin Accounting 2"),
                    new Claim(ClaimType.Gender, "F"),
                    new Claim(ClaimType.Birthday, "03/24/1977")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpAccounting),
                }
           );
            #endregion

            #region Sales
            /* CreateUser(
                 new User()
                 {
                     TenantId = tenant.Id,
                     DirectoryId = DbKeys.DirectoryIds.PcMac,
                     Id = DbKeys.UserIds.AdminSales,
                     UserName = "admin_Sales",
                     Email = "admin_sales@admin.com",
                     PasswordHash = "Sales.1",
                     FirstName = "Admin",
                     LastName = "Sales",
                     DisplayName = "Admin Sales",
                     Title = "",
                     Suffix = "",
                     BirthDate = DateTime.MinValue,
                     PhoneNumber = "999-999-9999"
                 },
                 new Claim[]
                 {
                     new Claim(ClaimType.FirstName, "Admin"),
                     new Claim(ClaimType.LastName, "Sales"),
                     new Claim(ClaimType.DisplayName, "Admin Sales"),
                     new Claim(ClaimType.Gender, "M"),
                     new Claim(ClaimType.Birthday, "03/24/1977")
                 },
                 new string[]
                 {
                     LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.Sales),
                 }
            );

             CreateUser(
                 new User()
                 {
                     TenantId = tenant.Id,
                     DirectoryId = DbKeys.DirectoryIds.PcMac,
                     Id = DbKeys.UserIds.AdminSales2,
                     UserName = "admin_Sales2",
                     Email = "admin_sales2@admin.com",
                     PasswordHash = "Sales.2",
                     FirstName = "Admin",
                     LastName = "Sales 2",
                     DisplayName = "Admin Sales 2",
                     Title = "",
                     Suffix = "",
                     BirthDate = DateTime.MinValue,
                     PhoneNumber = "999-999-9999"
                 },
                 new Claim[]
                 {
                     new Claim(ClaimType.FirstName, "Admin"),
                     new Claim(ClaimType.LastName, "Sales 2"),
                     new Claim(ClaimType.DisplayName, "Admin Sales 2"),
                     new Claim(ClaimType.Gender, "F"),
                     new Claim(ClaimType.Birthday, "03/24/1977")
                 },
                 new string[]
                 {
                     LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.Sales),
                 }
            ); */
            #endregion

            #region Gabi
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Gabi,
                    UserName = "gabi",
                    Email = "gabi@schoolinsites.com",
                    PasswordHash = "Gabi.Admin.1",
                    FirstName = "Gabi",
                    LastName = "Constantine",
                    DisplayName = "Gabi",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "999-999-9999",
                },
                new Claim[] {
                        new Claim(ClaimType.Gender, "M"),

                        // library claims
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site1),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site2),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site3),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, ConnectCoreConstants.CorporateId),
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp,  DbKeys.RoleNames.CorpAdmins)
                }
            );
            #endregion

            #region Dave
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Dave,
                    UserName = "david",
                    Email = "d.constantine@schoolinsites.com",
                    PasswordHash = "David.Admin.1",
                    FirstName = "David",
                    LastName = "Constantine",
                    DisplayName = "David",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "999-999-9999",
                },
                new Claim[] {
                        new Claim(ClaimType.Gender, "M"),

                        // library claims
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site1),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site2),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, DbKeys.SiteIds.PcMac_Site3),
                        new Claim(SiteClaimTypes.SiteLibraryOwner, ConnectCoreConstants.CorporateId),
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp,  DbKeys.RoleNames.CorpAdmins)
                }
            );
            #endregion

            #region Michael
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Michael,
                    UserName = "Michael",
                    Email = "michael@pcmac.org",
                    PasswordHash = "Michael.1",
                    FirstName = "Michael",
                    LastName = "Johnson",
                    DisplayName = "Mike J.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "999-999-9999",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Michael"),
                    new Claim(ClaimType.LastName, "Johnson"),
                    new Claim(ClaimType.DisplayName, "Aries Dane"),
                    new Claim(ClaimType.Gender, "M"),
                    new Claim(PageClaimTypes.DesignPage, DbKeys.PageIds.PcMac_PcMac1_Home),
                    new Claim(PageClaimTypes.DesignPage, DbKeys.PageIds.PcMac_PcMac2_Home),
                    new Claim(PageClaimTypes.PublishPage, DbKeys.PageIds.PcMac_PcMac1_Home),
                    new Claim(PageClaimTypes.PublishPage, DbKeys.PageIds.PcMac_PcMac2_Home),
                    new Claim(PageClaimTypes.PageOwner, DbKeys.PageIds.PcMac_PcMac1_Home),
                    new Claim(PageClaimTypes.PageOwner, DbKeys.PageIds.PcMac_PcMac2_Home),
                },
                new string[]
                {
                    //
                }
            );
            #endregion

            #region Chris
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Chris,
                    UserName = "Chris",
                    Email = "chris@pcmac.org",
                    PasswordHash = "Chris.1",
                    FirstName = "Chris",
                    LastName = "Van",
                    DisplayName = "Chris V.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "889-444-4554",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Chris"),
                    new Claim(ClaimType.LastName, "Van"),
                    new Claim(ClaimType.DisplayName, "Chris Van"),
                    new Claim(ClaimType.Gender, "M")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpAdmins),
                   // LookupRoleId(DbKeys.PoolIds.PcMacSis, DbKeys.RoleNames.Employees),
                    //LookupRoleId(DbKeys.PoolIds.PcMacSis, DbKeys.RoleNames.Developers),
                }
            );
            #endregion

            #region Ricardo
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Ricardo,
                    UserName = "Ricardo",
                    Email = "ricardo@pcmac.org",
                    PasswordHash = "Ricardo.1",
                    FirstName = "Ricardo",
                    LastName = "Lopez",
                    DisplayName = "Ricardo L.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Ricardo"),
                    new Claim(ClaimType.LastName, "Lopez"),
                    new Claim(ClaimType.DisplayName, "Ricardo Lopez"),
                    new Claim(ClaimType.Gender, "M"),

                    // library claims
                    new Claim(SiteClaimTypes.SiteLibraryReader, DbKeys.SiteIds.PcMac_Site1)
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpAdmins),
                   // LookupRoleId(DbKeys.PoolIds.PcMacSis, DbKeys.RoleNames.Employees),
                    //LookupRoleId(DbKeys.PoolIds.PcMacSis, DbKeys.RoleNames.Developers),
                }
            );
            #endregion

            #region Sherry
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Sherry,
                    UserName = "Sherry",
                    Email = "sherry@schoolinsites.com",
                    PasswordHash = "Sherry.1",
                    FirstName = "Sherry",
                    LastName = "Harris",
                    DisplayName = "Sherry H.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Sherry"),
                    new Claim(ClaimType.LastName, "Harris"),
                    new Claim(ClaimType.DisplayName, "Sharry Harris"),
                    new Claim(ClaimType.Gender, "F")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpSupport),
                    //LookupRoleId(DbKeys.PoolIds.PcMacSis, DbKeys.RoleNames.Employees)
                }
            );
            #endregion

            #region Christie
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Christie,
                    UserName = "Christie",
                    Email = "christie@schoolinsites.com",
                    PasswordHash = "Christie.1",
                    FirstName = "Christie",
                    LastName = "Stuckey",
                    DisplayName = "Christie S.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Christie"),
                    new Claim(ClaimType.LastName, "Stuckey"),
                    new Claim(ClaimType.DisplayName, "Christie Stuckey"),
                    new Claim(ClaimType.Gender, "F")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpSupport),
                    //LookupRoleId(DbKeys.PoolIds.PcMacSis, DbKeys.RoleNames.Employees)
                }
            );
            #endregion

            #region Stephanie
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.Stephanie,
                    UserName = "Stephanie",
                    Email = "stephanie@schoolinsites.com",
                    PasswordHash = "Stephanie.1",
                    FirstName = "Stephanie",
                    LastName = "Jacobs",
                    DisplayName = "Stephanie J.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Stephanie"),
                    new Claim(ClaimType.LastName, "Jacobs"),
                    new Claim(ClaimType.DisplayName, "Stephanie Jacobs"),
                    new Claim(ClaimType.Gender, "F")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpAccounting),
                    LookupRoleId(DbKeys.PoolIds.PcMacCorp, DbKeys.RoleNames.CorpSupport),
                    //LookupRoleId(DbKeys.PoolIds.PcMacSis, DbKeys.RoleNames.Employees)
                }
            );
            #endregion

            #region Client and Sites Administrator Jonathan
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.JonathanG,
                    UserName = "JonathanG",
                    Email = "jonathang@inspirationsd.schoolinsites.com",
                    PasswordHash = "Jonathan.1",
                    FirstName = "Jonathan",
                    LastName = "Gardner",
                    DisplayName = "Jonathan G.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Jonathan"),
                    new Claim(ClaimType.LastName, "Gardner"),
                    new Claim(ClaimType.DisplayName, "Jonathan Gardner"),
                    new Claim(ClaimType.Gender, "M")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacClient, DbKeys.RoleNames.ClientAdmins),
                    //LookupRoleId(DbKeys.PoolIds.PcMacClient, DbKeys.RoleNames.ClientSiteAdmins),
                    //LookupRoleId(DbKeys.PoolIds.PcMacSis, DbKeys.RoleNames.DistrictStaff),
                    LookupRoleId(DbKeys.PoolIds.PcMacSite1, DbKeys.RoleNames.SiteAdmins),
                }
            );
            #endregion

            #region Baseball Coach
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.JohnnyB,
                    UserName = "Johnny",
                    Email = "johnnyb@inspirationsd.schoolinsites.com",
                    PasswordHash = "Johnny.1",
                    FirstName = "Johnny",
                    LastName = "Baseballcoach",
                    DisplayName = "Johnny B.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Johnny"),
                    new Claim(ClaimType.LastName, "Baseballcoach"),
                    new Claim(ClaimType.DisplayName, "Johnny Baseballcoach"),
                    new Claim(ClaimType.Gender, "M")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacSite1, DbKeys.RoleNames.SiteStaff)
                }
            );
            #endregion

            #region Baseball Player
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.BillyB,
                    UserName = "Billy",
                    Email = "billyb@inspirationsd.schoolinsites.com",
                    PasswordHash = "Billy.1",
                    FirstName = "Billy",
                    LastName = "Baseballplayer",
                    DisplayName = "Billy B.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Billy"),
                    new Claim(ClaimType.LastName, "Baseballplayer"),
                    new Claim(ClaimType.DisplayName, "Billy Baseballplayer"),
                    new Claim(ClaimType.Gender, "M")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacSite1, DbKeys.RoleNames.SiteUsers)
                }
            );
            #endregion

            #region Client Administrator Sophie Jordan
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.SophieJ,
                    UserName = "SophieJ",
                    Email = "sophiej@inspirationsd.schoolinsites.com",
                    PasswordHash = "Sophie.1",
                    FirstName = "Sophie",
                    LastName = "Jordan",
                    DisplayName = "Sophie J.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Sophie"),
                    new Claim(ClaimType.LastName, "Jordan"),
                    new Claim(ClaimType.DisplayName, "Sophie Jordan"),
                    new Claim(ClaimType.Gender, "F")
                },
                new string[]
                {
                    //LookupRoleId(DbKeys.PoolIds.PcMacClient, DbKeys.RoleNames.ClientOnlyAdmins)
                }
            );
            #endregion

            #region Site2 Webmaster Oliver Burke
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.OliverB,
                    UserName = "Oliver",
                    Email = "oliverb@inspirationsd.schoolinsites.com",
                    PasswordHash = "Oliver.1",
                    FirstName = "Oliver",
                    LastName = "Burke",
                    DisplayName = "Oliver B.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Oliver"),
                    new Claim(ClaimType.LastName, "Burke"),
                    new Claim(ClaimType.DisplayName, "Oliver Burke"),
                    new Claim(ClaimType.Gender, "M")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacSite2, DbKeys.RoleNames.SiteAdmins)
                }
            );
            #endregion

            #region Site2 Staff Yasmin Mcguire
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.PcMac,
                    Id = DbKeys.UserIds.YasminM,
                    UserName = "YasminM",
                    Email = "yasminm@inspirationsd.schoolinsites.com",
                    PasswordHash = "Yasmin.1",
                    FirstName = "Yasmin",
                    LastName = "Mcguire",
                    DisplayName = "Yasmin M.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Yasmin"),
                    new Claim(ClaimType.LastName, "Mcguire"),
                    new Claim(ClaimType.DisplayName, "Yasmin Mcguire"),
                    new Claim(ClaimType.Gender, "F")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.PcMacSite2, DbKeys.RoleNames.SiteStaff)
                }
            );
            #endregion
        }

        private void SeedBocaUsers()
        {
            var tenant = _identityDb.Tenants.First(x => x.Key == DbKeys.ClientTenantKeys.Boca);

            #region Administrator Rhonda
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Boca,
                    Id = DbKeys.UserIds.Rhonda,
                    UserName = "Rhonda",
                    Email = "rhonda@cheatpads.com",
                    PasswordHash = "Rhonda.1",
                    FirstName = "Rhonda",
                    LastName = "Gillespie",
                    DisplayName = "Rhonda G.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Rhonda"),
                    new Claim(ClaimType.LastName, "Gillespie"),
                    new Claim(ClaimType.DisplayName, "Rhonda Gillespie"),
                    new Claim(ClaimType.Gender, "F")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.BocaClient, DbKeys.RoleNames.ClientAdmins),
                    LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteAdmins),
                }
            );
            #endregion

            #region Principal Sally
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Boca,
                    Id = DbKeys.UserIds.Sally,
                    UserName = "sally",
                    Email = "sally@cheatpads.com",
                    PasswordHash = "Sally.1",
                    FirstName = "Sally",
                    LastName = "O'Malley",
                    DisplayName = "Sally O.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Sally"),
                    new Claim(ClaimType.LastName, "O'Malley"),
                    new Claim(ClaimType.DisplayName, "Sally O'Malley"),
                    new Claim(ClaimType.Gender, "F")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteStaff)
                }
            );
            #endregion

            #region Principal Sammy
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Boca,
                    Id = DbKeys.UserIds.Sammy,
                    UserName = "Sammy",
                    Email = "sammy@cheatpads.com",
                    PasswordHash = "Sammy.1",
                    FirstName = "Sammy",
                    LastName = "Iam",
                    DisplayName = "Sammy I.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Sammy"),
                    new Claim(ClaimType.LastName, "Iam"),
                    new Claim(ClaimType.DisplayName, "Sammy Iam"),
                    new Claim(ClaimType.Gender, "X")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteStaff)
                }
            );
            #endregion

            #region Coach Steve
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Boca,
                    Id = DbKeys.UserIds.Steve,
                    UserName = "Steve",
                    Email = "steve@cheatpads.com",
                    PasswordHash = "Steve.1",
                    FirstName = "Steven",
                    LastName = "Reeves",
                    DisplayName = "Sammy I.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Steven"),
                    new Claim(ClaimType.LastName, "Reeves"),
                    new Claim(ClaimType.DisplayName, "Coach Steve"),
                    new Claim(ClaimType.Gender, "M")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteStaff)
                }
            );
            #endregion

            #region Teacher Sarah
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Boca,
                    Id = DbKeys.UserIds.Sarah,
                    UserName = "Sarah",
                    Email = "sarah@cheatpads.com",
                    PasswordHash = "Sarah.1",
                    FirstName = "Sarah",
                    LastName = "Teacher",
                    DisplayName = "Sarah I.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Sarah"),
                    new Claim(ClaimType.LastName, "Teacher"),
                    new Claim(ClaimType.DisplayName, "Sarah"),
                    new Claim(ClaimType.Gender, "F"),
                    new Claim(DbKeys.ClaimTypes.ACL_Parent, DbKeys.UserIds.Mandy)
                },
                new string[]
                {
                     LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteUsers),
                }
            );
            #endregion

            #region Teacher Jane
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Boca,
                    Id = DbKeys.UserIds.Jane,
                    UserName = "jane",
                    Email = "jane@cheatpads.com",
                    PasswordHash = "Jane.1",
                    FirstName = "Jane",
                    LastName = "Doe",
                    DisplayName = "Jane I.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Jane"),
                    new Claim(ClaimType.LastName, "Doe"),
                    new Claim(ClaimType.DisplayName, "Jane"),
                    new Claim(ClaimType.Gender, "F"),
                    new Claim(DbKeys.ClaimTypes.ACL_Parent, DbKeys.UserIds.Bobby)
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteUsers),
                }
            );
            #endregion

            #region Student Bobby
            CreateUser(
               new User()
               {
                   TenantId = tenant.Id,
                   DirectoryId = DbKeys.DirectoryIds.Boca,
                   Id = DbKeys.UserIds.Bobby,
                   UserName = "Bobby",
                   Email = "bobby@cheatpads.com",
                   PasswordHash = "Bobby.1",
                   PhoneNumber = "504-905-7984",
                   PhoneNumberConfirmed = true,
                   WirelessProviderId = DbKeys.WirelessProviderIds.Verizon,
                   FirstName = "Bobby",
                   LastName = "ReeveDoes",
                   DisplayName = "Bobby Doe",
                   Title = "",
                   Suffix = "",
                   BirthDate = DateTime.MinValue,
               },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Bobby"),
                    new Claim(ClaimType.LastName, "Doe"),
                    new Claim(ClaimType.DisplayName, "Bobby Doe"),
                    new Claim(ClaimType.Gender, "M"),
                    new Claim(ClaimType.AltId, "STU-12300987"),
                    new Claim("emergency_contact1", "Jane Doe"),
                    new Claim("emergency_phone1", "251-555-5555"),
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteUsers),
                }
            );
            #endregion

            #region Student Mandy
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Boca,
                    Id = DbKeys.UserIds.Mandy,
                    UserName = "mandy",
                    Email = "mandy@cheatpads.com",
                    PasswordHash = "Mandy.1",
                    FirstName = "Mandy",
                    LastName = "Pop",
                    DisplayName = "Mandy Pop.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "251-555-1234",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Mandy"),
                    new Claim(ClaimType.LastName, "Pop"),
                    new Claim(ClaimType.DisplayName, "Mandy Popp"),
                    new Claim(ClaimType.Gender, "F"),
                    new Claim(ClaimType.AltId, "STU-11002233"),
                    new Claim("allow_pickup", "Candy Popp"),
                    new Claim("emergency_contact1", "Candy Popp"),
                    new Claim("emergency_phone1", "251-555-1234"),
                    new Claim("emergency_contact2", "Soda Popp"),
                    new Claim("emergency_phone2", "251-555-6789")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteUsers),
                }
            );
            #endregion

            #region Parent Candy
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Boca,
                    Id = DbKeys.UserIds.Candy,
                    UserName = "candy",
                    Email = "candy@cheatpads.com",
                    PasswordHash = "Candy.1",
                    FirstName = "Candice",
                    LastName = "Pop",
                    DisplayName = "Candice P.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222",
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Candice"),
                    new Claim(ClaimType.LastName, "Pop"),
                    new Claim(ClaimType.DisplayName, "Candy Popp"),
                    new Claim(DbKeys.ClaimTypes.ACL_Parent, DbKeys.UserIds.Mandy)
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteUsers),
                }
            );
            #endregion


            #region MockUsers
            /*
            var bulkRoleIds = new string[]
            {
                LookupRoleId(DbKeys.PoolIds.BocaDistrict, DbKeys.RoleNames.SiteUsers),
            };


            for (var i = 0; i < MockData.People.Count; i += 3)
            {
                var person = MockData.People[i];

                CreateUser(
                    new User
                    {
                        TenantId = tenant.Id,
                        DirectoryId = DbKeys.DirectoryIds.Boca,
                        Id = Guid.NewGuid().ToString(),
                        UserName = person.UserName,
                        Email = person.Email,
						PasswordHash = person.FirstName + ".1"
                    },
                    new Claim[]
                    {
                        new Claim(ClaimType.FirstName, person.FirstName),
                        new Claim(ClaimType.LastName, person.LastName),
                        new Claim(ClaimType.DisplayName, person.FullName),
                    },
                    bulkRoleIds
                );
            }
            */
            #endregion
        }

        private void SeedWemoUsers()
        {
            var tenant = _identityDb.Tenants.First(x => x.Key == DbKeys.ClientTenantKeys.Wemo);

            #region Father Joe
            CreateUser(
                new User()
                {
                    TenantId = tenant.Id,
                    DirectoryId = DbKeys.DirectoryIds.Wemo,
                    Id = DbKeys.UserIds.Joel,
                    UserName = "joel",
                    Email = "joel@cheatpads.com",
                    PasswordHash = "Joel.1",
                    FirstName = "Joel",
                    LastName = "Doe",
                    DisplayName = "Joel D.",
                    Title = "",
                    Suffix = "",
                    BirthDate = DateTime.MinValue,
                    PhoneNumber = "222-222-2222"
                },
                new Claim[]
                {
                    new Claim(ClaimType.FirstName, "Joel"),
                    new Claim(ClaimType.LastName, "Doe"),
                    new Claim(ClaimType.DisplayName, "Father Joel"),
                    new Claim(ClaimType.Gender, "M")
                },
                new string[]
                {
                    LookupRoleId(DbKeys.PoolIds.WemoClient, DbKeys.RoleNames.ClientAdmins)
                }
            );
            #endregion

        }

        private Client CreateClient(string defaultDirectoryId, Client client)
        {
            //
            // Create the OIDC tenant for this client
            //
            var tenant = new Tenant
            {
                Id = KeyGen.NewGuid(),
                Name = client.Name,
                Key = client.TenantKey,
                OidcTitle = client.Name + " Sign-on Server",
                OidcBanner = "/img/pcmac/sislogo-white.png"
            };

            _identityDb.Tenants.Add(tenant);
            _identityDb.SaveChanges();

            //
            // Create the default User Directory
            //
            _identityDb.Directories.Add(
                new Directory
                {
                    Id = defaultDirectoryId,
                    Name = client.Name + " User Directory",
                    TenantId = tenant.Id,
                }
            );

            _identityDb.SaveChanges();

            //
            // Create the client's security pool
            //
            _identityDb.SecurityPools.Add(new Angelo.Identity.Models.SecurityPool()
            {
                PoolId = client.SecurityPoolId,
                Name = client.Name + " Security Pool",
                TenantId = tenant.Id,
                PoolType = PoolType.Client,
                DirectoryMap = new DirectoryMap[]
                {
                    new DirectoryMap
                    {
                        PoolId = client.SecurityPoolId,
                        DirectoryId = defaultDirectoryId
                    }
                }
            });

            _identityDb.SaveChanges();

            //
            // Create Client Root Admin
            // 
            _identityDb.Roles.Add(new Role()
            {
                PoolId = client.SecurityPoolId,
                Name = DbKeys.RoleNames.ClientAdmins,
                IsLocked = true,
                RoleClaims = new RoleClaim[]
                {
                    new RoleClaim { ClaimType = SiteClaimTypes.SitePrimaryAdmin, ClaimValue = client.Id },
                    new RoleClaim { ClaimType = ClientClaimTypes.PrimaryAdmin, ClaimValue = client.Id },
                }
            });

            _identityDb.SaveChanges();

            //
            // Create the Client in Connect
            //
            _connectDb.Clients.Add(client);
            _connectDb.SaveChanges();


            //
            // Create the Client library
            // 
            var clientlocationResolver = new DocumentPhysicalLocationResolver("Client", client.Id, "", "");
            CreateDocumentLibrary(client.Id, "Client", clientlocationResolver.Resolve());



            return client;
        }

        private void CreateSite(string defaultDirectoryId, Client client, Site site)
        {
            #region Aegis OIDC Settings
            //
            // Configure OIDC Uris allowed for this site based on configured domains
            // 
            var tenant = _identityDb.Tenants.First(x => x.Key == client.TenantKey);
            var tenantUris = new List<TenantUri>();

            foreach (var domain in site.Domains)
            {
                // Note a "tenant" in OIDC corresponds to a Client in Connect
                tenantUris.Add(new TenantUri { Id = KeyGen.NewGuid(), TenantId = tenant.Id, Type = TenantUriType.OidcSignin, Uri = $"{domain.DomainKey}/signin-oidc" });
                tenantUris.Add(new TenantUri { Id = KeyGen.NewGuid(), TenantId = tenant.Id, Type = TenantUriType.OidcPostLogout, Uri = $"{domain.DomainKey}/signout-oidc" });
            }

            _identityDb.TenantUris.AddRange(tenantUris);


            //
            // Create the Site's Security Pool
            //
            _identityDb.SecurityPools.Add(
                new Angelo.Identity.Models.SecurityPool
                {
                    TenantId = tenant.Id,
                    PoolId = site.SecurityPoolId,
                    Name = site.Title + " Security Pool",
                    PoolType = PoolType.Site,
                    ParentPoolId = client.SecurityPoolId,
                    DirectoryMap = new DirectoryMap[]
                    {
                        new DirectoryMap
                        {
                            PoolId = site.SecurityPoolId,
                            DirectoryId = defaultDirectoryId
                        }
                    }
                }
            );

            _identityDb.SaveChanges();
            #endregion


            #region Site Collecton
            // 
            // Get or Create Default Site Collection
            //          

            var siteCollection = _connectDb.SiteCollections.FirstOrDefault(x => x.ClientId == client.Id);

            if (siteCollection == null)
            {
                siteCollection = new SiteCollection()
                {
                    Id = KeyGen.NewGuid(),
                    ClientId = client.Id,
                    Name = "Default Site Collection",
                };

                _connectDb.SiteCollections.Add(siteCollection);
                _connectDb.SaveChanges();
            }

            // 
            // Map the site Collection 
            // 
            site.SiteCollectionMaps = new SiteCollectionMap[]
            {
                new SiteCollectionMap {
                    SiteId = site.Id,
                    SiteCollectionId = siteCollection.Id
                }
            };

            //
            // Set Default settings & Save Stie
            //
            site.Cultures = GetDefaultSiteCultures();
            site.SiteSettings = GetDefaultSiteSettings();

            _connectDb.Sites.Add(site);
            _connectDb.SaveChanges();

            //
            // Execute the Site Seed Process to create Pages, Roles, Menus, etc
            //
            _sitePublisher.CreateInitialVersion(site).Wait();
            #endregion
        }

        private void CreateUser(User user, IEnumerable<Claim> claims, IEnumerable<string> roles)
        {
            // Override the default flags for seeded users
            user.PhoneNumberConfirmed = true;
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            user.TwoFactorEnabled = false;
            user.MustChangePassword = false;
            user.AccessFailedCount = 0;

            // Create using usermanager to normalize username, initialize security stamp, etc.
            var result = _userManager.CreateAsync(user).Result;

            // Throw failures as exceptions with full details
            if (!result.Succeeded)
            {
                var error = $"An error occurred creating user {user.UserName}. ";

                if (result.Errors != null)
                    error += String.Join(". ", result.Errors.Select(x => x.Description));

                throw new Exception(error);
            }

            // TODO: Claims should be seeded using user manager rather than directly to the DB
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    _identityDb.UserClaims.Add(new UserClaim
                    {
                        UserId = user.Id,
                        ClaimType = claim.Type,
                        ClaimValue = claim.Value
                    });
                }

                _identityDb.SaveChanges();
            }

            // TODO: Roles should be seeded using user manager  rather than directly to DB
            if (roles != null)
            {
                foreach (var id in roles)
                {
                    if (id != null) // eg, we're trying to assign a role by name that doens't exist
                        _identityDb.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = id });
                }

                _identityDb.SaveChanges();
            }
        }

        private DocumentLibrary CreateDocumentLibrary(string ownerId, string libraryType, string location)
        {
            var documentLibrary = _connectDb.DocumentLibraries.FirstOrDefault(x => x.OwnerId == ownerId && x.LibraryType == libraryType);
            if (documentLibrary == null)
            {
                documentLibrary = new DocumentLibrary
                {
                    Id = Guid.NewGuid().ToString("N"),
                    OwnerId = ownerId,
                    LibraryType = libraryType,
                    Location = location
                };

                _connectDb.DocumentLibraries.Add(documentLibrary);
                _connectDb.SaveChanges();
            }

            //ensure root is created
            var root = _connectDb.Folders.SingleOrDefault(x =>
                                    x.OwnerId == ownerId
                                    && x.DocumentLibraryId == documentLibrary.Id
                                    && string.IsNullOrEmpty(x.ParentId)
                                 );

            if (root == null)
            {
                root = new Folder()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = string.Empty,
                    OwnerLevel = documentLibrary.LibraryType == "Site" ? Security.OwnerLevel.Site : (documentLibrary.LibraryType == "Client" ? Security.OwnerLevel.Client : Security.OwnerLevel.User),
                    DocumentType = typeof(FileDocument).FullName,
                    FolderType = typeof(Folder).Name,
                    OwnerId = ownerId,
                    DocumentLibraryId = documentLibrary.Id,
                    CreatedBy = ownerId
                };

                _connectDb.Folders.Add(root);
                _connectDb.SaveChanges();
            }


            //ensure trash folder is created

            var trash = _connectDb.Folders.SingleOrDefault(x =>
                                    x.OwnerId == ownerId
                                    && x.DocumentLibraryId == documentLibrary.Id
                                    && x.ParentId == root.Id
                                    && x.IsSystemFolder
                                    && x.Title == "Trash"
                                 );
            if (trash == null)
            {
                trash = new Folder()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = "Trash",
                    OwnerLevel = documentLibrary.LibraryType == "Site" ? Security.OwnerLevel.Site : (documentLibrary.LibraryType == "Client" ? Security.OwnerLevel.Client : Security.OwnerLevel.User),
                    DocumentType = typeof(FileDocument).FullName,
                    FolderType = typeof(Folder).Name,
                    OwnerId = ownerId,
                    DocumentLibraryId = documentLibrary.Id,
                    ParentId = root.Id,
                    IsSystemFolder = true,
                    CreatedBy = ownerId
                };
                _connectDb.Folders.Add(trash);
                _connectDb.SaveChanges();
            }

            return documentLibrary;
        }

        private IList<SiteCulture> GetDefaultSiteCultures()
        {
            return new List<SiteCulture>()
            {
                new SiteCulture() { CultureKey = "en-US" },
                new SiteCulture() { CultureKey = "es-US" },
                new SiteCulture() { CultureKey = "ar-LB" }
            };
        }

        private IList<SiteSetting> GetDefaultSiteSettings()
        {
            return new List<SiteSetting>()
            {
                new SiteSetting() { FieldName = "Custom401", Value = "0" },
                new SiteSetting() { FieldName = "Custom404", Value = "0" },
                new SiteSetting() { FieldName = "Custom500", Value = "0" },
                new SiteSetting() { FieldName = "LandingPage", Value = "0" },
            };
        }

        // TODO: Move Wireless Providers to Settings. No reason for this to be in DB

        private string LookupRoleId(string appId, string roleName)
        {
            var role = _identityDb.Roles.FirstOrDefault(x => x.PoolId == appId && x.Name == roleName);

            return role?.Id;
        }

        private string LookupProductTitle(string productId)
        {
            return _connectDb.Products.FirstOrDefault(x => x.Id == productId)?.Name;
        }
    }
}
