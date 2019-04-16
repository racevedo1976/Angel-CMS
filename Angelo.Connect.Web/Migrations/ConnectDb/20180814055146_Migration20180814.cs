using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Angelo.Connect.Web.Migrations.ConnectDb
{
    public partial class Migration20180814 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "cms");

            migrationBuilder.CreateTable(
                name: "Category",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true),
                    OwnerLevel = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Category", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Client",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Address1 = table.Column<string>(nullable: true),
                    Address2 = table.Column<string>(nullable: true),
                    AnniversaryDate = table.Column<DateTime>(nullable: false),
                    City = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Notes = table.Column<string>(nullable: true),
                    PostalCode = table.Column<string>(nullable: true),
                    PreferredName = table.Column<string>(nullable: true),
                    SecurityPoolId = table.Column<string>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    TenantKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Client", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentClaim",
                schema: "cms",
                columns: table => new
                {
                    ContentType = table.Column<string>(nullable: false),
                    ContentId = table.Column<string>(nullable: false),
                    OwnerId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: false),
                    OwnerLevel = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentClaim", x => new { x.ContentType, x.ContentId, x.OwnerId, x.ClaimType });
                });

            migrationBuilder.CreateTable(
                name: "ContentTag",
                schema: "cms",
                columns: table => new
                {
                    ContentType = table.Column<string>(nullable: false),
                    ContentId = table.Column<string>(nullable: false),
                    TagName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTag", x => new { x.ContentType, x.ContentId, x.TagName });
                });

            migrationBuilder.CreateTable(
                name: "ContentTree",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ContentId = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    VersionCode = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTree", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentVersion",
                schema: "cms",
                columns: table => new
                {
                    ContentId = table.Column<string>(nullable: false),
                    VersionCode = table.Column<string>(nullable: false),
                    ContentType = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    VersionLabel = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentVersion", x => new { x.ContentId, x.VersionCode });
                });

            migrationBuilder.CreateTable(
                name: "DocumentLibrary",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    LibraryType = table.Column<string>(nullable: true),
                    Location = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentLibrary", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileDocument",
                schema: "cms",
                columns: table => new
                {
                    DocumentId = table.Column<string>(nullable: false),
                    ContentLength = table.Column<long>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FileName = table.Column<string>(nullable: true),
                    FileType = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileDocument", x => x.DocumentId);
                });

            migrationBuilder.CreateTable(
                name: "Notification",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedUTC = table.Column<DateTime>(nullable: false),
                    EmailBody = table.Column<string>(nullable: true),
                    EmailHeaderId = table.Column<string>(nullable: true),
                    EmailSubject = table.Column<string>(nullable: true),
                    ErrorMsg = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    OwnerLevel = table.Column<int>(nullable: false),
                    ProcId = table.Column<string>(nullable: true),
                    ProcStartUTC = table.Column<DateTime>(nullable: false),
                    ProcStopUTC = table.Column<DateTime>(nullable: false),
                    RetryCount = table.Column<int>(nullable: false),
                    ScheduledUTC = table.Column<DateTime>(nullable: false),
                    SendEmail = table.Column<bool>(nullable: false),
                    SendSms = table.Column<bool>(nullable: false),
                    SmsMessage = table.Column<string>(nullable: true),
                    Status = table.Column<string>(maxLength: 10, nullable: true),
                    TimeZoneId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notification", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationEmailHeader",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true),
                    OwnerLevel = table.Column<int>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmailHeader", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationEmailLog",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    EmailAddress = table.Column<string>(nullable: true),
                    NotificationId = table.Column<string>(nullable: true),
                    SentUTC = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationEmailLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationSmsLog",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    MobileNumber = table.Column<string>(nullable: true),
                    NotificationId = table.Column<string>(nullable: true),
                    SentUTC = table.Column<DateTime>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    WirelessProviderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationSmsLog", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationUnsubscribeCode",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(nullable: true),
                    Confirmed = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    ExpirationUTC = table.Column<DateTime>(nullable: false),
                    FailureCount = table.Column<int>(nullable: false),
                    NotificationId = table.Column<string>(nullable: true),
                    SentEmailUTC = table.Column<DateTime>(nullable: false),
                    SentSmsUTC = table.Column<DateTime>(nullable: false),
                    SmsNumber = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationUnsubscribeCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotificationUserGroup",
                schema: "cms",
                columns: table => new
                {
                    NotificationId = table.Column<string>(nullable: false),
                    UserGroupId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotificationUserGroup", x => new { x.NotificationId, x.UserGroupId });
                });

            migrationBuilder.CreateTable(
                name: "ProductAddOn",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SchemaFile = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAddOn", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductCategory",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tag",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false),
                    TagName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tag", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroup",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AllowPublicEnrollment = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    CreatedDT = table.Column<DateTime>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(nullable: true),
                    OwnerLevel = table.Column<int>(nullable: false),
                    UserGroupType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroup", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupHierarchy",
                schema: "cms",
                columns: table => new
                {
                    ParentGroupId = table.Column<string>(nullable: false),
                    ChildGroupId = table.Column<string>(nullable: false),
                    MembershipType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupHierarchy", x => new { x.ParentGroupId, x.ChildGroupId });
                });

            migrationBuilder.CreateTable(
                name: "GroupResourceClaim",
                schema: "cms",
                columns: table => new
                {
                    ResourceId = table.Column<string>(nullable: false),
                    ResourceType = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: false),
                    GroupId = table.Column<string>(nullable: false),
                    GroupProviderType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupResourceClaim", x => new { x.ResourceId, x.ResourceType, x.ClaimType, x.GroupId });
                });

            migrationBuilder.CreateTable(
                name: "JsonWidget",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ModelJson = table.Column<string>(nullable: true),
                    ModelType = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JsonWidget", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ContentCategory",
                schema: "cms",
                columns: table => new
                {
                    ContentType = table.Column<string>(nullable: false),
                    ContentId = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentCategory", x => new { x.ContentType, x.ContentId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_ContentCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cms",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentCategory",
                schema: "cms",
                columns: table => new
                {
                    DocumentId = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DocumentTagId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentCategory", x => new { x.DocumentId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_DocumentCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cms",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteCollection",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SiteCollection_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "cms",
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ContentNode",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ContentTreeId = table.Column<string>(nullable: true),
                    CssColumnSize = table.Column<string>(nullable: true),
                    Index = table.Column<int>(nullable: false),
                    JsonStyle = table.Column<string>(nullable: true),
                    Locked = table.Column<bool>(nullable: false),
                    ParentId = table.Column<string>(nullable: true),
                    ViewId = table.Column<string>(nullable: true),
                    WidgetId = table.Column<string>(nullable: true),
                    WidgetType = table.Column<string>(nullable: true),
                    Zone = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentNode", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ContentNode_ContentTree_ContentTreeId",
                        column: x => x.ContentTreeId,
                        principalSchema: "cms",
                        principalTable: "ContentTree",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ContentNode_ContentNode_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cms",
                        principalTable: "ContentNode",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Folder",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DocumentLibraryId = table.Column<string>(nullable: true),
                    DocumentType = table.Column<string>(nullable: true),
                    FolderFlags = table.Column<int>(nullable: false),
                    FolderType = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsSystemFolder = table.Column<bool>(nullable: false),
                    OwnerId = table.Column<string>(nullable: true),
                    OwnerLevel = table.Column<int>(nullable: false),
                    ParentId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folder_DocumentLibrary_DocumentLibraryId",
                        column: x => x.DocumentLibraryId,
                        principalSchema: "cms",
                        principalTable: "DocumentLibrary",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Folder_Folder_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cms",
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    CategoryId = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SchemaFile = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductCategory_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cms",
                        principalTable: "ProductCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DocumentTag",
                schema: "cms",
                columns: table => new
                {
                    DocumentId = table.Column<string>(nullable: false),
                    TagName = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    DocumentTagId = table.Column<string>(nullable: true),
                    TagId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentTag", x => new { x.DocumentId, x.TagName });
                    table.ForeignKey(
                        name: "FK_DocumentTag_FileDocument_DocumentId",
                        column: x => x.DocumentId,
                        principalSchema: "cms",
                        principalTable: "FileDocument",
                        principalColumn: "DocumentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DocumentTag_Tag_TagId",
                        column: x => x.TagId,
                        principalSchema: "cms",
                        principalTable: "Tag",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserGroupMembership",
                schema: "cms",
                columns: table => new
                {
                    UserGroupId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    AccessLevel = table.Column<int>(nullable: false),
                    AddedBy = table.Column<string>(nullable: true),
                    AddedDT = table.Column<DateTime>(nullable: false),
                    AllowEmailMessaging = table.Column<bool>(nullable: false),
                    AllowSmsMessaging = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserGroupMembership", x => new { x.UserGroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserGroupMembership_UserGroup_UserGroupId",
                        column: x => x.UserGroupId,
                        principalSchema: "cms",
                        principalTable: "UserGroup",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderCategory",
                schema: "cms",
                columns: table => new
                {
                    FolderId = table.Column<string>(nullable: false),
                    CategoryId = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderCategory", x => new { x.FolderId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_FolderCategory_Category_CategoryId",
                        column: x => x.CategoryId,
                        principalSchema: "cms",
                        principalTable: "Category",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FolderCategory_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "cms",
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderItem",
                schema: "cms",
                columns: table => new
                {
                    FolderId = table.Column<string>(nullable: false),
                    DocumentId = table.Column<string>(nullable: false),
                    AllowComments = table.Column<bool>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: false),
                    InheritSecurity = table.Column<bool>(nullable: false),
                    InheritSharing = table.Column<bool>(nullable: false),
                    InheritTags = table.Column<bool>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    ItemStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderItem", x => new { x.FolderId, x.DocumentId });
                    table.UniqueConstraint("AK_FolderItem_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FolderItem_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "cms",
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderSharing",
                schema: "cms",
                columns: table => new
                {
                    FolderId = table.Column<string>(nullable: false),
                    SharedFolderId = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderSharing", x => new { x.FolderId, x.SharedFolderId });
                    table.ForeignKey(
                        name: "FK_FolderSharing_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "cms",
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FolderSharing_Folder_SharedFolderId",
                        column: x => x.SharedFolderId,
                        principalSchema: "cms",
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FolderTag",
                schema: "cms",
                columns: table => new
                {
                    FolderId = table.Column<string>(nullable: false),
                    TagName = table.Column<string>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderTag", x => new { x.FolderId, x.TagName });
                    table.ForeignKey(
                        name: "FK_FolderTag_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "cms",
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ResourceClaim",
                schema: "cms",
                columns: table => new
                {
                    ResourceId = table.Column<string>(nullable: false),
                    ResourceType = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    FolderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResourceClaim", x => new { x.ResourceId, x.ResourceType, x.ClaimType, x.UserId });
                    table.ForeignKey(
                        name: "FK_ResourceClaim_Folder_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "cms",
                        principalTable: "Folder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientProductApp",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClientId = table.Column<string>(nullable: true),
                    MaxSiteCount = table.Column<int>(nullable: false),
                    ProductId = table.Column<string>(nullable: true),
                    SubscriptionEndUTC = table.Column<DateTime>(nullable: true),
                    SubscriptionStartUTC = table.Column<DateTime>(nullable: false),
                    SubscriptionType = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProductApp", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientProductApp_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "cms",
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientProductApp_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "cms",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProductAddOnLink",
                schema: "cms",
                columns: table => new
                {
                    ProductId = table.Column<string>(nullable: false),
                    ProductAddOnId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAddOnLink", x => new { x.ProductId, x.ProductAddOnId });
                    table.ForeignKey(
                        name: "FK_ProductAddOnLink_ProductAddOn_ProductAddOnId",
                        column: x => x.ProductAddOnId,
                        principalSchema: "cms",
                        principalTable: "ProductAddOn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProductAddOnLink_Product_ProductId",
                        column: x => x.ProductId,
                        principalSchema: "cms",
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientProductAddOn",
                schema: "cms",
                columns: table => new
                {
                    ClientProductAppId = table.Column<string>(nullable: false),
                    ProductAddOnId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientProductAddOn", x => new { x.ClientProductAppId, x.ProductAddOnId });
                    table.ForeignKey(
                        name: "FK_ClientProductAddOn_ClientProductApp_ClientProductAppId",
                        column: x => x.ClientProductAppId,
                        principalSchema: "cms",
                        principalTable: "ClientProductApp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClientProductAddOn_ProductAddOn_ProductAddOnId",
                        column: x => x.ProductAddOnId,
                        principalSchema: "cms",
                        principalTable: "ProductAddOn",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Site",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Banner = table.Column<string>(nullable: true),
                    ClientId = table.Column<string>(nullable: true),
                    ClientProductAppId = table.Column<string>(nullable: true),
                    DefaultCultureKey = table.Column<string>(nullable: true),
                    Published = table.Column<bool>(nullable: false),
                    SecurityPoolId = table.Column<string>(nullable: true),
                    SiteTemplateId = table.Column<string>(nullable: true),
                    TenantKey = table.Column<string>(nullable: true),
                    ThemeId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Site", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Site_Client_ClientId",
                        column: x => x.ClientId,
                        principalSchema: "cms",
                        principalTable: "Client",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Site_ClientProductApp_ClientProductAppId",
                        column: x => x.ClientProductAppId,
                        principalSchema: "cms",
                        principalTable: "ClientProductApp",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "NavigationMenu",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    SiteId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationMenu", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NavigationMenu_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "cms",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PageMaster",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    PreviewPath = table.Column<string>(nullable: true),
                    SiteId = table.Column<string>(nullable: true),
                    TemplateId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageMaster", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageMaster_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "cms",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SiteCollectionMap",
                schema: "cms",
                columns: table => new
                {
                    SiteCollectionId = table.Column<string>(nullable: false),
                    SiteId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteCollectionMap", x => new { x.SiteCollectionId, x.SiteId });
                    table.ForeignKey(
                        name: "FK_SiteCollectionMap_SiteCollection_SiteCollectionId",
                        column: x => x.SiteCollectionId,
                        principalSchema: "cms",
                        principalTable: "SiteCollection",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SiteCollectionMap_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "cms",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteCulture",
                schema: "cms",
                columns: table => new
                {
                    SiteId = table.Column<string>(nullable: false),
                    CultureKey = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteCulture", x => new { x.SiteId, x.CultureKey });
                    table.ForeignKey(
                        name: "FK_SiteCulture_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "cms",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteDirectory",
                schema: "cms",
                columns: table => new
                {
                    DirectoryId = table.Column<string>(nullable: false),
                    SiteId = table.Column<string>(nullable: false),
                    Edit = table.Column<bool>(nullable: false),
                    View = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteDirectory", x => new { x.DirectoryId, x.SiteId });
                    table.ForeignKey(
                        name: "FK_SiteDirectory_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "cms",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteDomain",
                schema: "cms",
                columns: table => new
                {
                    SiteId = table.Column<string>(nullable: false),
                    DomainKey = table.Column<string>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteDomain", x => new { x.SiteId, x.DomainKey });
                    table.ForeignKey(
                        name: "FK_SiteDomain_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "cms",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SiteSetting",
                schema: "cms",
                columns: table => new
                {
                    SiteId = table.Column<string>(nullable: false),
                    FieldName = table.Column<string>(nullable: false),
                    FieldType = table.Column<string>(nullable: true),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SiteSetting", x => new { x.SiteId, x.FieldName });
                    table.ForeignKey(
                        name: "FK_SiteSetting_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "cms",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NavigationMenuItem",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ContentId = table.Column<string>(nullable: true),
                    ContentType = table.Column<string>(nullable: true),
                    ExternalURL = table.Column<string>(nullable: true),
                    NavMenuId = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: false),
                    ParentId = table.Column<string>(nullable: true),
                    TargetType = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NavigationMenuItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NavigationMenuItem_NavigationMenu_NavMenuId",
                        column: x => x.NavMenuId,
                        principalSchema: "cms",
                        principalTable: "NavigationMenu",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_NavigationMenuItem_NavigationMenuItem_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "cms",
                        principalTable: "NavigationMenuItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Page",
                schema: "cms",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    InitialSeedStrategy = table.Column<string>(nullable: true),
                    IsPrivate = table.Column<bool>(nullable: false),
                    Layout = table.Column<string>(nullable: true),
                    PageMasterId = table.Column<string>(nullable: true),
                    ParentPageId = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    PublishedVersionCode = table.Column<string>(nullable: true),
                    SiteId = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Page", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Page_PageMaster_PageMasterId",
                        column: x => x.PageMasterId,
                        principalSchema: "cms",
                        principalTable: "PageMaster",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Page_Page_ParentPageId",
                        column: x => x.ParentPageId,
                        principalSchema: "cms",
                        principalTable: "Page",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Page_Site_SiteId",
                        column: x => x.SiteId,
                        principalSchema: "cms",
                        principalTable: "Site",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientProductAddOn_ProductAddOnId",
                schema: "cms",
                table: "ClientProductAddOn",
                column: "ProductAddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientProductApp_ClientId",
                schema: "cms",
                table: "ClientProductApp",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientProductApp_ProductId",
                schema: "cms",
                table: "ClientProductApp",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentCategory_CategoryId",
                schema: "cms",
                table: "ContentCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentNode_ContentTreeId",
                schema: "cms",
                table: "ContentNode",
                column: "ContentTreeId");

            migrationBuilder.CreateIndex(
                name: "IX_ContentNode_ParentId",
                schema: "cms",
                table: "ContentNode",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentCategory_CategoryId",
                schema: "cms",
                table: "DocumentCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentTag_TagId",
                schema: "cms",
                table: "DocumentTag",
                column: "TagId");

            migrationBuilder.CreateIndex(
                name: "IX_Folder_DocumentLibraryId",
                schema: "cms",
                table: "Folder",
                column: "DocumentLibraryId");

            migrationBuilder.CreateIndex(
                name: "IX_Folder_ParentId",
                schema: "cms",
                table: "Folder",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderCategory_CategoryId",
                schema: "cms",
                table: "FolderCategory",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_FolderSharing_SharedFolderId",
                schema: "cms",
                table: "FolderSharing",
                column: "SharedFolderId");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationMenu_SiteId",
                schema: "cms",
                table: "NavigationMenu",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationMenuItem_NavMenuId",
                schema: "cms",
                table: "NavigationMenuItem",
                column: "NavMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_NavigationMenuItem_ParentId",
                schema: "cms",
                table: "NavigationMenuItem",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Page_PageMasterId",
                schema: "cms",
                table: "Page",
                column: "PageMasterId");

            migrationBuilder.CreateIndex(
                name: "IX_Page_ParentPageId",
                schema: "cms",
                table: "Page",
                column: "ParentPageId");

            migrationBuilder.CreateIndex(
                name: "IX_Page_SiteId",
                schema: "cms",
                table: "Page",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_PageMaster_SiteId",
                schema: "cms",
                table: "PageMaster",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_CategoryId",
                schema: "cms",
                table: "Product",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductAddOnLink_ProductAddOnId",
                schema: "cms",
                table: "ProductAddOnLink",
                column: "ProductAddOnId");

            migrationBuilder.CreateIndex(
                name: "IX_Site_ClientId",
                schema: "cms",
                table: "Site",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Site_ClientProductAppId",
                schema: "cms",
                table: "Site",
                column: "ClientProductAppId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteCollection_ClientId",
                schema: "cms",
                table: "SiteCollection",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteCollectionMap_SiteId",
                schema: "cms",
                table: "SiteCollectionMap",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_SiteDirectory_SiteId",
                schema: "cms",
                table: "SiteDirectory",
                column: "SiteId");

            migrationBuilder.CreateIndex(
                name: "IX_ResourceClaim_FolderId",
                schema: "cms",
                table: "ResourceClaim",
                column: "FolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientProductAddOn",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ContentCategory",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ContentClaim",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ContentNode",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ContentTag",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ContentVersion",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "DocumentCategory",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "DocumentTag",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FolderCategory",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FolderItem",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FolderSharing",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FolderTag",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "NavigationMenuItem",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Notification",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "NotificationEmailHeader",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "NotificationEmailLog",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "NotificationSmsLog",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "NotificationUnsubscribeCode",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "NotificationUserGroup",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Page",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ProductAddOnLink",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "SiteCollectionMap",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "SiteCulture",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "SiteDirectory",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "SiteDomain",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "SiteSetting",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "UserGroupHierarchy",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "UserGroupMembership",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "GroupResourceClaim",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ResourceClaim",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "JsonWidget",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ContentTree",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "FileDocument",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Tag",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Category",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "NavigationMenu",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "PageMaster",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ProductAddOn",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "SiteCollection",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "UserGroup",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Folder",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Site",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "DocumentLibrary",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ClientProductApp",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Client",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "Product",
                schema: "cms");

            migrationBuilder.DropTable(
                name: "ProductCategory",
                schema: "cms");
        }
    }
}
