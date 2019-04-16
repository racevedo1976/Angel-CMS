using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Models;
using Angelo.Connect.Widgets.Models;
using Microsoft.EntityFrameworkCore.Metadata;
using Angelo.Connect.Security;

namespace Angelo.Connect.Data
{
    public class ConnectDbContext : DbContext
    {
        // Client Models
        public DbSet<Client> Clients { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductAddOn> ProductAddOns { get; set; }
        public DbSet<ProductAddOnLink> ProductAddOnLinks { get; set; }
        public DbSet<ClientProductApp> ClientProductApps { get; set; }
        public DbSet<ClientProductAddOn> ClientProductAddOns { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        // Site Models
        public DbSet<Site> Sites { get; set; }
        public DbSet<SiteCulture> SiteCultures { get; set; }
        public DbSet<SiteDomain> SiteDomains { get; set; }
        public DbSet<SiteCollection> SiteCollections { get; set; }
        public DbSet<SiteCollectionMap> SiteCollectionMaps { get; set; }
        public DbSet<SiteSetting> SiteSettings { get; set; }
        public DbSet<SiteDirectory> SiteDirectories { get; set; }
        public DbSet<SiteAlert> SiteAlerts { get; set; }

        //Page / Content Models
        public DbSet<Page> Pages { get; set; }
        public DbSet<PageMaster> PageMasters { get; set; }
        public DbSet<ContentClaim> ContentClaims { get; set; }
        public DbSet<ContentCategory> ContentCategories { get; set; }
        public DbSet<ContentTag> ContentTags { get; set; }
        public DbSet<ContentTree> ContentTrees { get; set; }
        public DbSet<ContentNode> ContentNodes { get; set; }
        public DbSet<ContentVersion> ContentVersions { get; set; }

        //Generic Widget Models
        public DbSet<JsonWidget> JsonWidgets { get; set; }

        //public DbSet<ViewTemplate> ViewTemplates { get; set; }
        public DbSet<NavigationMenu> NavigationMenu { get; set; }
        public DbSet<NavigationMenuItem> NavigationMenuItems { get; set; }

        // Document /Folder Models
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<FolderItem> FolderItems { get; set; }

        // Folder Security
        public DbSet<FolderSharing> FolderSharing { get; set; }
        public DbSet<ResourceClaim> ResourceClaims { get; set; }
        public DbSet<GroupResourceClaim> GroupResourceClaims { get; set; }
        public DbSet<FolderTag> FolderTags { get; set; }
        public DbSet<FolderCategory> FolderCategories { get; set; }
        public DbSet<DocumentTag> DocumentTags { get; set; }
        public DbSet<DocumentCategory> DocumentCategories { get; set; }
        public DbSet<FileDocument> FileDocuments { get; set; }
        public DbSet<DocumentLibrary> DocumentLibraries { get; set; }

        // User Models
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<UserGroupMembership> UserGroupMemberships { get; set; }
        public DbSet<UserGroupHierarchy> UserGroupHierarchies { get; set; }

        // Notification Models
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationUserGroup> NotificationUserGroups { get; set; }
        public DbSet<NotificationEmailLog> NotificationEmailLog { get; set; }
        public DbSet<NotificationSmsLog> NotificationSmsLog { get; set; }
        public DbSet<NotificationEmailHeader> NotificationEmailHeaders { get; set; }
        public DbSet<NotificationUnsubscribeCode> NotificationUnsubscribeCode { get; set; }

        public ConnectDbContext(DbContextOptions<ConnectDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureClientModels(modelBuilder);
            ConfigureSiteModels(modelBuilder);
            ConfigurePageModels(modelBuilder);
            ConfigureWidgetModels(modelBuilder);
            ConfigureDocumentModels(modelBuilder);
            ConfigureUserModels(modelBuilder);
            ConfigureNotificationModels(modelBuilder);
        }

        private static void ConfigureClientModels(ModelBuilder builder)
        {
            builder.Entity<Client>(client =>
            {
                client.ToTable("Client", "cms").HasKey(x => x.Id);
            });

            builder.Entity<ClientProductApp>(product =>
            {
                product.Ignore(x => x.IsActive);

                product.ToTable("ClientProductApp", "cms").HasKey(x => x.Id);

                product.HasOne(x => x.Product)
                     .WithMany(x => x.ClientProductApps)
                     .HasForeignKey(x => x.ProductId);

                product.HasOne(x => x.Client)
                    .WithMany(x => x.ClientProductApps)
                    .HasForeignKey(x => x.ClientId);
            });

            builder.Entity<ClientProductAddOn>(product =>
            {
                product.ToTable("ClientProductAddOn", "cms").HasKey(x => new { x.ClientProductAppId, x.ProductAddOnId });

                product.HasOne(x => x.ClientProductApp)
                     .WithMany(x => x.AddOns)
                     .HasForeignKey(x => x.ClientProductAppId);

                product.HasOne(x => x.ProductAddOn)
                    .WithMany(x => x.ClientProductAddOns)
                    .HasForeignKey(x => x.ProductAddOnId);
            });

            builder.Entity<Product>(product =>
            {
                product.ToTable("Product", "cms").HasKey(x => x.Id);

                product.HasOne(x => x.Category)
                    .WithMany(x => x.Products)
                    .HasForeignKey(x => x.CategoryId);
            });

            builder.Entity<ProductAddOn>(productAddOn =>
            {
                productAddOn.ToTable("ProductAddOn", "cms").HasKey(x => x.Id);
            });

            builder.Entity<ProductAddOnLink>(productAddOn =>
            {
                productAddOn.ToTable("ProductAddOnLink", "cms").HasKey(x => new { x.ProductId, x.ProductAddOnId });

                productAddOn.HasOne(x => x.Product)
                    .WithMany(x => x.ProductAddOnLinks)
                    .HasForeignKey(x => x.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                productAddOn.HasOne(x => x.ProductAddOn)
                    .WithMany(x => x.ProductAddOnLinks)
                    .HasForeignKey(x => x.ProductAddOnId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            builder.Entity<ProductCategory>(category =>
            {
                category.ToTable("ProductCategory", "cms").HasKey(x => x.Id);

            });
        }

        private static void ConfigureSiteModels(ModelBuilder builder)
        {
            builder.Entity<Site>(site =>
            {
                site.ToTable("Site", "cms").HasKey(x => x.Id);

                site.HasOne(x => x.Client)
                    .WithMany(x => x.Sites)
                    .HasForeignKey(x => x.ClientId);

                site.HasOne(x => x.ClientProductApp)
                    .WithMany(x => x.Sites)
                    .HasForeignKey(x => x.ClientProductAppId);
            });

            builder.Entity<SiteCollection>(collection =>
            {
                collection.ToTable("SiteCollection", "cms").HasKey(x => x.Id);
                //collection.Property(x => x.Id).UseSqlServerIdentityColumn();

                collection.HasOne(x => x.Client)
                    .WithMany(x => x.SiteCollections)
                    .HasForeignKey(x => x.ClientId);
            });

            builder.Entity<SiteCollectionMap>(map =>
            {
                map.ToTable("SiteCollectionMap", "cms").HasKey(x => new { x.SiteCollectionId, x.SiteId });

                map.HasOne(x => x.SiteCollection)
                    .WithMany(sc => sc.SiteCollectionMaps)
                    .HasForeignKey(x => x.SiteCollectionId);

                map.HasOne(x => x.Site)
                    .WithMany(s => s.SiteCollectionMaps)
                    .HasForeignKey(x => x.SiteId);
            });

            builder.Entity<SiteCulture>(siteCulture =>
            {
                siteCulture.ToTable("SiteCulture", "cms").HasKey(x => new { x.SiteId, x.CultureKey });

                siteCulture.HasOne(x => x.Site)
                    .WithMany(x => x.Cultures)
                    .HasForeignKey(x => x.SiteId);
            });

            builder.Entity<SiteDomain>(domain =>
            {
                domain.ToTable("SiteDomain", "cms").HasKey(x => new { x.SiteId, x.DomainKey });

                domain.HasOne(x => x.Site)
                    .WithMany(x => x.Domains)
                    .HasForeignKey(x => x.SiteId);
            });

            builder.Entity<SiteSetting>(siteSetting =>
            {
                siteSetting.ToTable("SiteSetting", "cms").HasKey(x => new { x.SiteId, x.FieldName });

                siteSetting.HasOne(x => x.Site)
                     .WithMany(x => x.SiteSettings)
                     .HasForeignKey(x => new { x.SiteId });
            });        

            builder.Entity<SiteDirectory>(siteDirectory =>
            {
                siteDirectory.ToTable("SiteDirectory", "cms").HasKey(x => new { x.DirectoryId, x.SiteId });

                siteDirectory.HasOne(x => x.Site)
                    .WithMany(x => x.SiteDirectories)
                    .HasForeignKey(x => new { x.SiteId });
            });

            
            builder.Entity<SiteAlert>(siteAlert =>
            {
                siteAlert.ToTable("SiteAlert", "cms").HasKey(x => x.Id);
            });
            
        }

        private static void ConfigurePageModels(ModelBuilder builder)
        {
            builder.Entity<Page>(page =>
            {
                page.ToTable("Page", "cms").HasKey(x => x.Id);

                page.HasOne(x => x.Site)
                    .WithMany(x => x.Pages)
                    .HasForeignKey(x => x.SiteId);

                page.HasOne(x => x.ParentPage)
                    .WithMany(x => x.ChildPages)
                    .HasForeignKey(x => x.ParentPageId);

                page.HasOne(x => x.MasterPage)
                    .WithMany(x => x.DerivedPages)
                    .HasForeignKey(x => x.PageMasterId);

            });

            builder.Entity<PageMaster>(master =>
            {
                master.ToTable("PageMaster", "cms").HasKey(x => x.Id);

                master.HasOne(x => x.Site)
                    .WithMany(x => x.PageMasters)
                    .HasForeignKey(x => x.SiteId);
            });

            builder.Entity<ContentTree>(tree => {
                tree.ToTable("ContentTree", "cms").HasKey(x => x.Id);
            });

            builder.Entity<ContentNode>(node =>
            {
                node.ToTable("ContentNode", "cms").HasKey(x => x.Id);

                node.HasOne(x => x.ContentTree)
                    .WithMany(x => x.ContentNodes)
                    .HasForeignKey(x => x.ContentTreeId);

                node.HasMany(x => x.ChildNodes)
                    .WithOne(x => x.ParentNode)
                    .HasForeignKey(x => x.ParentId);

            });

            builder.Entity<ContentVersion>(version => {
                version.ToTable("ContentVersion", "cms").HasKey(x => new { x.ContentId, x.VersionCode });
            });

            builder.Entity<ContentClaim>(entity =>
            {
                entity
                    .ToTable("ContentClaim", "cms")
                    .HasKey(x => new { x.ContentType, x.ContentId, x.OwnerId, x.ClaimType });
            });

            builder.Entity<ContentCategory>(entity =>
            {
                entity
                    .ToTable("ContentCategory", "cms")
                    .HasKey(x => new { x.ContentType, x.ContentId, x.CategoryId });
            });

            builder.Entity<ContentTag>(entity =>
            {
                entity
                    .ToTable("ContentTag", "cms")
                    .HasKey(x => new { x.ContentType, x.ContentId, x.TagName });
            });


            builder.Entity<NavigationMenu>(navigationMenu =>
            {
                navigationMenu.ToTable("NavigationMenu", "cms").HasKey(x => x.Id);

                // IsDefault is readonly - determine based on menu's scope
                navigationMenu.Ignore(x => x.IsDefault);

                navigationMenu.HasOne(x => x.Site)
                    .WithMany(x => x.NavigationMenus)
                    .HasForeignKey(x => x.SiteId);

            });


            builder.Entity<NavigationMenuItem>(navigationMenuItem =>
            {
                navigationMenuItem.ToTable("NavigationMenuItem", "cms").HasKey(x => x.Id);

                navigationMenuItem.HasOne(x => x.NavMenu)
                .WithMany(x => x.MenuItems)
                .HasForeignKey(x => x.NavMenuId);

                navigationMenuItem.HasOne(x => x.Parent)
                .WithMany(x => x.Children)
                .HasForeignKey(x => x.ParentId);
            });
        }

        private static void ConfigureWidgetModels(ModelBuilder builder)
        {
            builder.Entity<JsonWidget>(widget => 
            {
                widget.ToTable("JsonWidget", "cms").HasKey(x => x.Id);
            });
        }

        private static void ConfigureDocumentModels(ModelBuilder builder)
        {
            builder.Entity<Tag>(tag =>
            {
                tag.ToTable("Tag", "cms").HasKey(x => x.Id);
            });

            builder.Entity<Category>(category =>
            {
                category.ToTable("Category", "cms").HasKey(x => x.Id);

                category
                    .HasMany(x => x.FolderMap)
                    .WithOne(x => x.Category)
                    .HasForeignKey(x => x.CategoryId);

                category
                    .HasMany(x => x.DocumentMap)
                    .WithOne(x => x.Category)
                    .HasForeignKey(x => x.CategoryId);

                category
                    .HasMany(x => x.ContentMap)
                    .WithOne(x => x.Category)
                    .HasForeignKey(x => x.CategoryId);
            });

            builder.Entity<Folder>(entity =>
            {
                entity
                    .ToTable("Folder", "cms")
                    .HasKey(x => x.Id);

                entity
                    .HasMany(x => x.ChildFolders)
                    .WithOne(x => x.ParentFolder)
                    .HasForeignKey(x => x.ParentId);

                entity
                    .HasMany(x => x.Items)
                    .WithOne(x => x.Folder)
                    .HasForeignKey(x => x.FolderId);

                entity
                    .HasMany(x => x.CategoryMap)
                    .WithOne(x => x.Folder)
                    .HasForeignKey(x => x.FolderId);

                entity
                    .HasMany(x => x.Tags)
                    .WithOne(x => x.Folder)
                    .HasForeignKey(x => x.FolderId);

                //entity
                //    .HasMany(x => x.Security)
                //    .WithOne(x => x.)
                //    .HasForeignKey(x => x.FolderId);

                entity
                    .HasMany(x => x.Sharing)
                    .WithOne(x => x.Folder)
                    .HasForeignKey(x => x.FolderId)
                    .OnDelete(Microsoft.EntityFrameworkCore.Metadata.DeleteBehavior.Restrict);

            });


            builder.Entity<DocumentLibrary>(entity =>
            {
                entity
                    .ToTable("DocumentLibrary", "cms")
                    .HasKey(x => new { x.Id });

                entity.HasMany(x => x.Folders).WithOne(x => x.DocumentLibrary).HasForeignKey(x => x.DocumentLibraryId);

            });

            builder.Entity<FolderItem>(entity =>
            {
                entity
                    .ToTable("FolderItem", "cms")
                    .HasKey(x => new { x.FolderId, x.DocumentId });

                entity.HasAlternateKey(x => x.Id);
            });

            builder.Entity<FolderSharing>(entity =>
            {
                entity
                    .ToTable("FolderSharing", "cms")
                    .HasKey(x => new { x.FolderId, x.SharedFolderId });
            });

            builder.Entity<ResourceClaim>(entity =>
            {
                entity
                    .ToTable("ResourceClaim", "cms")
                    .HasKey(x => new { x.ResourceId, x.ResourceType, x.ClaimType, x.UserId });
            });

            builder.Entity<GroupResourceClaim>(entity =>
            {
                entity
                    .ToTable("GroupResourceClaim", "cms")
                    .HasKey(x => new { x.ResourceId, x.ResourceType, x.ClaimType, x.GroupId });
            });

            builder.Entity<FolderCategory>(entity =>
            {
                entity
                    .ToTable("FolderCategory", "cms")
                    .HasKey(x => new { x.FolderId, x.CategoryId });
            });

            builder.Entity<FolderTag>(entity =>
            {
                entity
                    .ToTable("FolderTag", "cms")
                    .HasKey(x => new { x.FolderId, x.TagName });
            });

            builder.Entity<DocumentCategory>(entity =>
            {
                entity
                    .ToTable("DocumentCategory", "cms")
                    .HasKey(x => new { x.DocumentId, x.CategoryId });
            });

            builder.Entity<DocumentTag>(entity =>
            {
                entity
                    .ToTable("DocumentTag", "cms")
                    .HasKey(x => new { x.DocumentId, x.TagName });
            });

            builder.Entity<FileDocument>(entity =>
            {
                entity
                    .ToTable("FileDocument", "cms")
                    .HasKey(x => x.DocumentId);

                entity
                    .HasMany(x => x.Tags)
                    .WithOne(x => x.Document)
                    .HasForeignKey(x => x.DocumentId);
            });
            //TODO: Sharing / Claim = Security. Consolidate, then add for Documents
        }

        private static void ConfigureUserModels(ModelBuilder builder)
        {
            builder.Entity<UserGroup>(userGroup =>
            {
                userGroup.ToTable("UserGroup", "cms").HasKey(x => x.Id);
            });

            builder.Entity<UserGroupMembership>(userGroupMembership =>
            {
                userGroupMembership.ToTable("UserGroupMembership", "cms").HasKey(x => new { x.UserGroupId, x.UserId });

                userGroupMembership.HasOne(x => x.UserGroup)
                    .WithMany(x => x.Memberships)
                    .HasForeignKey(x => x.UserGroupId);
            });

            builder.Entity<UserGroupHierarchy>(userGroupHierarchy =>
            {
                userGroupHierarchy.ToTable("UserGroupHierarchy", "cms").HasKey(x => new { x.ParentGroupId, x.ChildGroupId });
            });
        }

        private static void ConfigureNotificationModels(ModelBuilder builder)
        {
            builder.Entity<Notification>(n =>
            {
                n.ToTable("Notification", "cms").HasKey(x => x.Id);
                n.Property(x => x.Status).HasMaxLength(10);
            });

            builder.Entity<NotificationUserGroup>(g =>
            {
                g.ToTable("NotificationUserGroup", "cms").HasKey(x => new { x.NotificationId, x.UserGroupId });
            });

            builder.Entity<NotificationEmailLog>(l =>
            {
                l.ToTable("NotificationEmailLog", "cms").HasKey(x => x.Id);
            });

            builder.Entity<NotificationSmsLog>(l =>
            {
                l.ToTable("NotificationSmsLog", "cms").HasKey(x => x.Id);
            });

            builder.Entity<NotificationEmailHeader>(h =>
            {
                h.ToTable("NotificationEmailHeader", "cms").HasKey(x => x.Id);
            });

            builder.Entity<NotificationUnsubscribeCode>(c =>
            {
                c.ToTable("NotificationUnsubscribeCode", "cms").HasKey(x => x.Id);
                c.Property(x => x.Id).UseSqlServerIdentityColumn();
            });
        }

    }
}
