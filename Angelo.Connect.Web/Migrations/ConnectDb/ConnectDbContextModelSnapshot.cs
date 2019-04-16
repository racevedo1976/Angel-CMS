using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Angelo.Connect.Data;
using Angelo.Connect.Security;
using Angelo.Connect.Models;
using Angelo.Identity.Models;

namespace Angelo.Connect.Web.Migrations.ConnectDb
{
    [DbContext(typeof(ConnectDbContext))]
    partial class ConnectDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Angelo.Connect.Models.Category", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<string>("OwnerId");

                    b.Property<int>("OwnerLevel");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Category","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Client", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Address1");

                    b.Property<string>("Address2");

                    b.Property<DateTime>("AnniversaryDate");

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("Name");

                    b.Property<string>("Notes");

                    b.Property<string>("PostalCode");

                    b.Property<string>("PreferredName");

                    b.Property<string>("SecurityPoolId");

                    b.Property<string>("ShortName");

                    b.Property<string>("State");

                    b.Property<string>("TenantKey");

                    b.HasKey("Id");

                    b.ToTable("Client","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ClientProductAddOn", b =>
                {
                    b.Property<string>("ClientProductAppId");

                    b.Property<string>("ProductAddOnId");

                    b.HasKey("ClientProductAppId", "ProductAddOnId");

                    b.HasIndex("ProductAddOnId");

                    b.ToTable("ClientProductAddOn","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ClientProductApp", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId");

                    b.Property<int>("MaxSiteCount");

                    b.Property<string>("ProductId");

                    b.Property<DateTime?>("SubscriptionEndUTC");

                    b.Property<DateTime>("SubscriptionStartUTC");

                    b.Property<int>("SubscriptionType");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ProductId");

                    b.ToTable("ClientProductApp","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ContentCategory", b =>
                {
                    b.Property<string>("ContentType");

                    b.Property<string>("ContentId");

                    b.Property<string>("CategoryId");

                    b.HasKey("ContentType", "ContentId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("ContentCategory","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ContentClaim", b =>
                {
                    b.Property<string>("ContentType");

                    b.Property<string>("ContentId");

                    b.Property<string>("OwnerId");

                    b.Property<string>("ClaimType");

                    b.Property<int>("OwnerLevel");

                    b.HasKey("ContentType", "ContentId", "OwnerId", "ClaimType");

                    b.ToTable("ContentClaim","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ContentNode", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentTreeId");

                    b.Property<string>("CssColumnSize");

                    b.Property<int>("Index");

                    b.Property<string>("JsonStyle");

                    b.Property<bool>("Locked");

                    b.Property<string>("ParentId");

                    b.Property<string>("ViewId");

                    b.Property<string>("WidgetId");

                    b.Property<string>("WidgetType");

                    b.Property<string>("Zone");

                    b.HasKey("Id");

                    b.HasIndex("ContentTreeId");

                    b.HasIndex("ParentId");

                    b.ToTable("ContentNode","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ContentTag", b =>
                {
                    b.Property<string>("ContentType");

                    b.Property<string>("ContentId");

                    b.Property<string>("TagName");

                    b.HasKey("ContentType", "ContentId", "TagName");

                    b.ToTable("ContentTag","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ContentTree", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentId");

                    b.Property<string>("ContentType");

                    b.Property<string>("VersionCode");

                    b.HasKey("Id");

                    b.ToTable("ContentTree","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ContentVersion", b =>
                {
                    b.Property<string>("ContentId");

                    b.Property<string>("VersionCode");

                    b.Property<string>("ContentType");

                    b.Property<DateTime>("Created");

                    b.Property<int>("Status");

                    b.Property<string>("UserId");

                    b.Property<string>("VersionLabel");

                    b.HasKey("ContentId", "VersionCode");

                    b.ToTable("ContentVersion","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.DocumentCategory", b =>
                {
                    b.Property<string>("DocumentId");

                    b.Property<string>("CategoryId");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("DocumentTagId");

                    b.HasKey("DocumentId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("DocumentCategory","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.DocumentLibrary", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LibraryType");

                    b.Property<string>("Location");

                    b.Property<string>("OwnerId");

                    b.HasKey("Id");

                    b.ToTable("DocumentLibrary","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.DocumentTag", b =>
                {
                    b.Property<string>("DocumentId");

                    b.Property<string>("TagName");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("DocumentTagId");

                    b.Property<string>("TagId");

                    b.HasKey("DocumentId", "TagName");

                    b.HasIndex("TagId");

                    b.ToTable("DocumentTag","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.FileDocument", b =>
                {
                    b.Property<string>("DocumentId")
                        .ValueGeneratedOnAdd();

                    b.Property<long>("ContentLength");

                    b.Property<DateTime>("Created");

                    b.Property<string>("Description");

                    b.Property<string>("FileName");

                    b.Property<string>("FileType");

                    b.Property<string>("Title");

                    b.HasKey("DocumentId");

                    b.ToTable("FileDocument","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Folder", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<string>("DocumentLibraryId");

                    b.Property<string>("DocumentType");

                    b.Property<int>("FolderFlags");

                    b.Property<string>("FolderType");

                    b.Property<bool>("IsDeleted");

                    b.Property<bool>("IsSystemFolder");

                    b.Property<string>("OwnerId");

                    b.Property<int>("OwnerLevel");

                    b.Property<string>("ParentId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("DocumentLibraryId");

                    b.HasIndex("ParentId");

                    b.ToTable("Folder","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.FolderCategory", b =>
                {
                    b.Property<string>("FolderId");

                    b.Property<string>("CategoryId");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Id");

                    b.HasKey("FolderId", "CategoryId");

                    b.HasIndex("CategoryId");

                    b.ToTable("FolderCategory","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.FolderItem", b =>
                {
                    b.Property<string>("FolderId");

                    b.Property<string>("DocumentId");

                    b.Property<bool>("AllowComments");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Id")
                        .IsRequired();

                    b.Property<bool>("InheritSecurity");

                    b.Property<bool>("InheritSharing");

                    b.Property<bool>("InheritTags");

                    b.Property<bool>("IsDeleted");

                    b.Property<int>("ItemStatus");

                    b.HasKey("FolderId", "DocumentId");

                    b.HasAlternateKey("Id");

                    b.ToTable("FolderItem","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.FolderSharing", b =>
                {
                    b.Property<string>("FolderId");

                    b.Property<string>("SharedFolderId");

                    b.Property<string>("CreatedBy");

                    b.HasKey("FolderId", "SharedFolderId");

                    b.HasIndex("SharedFolderId");

                    b.ToTable("FolderSharing","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.FolderTag", b =>
                {
                    b.Property<string>("FolderId");

                    b.Property<string>("TagName");

                    b.Property<string>("CreatedBy");

                    b.Property<string>("Id");

                    b.HasKey("FolderId", "TagName");

                    b.ToTable("FolderTag","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.NavigationMenu", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SiteId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.ToTable("NavigationMenu","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.NavigationMenuItem", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContentId");

                    b.Property<string>("ContentType");

                    b.Property<string>("ExternalURL");

                    b.Property<string>("NavMenuId");

                    b.Property<int>("Order");

                    b.Property<string>("ParentId");

                    b.Property<int>("TargetType");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("NavMenuId");

                    b.HasIndex("ParentId");

                    b.ToTable("NavigationMenuItem","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Notification", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedUTC");

                    b.Property<string>("EmailBody");

                    b.Property<string>("EmailHeaderId");

                    b.Property<string>("EmailSubject");

                    b.Property<string>("ErrorMsg");

                    b.Property<string>("OwnerId");

                    b.Property<int>("OwnerLevel");

                    b.Property<string>("ProcId");

                    b.Property<DateTime>("ProcStartUTC");

                    b.Property<DateTime>("ProcStopUTC");

                    b.Property<int>("RetryCount");

                    b.Property<DateTime>("ScheduledUTC");

                    b.Property<bool>("SendEmail");

                    b.Property<bool>("SendSms");

                    b.Property<string>("SmsMessage");

                    b.Property<string>("Status")
                        .HasMaxLength(10);

                    b.Property<string>("TimeZoneId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Notification","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.NotificationEmailHeader", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("OwnerId");

                    b.Property<int>("OwnerLevel");

                    b.Property<string>("Path");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("NotificationEmailHeader","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.NotificationEmailLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("EmailAddress");

                    b.Property<string>("NotificationId");

                    b.Property<DateTime>("SentUTC");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("NotificationEmailLog","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.NotificationSmsLog", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("MobileNumber");

                    b.Property<string>("NotificationId");

                    b.Property<DateTime>("SentUTC");

                    b.Property<string>("UserId");

                    b.Property<string>("WirelessProviderId");

                    b.HasKey("Id");

                    b.ToTable("NotificationSmsLog","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.NotificationUnsubscribeCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Code");

                    b.Property<bool>("Confirmed");

                    b.Property<string>("Email");

                    b.Property<DateTime>("ExpirationUTC");

                    b.Property<int>("FailureCount");

                    b.Property<string>("NotificationId");

                    b.Property<DateTime>("SentEmailUTC");

                    b.Property<DateTime>("SentSmsUTC");

                    b.Property<string>("SmsNumber");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("NotificationUnsubscribeCode","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.NotificationUserGroup", b =>
                {
                    b.Property<string>("NotificationId");

                    b.Property<string>("UserGroupId");

                    b.HasKey("NotificationId", "UserGroupId");

                    b.ToTable("NotificationUserGroup","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Page", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("InitialSeedStrategy");

                    b.Property<bool>("IsPrivate");

                    b.Property<string>("Layout");

                    b.Property<string>("PageMasterId");

                    b.Property<string>("ParentPageId");

                    b.Property<string>("Path");

                    b.Property<string>("PublishedVersionCode");

                    b.Property<string>("SiteId");

                    b.Property<string>("Title");

                    b.Property<int>("Type");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("PageMasterId");

                    b.HasIndex("ParentPageId");

                    b.HasIndex("SiteId");

                    b.ToTable("Page","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.PageMaster", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsDefault");

                    b.Property<string>("PreviewPath");

                    b.Property<string>("SiteId");

                    b.Property<string>("TemplateId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("SiteId");

                    b.ToTable("PageMaster","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Product", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("CategoryId");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("SchemaFile");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Product","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ProductAddOn", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Description");

                    b.Property<string>("Name");

                    b.Property<string>("SchemaFile");

                    b.HasKey("Id");

                    b.ToTable("ProductAddOn","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ProductAddOnLink", b =>
                {
                    b.Property<string>("ProductId");

                    b.Property<string>("ProductAddOnId");

                    b.HasKey("ProductId", "ProductAddOnId");

                    b.HasIndex("ProductAddOnId");

                    b.ToTable("ProductAddOnLink","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ProductCategory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("ProductCategory","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Site", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Banner");

                    b.Property<string>("ClientId");

                    b.Property<string>("ClientProductAppId");

                    b.Property<string>("DefaultCultureKey");

                    b.Property<bool>("Published");

                    b.Property<string>("SecurityPoolId");

                    b.Property<string>("SiteTemplateId");

                    b.Property<string>("TenantKey");

                    b.Property<string>("ThemeId");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.HasIndex("ClientProductAppId");

                    b.ToTable("Site","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteCollection", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClientId");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("SiteCollection","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteCollectionMap", b =>
                {
                    b.Property<string>("SiteCollectionId");

                    b.Property<string>("SiteId");

                    b.HasKey("SiteCollectionId", "SiteId");

                    b.HasIndex("SiteId");

                    b.ToTable("SiteCollectionMap","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteCulture", b =>
                {
                    b.Property<string>("SiteId");

                    b.Property<string>("CultureKey");

                    b.HasKey("SiteId", "CultureKey");

                    b.ToTable("SiteCulture","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteDirectory", b =>
                {
                    b.Property<string>("DirectoryId");

                    b.Property<string>("SiteId");

                    b.Property<bool>("Edit");

                    b.Property<bool>("View");

                    b.HasKey("DirectoryId", "SiteId");

                    b.HasIndex("SiteId");

                    b.ToTable("SiteDirectory","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteDomain", b =>
                {
                    b.Property<string>("SiteId");

                    b.Property<string>("DomainKey");

                    b.Property<bool>("IsDefault");

                    b.HasKey("SiteId", "DomainKey");

                    b.ToTable("SiteDomain","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteSetting", b =>
                {
                    b.Property<string>("SiteId");

                    b.Property<string>("FieldName");

                    b.Property<string>("FieldType");

                    b.Property<string>("Value");

                    b.HasKey("SiteId", "FieldName");

                    b.ToTable("SiteSetting","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Tag", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.Property<string>("TagName");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Tag","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.UserGroup", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("AllowPublicEnrollment");

                    b.Property<string>("CreatedBy");

                    b.Property<DateTime>("CreatedDT");

                    b.Property<string>("Name");

                    b.Property<string>("OwnerId");

                    b.Property<int>("OwnerLevel");

                    b.Property<int>("UserGroupType");

                    b.HasKey("Id");

                    b.ToTable("UserGroup","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.UserGroupHierarchy", b =>
                {
                    b.Property<string>("ParentGroupId");

                    b.Property<string>("ChildGroupId");

                    b.Property<int>("MembershipType");

                    b.HasKey("ParentGroupId", "ChildGroupId");

                    b.ToTable("UserGroupHierarchy","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.UserGroupMembership", b =>
                {
                    b.Property<string>("UserGroupId");

                    b.Property<string>("UserId");

                    b.Property<int>("AccessLevel");

                    b.Property<string>("AddedBy");

                    b.Property<DateTime>("AddedDT");

                    b.Property<bool>("AllowEmailMessaging");

                    b.Property<bool>("AllowSmsMessaging");

                    b.HasKey("UserGroupId", "UserId");

                    b.ToTable("UserGroupMembership","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Security.GroupResourceClaim", b =>
                {
                    b.Property<string>("ResourceId");

                    b.Property<string>("ResourceType");

                    b.Property<string>("ClaimType");

                    b.Property<string>("GroupId");

                    b.Property<string>("GroupProviderType");

                    b.HasKey("ResourceId", "ResourceType", "ClaimType", "GroupId");

                    b.ToTable("GroupResourceClaim","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Security.ResourceClaim", b =>
                {
                    b.Property<string>("ResourceId");

                    b.Property<string>("ResourceType");

                    b.Property<string>("ClaimType");

                    b.Property<string>("UserId");

                    b.Property<string>("FolderId");

                    b.HasKey("ResourceId", "ResourceType", "ClaimType", "UserId");

                    b.HasIndex("FolderId");

                    b.ToTable("ResourceClaim","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Widgets.Models.JsonWidget", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ModelJson");

                    b.Property<string>("ModelType");

                    b.HasKey("Id");

                    b.ToTable("JsonWidget","cms");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ClientProductAddOn", b =>
                {
                    b.HasOne("Angelo.Connect.Models.ClientProductApp", "ClientProductApp")
                        .WithMany("AddOns")
                        .HasForeignKey("ClientProductAppId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Connect.Models.ProductAddOn", "ProductAddOn")
                        .WithMany("ClientProductAddOns")
                        .HasForeignKey("ProductAddOnId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.ClientProductApp", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Client", "Client")
                        .WithMany("ClientProductApps")
                        .HasForeignKey("ClientId");

                    b.HasOne("Angelo.Connect.Models.Product", "Product")
                        .WithMany("ClientProductApps")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ContentCategory", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Category", "Category")
                        .WithMany("ContentMap")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.ContentNode", b =>
                {
                    b.HasOne("Angelo.Connect.Models.ContentTree", "ContentTree")
                        .WithMany("ContentNodes")
                        .HasForeignKey("ContentTreeId");

                    b.HasOne("Angelo.Connect.Models.ContentNode", "ParentNode")
                        .WithMany("ChildNodes")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.DocumentCategory", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Category", "Category")
                        .WithMany("DocumentMap")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.DocumentTag", b =>
                {
                    b.HasOne("Angelo.Connect.Models.FileDocument", "Document")
                        .WithMany("Tags")
                        .HasForeignKey("DocumentId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Connect.Models.Tag", "Tag")
                        .WithMany()
                        .HasForeignKey("TagId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Folder", b =>
                {
                    b.HasOne("Angelo.Connect.Models.DocumentLibrary", "DocumentLibrary")
                        .WithMany("Folders")
                        .HasForeignKey("DocumentLibraryId");

                    b.HasOne("Angelo.Connect.Models.Folder", "ParentFolder")
                        .WithMany("ChildFolders")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.FolderCategory", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Category", "Category")
                        .WithMany("FolderMap")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Connect.Models.Folder", "Folder")
                        .WithMany("CategoryMap")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.FolderItem", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Folder", "Folder")
                        .WithMany("Items")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.FolderSharing", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Folder", "Folder")
                        .WithMany("Sharing")
                        .HasForeignKey("FolderId");

                    b.HasOne("Angelo.Connect.Models.Folder", "SharedFolder")
                        .WithMany()
                        .HasForeignKey("SharedFolderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.FolderTag", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Folder", "Folder")
                        .WithMany("Tags")
                        .HasForeignKey("FolderId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.NavigationMenu", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Site", "Site")
                        .WithMany("NavigationMenus")
                        .HasForeignKey("SiteId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.NavigationMenuItem", b =>
                {
                    b.HasOne("Angelo.Connect.Models.NavigationMenu", "NavMenu")
                        .WithMany("MenuItems")
                        .HasForeignKey("NavMenuId");

                    b.HasOne("Angelo.Connect.Models.NavigationMenuItem", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Page", b =>
                {
                    b.HasOne("Angelo.Connect.Models.PageMaster", "MasterPage")
                        .WithMany("DerivedPages")
                        .HasForeignKey("PageMasterId");

                    b.HasOne("Angelo.Connect.Models.Page", "ParentPage")
                        .WithMany("ChildPages")
                        .HasForeignKey("ParentPageId");

                    b.HasOne("Angelo.Connect.Models.Site", "Site")
                        .WithMany("Pages")
                        .HasForeignKey("SiteId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.PageMaster", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Site", "Site")
                        .WithMany("PageMasters")
                        .HasForeignKey("SiteId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Product", b =>
                {
                    b.HasOne("Angelo.Connect.Models.ProductCategory", "Category")
                        .WithMany("Products")
                        .HasForeignKey("CategoryId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.ProductAddOnLink", b =>
                {
                    b.HasOne("Angelo.Connect.Models.ProductAddOn", "ProductAddOn")
                        .WithMany("ProductAddOnLinks")
                        .HasForeignKey("ProductAddOnId");

                    b.HasOne("Angelo.Connect.Models.Product", "Product")
                        .WithMany("ProductAddOnLinks")
                        .HasForeignKey("ProductId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.Site", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Client", "Client")
                        .WithMany("Sites")
                        .HasForeignKey("ClientId");

                    b.HasOne("Angelo.Connect.Models.ClientProductApp", "ClientProductApp")
                        .WithMany("Sites")
                        .HasForeignKey("ClientProductAppId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteCollection", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Client", "Client")
                        .WithMany("SiteCollections")
                        .HasForeignKey("ClientId");
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteCollectionMap", b =>
                {
                    b.HasOne("Angelo.Connect.Models.SiteCollection", "SiteCollection")
                        .WithMany("SiteCollectionMaps")
                        .HasForeignKey("SiteCollectionId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Connect.Models.Site", "Site")
                        .WithMany("SiteCollectionMaps")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteCulture", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Site", "Site")
                        .WithMany("Cultures")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteDirectory", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Site", "Site")
                        .WithMany("SiteDirectories")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteDomain", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Site", "Site")
                        .WithMany("Domains")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.SiteSetting", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Site", "Site")
                        .WithMany("SiteSettings")
                        .HasForeignKey("SiteId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Models.UserGroupMembership", b =>
                {
                    b.HasOne("Angelo.Connect.Models.UserGroup", "UserGroup")
                        .WithMany("Memberships")
                        .HasForeignKey("UserGroupId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Connect.Security.ResourceClaim", b =>
                {
                    b.HasOne("Angelo.Connect.Models.Folder")
                        .WithMany("Security")
                        .HasForeignKey("FolderId");
                });
        }
    }
}
