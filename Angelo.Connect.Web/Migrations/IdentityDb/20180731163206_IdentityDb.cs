using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Angelo.Connect.Web.Migrations.IdentityDb
{
    public partial class IdentityDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "Group",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Group", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GroupClaim",
                schema: "auth",
                columns: table => new
                {
                    GroupId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: false),
                    ClaimValue = table.Column<string>(nullable: false),
                    Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupClaim", x => new { x.GroupId, x.ClaimType, x.ClaimValue });
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    OidcBanner = table.Column<string>(nullable: true),
                    OidcTitle = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "WirelessProvider",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SmsDomain = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WirelessProvider", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Directory",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    TenantId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Directory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Directory_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "auth",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SecurityPool",
                schema: "auth",
                columns: table => new
                {
                    PoolId = table.Column<string>(nullable: false),
                    LdapFilterGroup = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    ParentPoolId = table.Column<string>(nullable: true),
                    PoolType = table.Column<int>(nullable: false),
                    TenantId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SecurityPool", x => x.PoolId);
                    table.ForeignKey(
                        name: "FK_SecurityPool_SecurityPool_ParentPoolId",
                        column: x => x.ParentPoolId,
                        principalSchema: "auth",
                        principalTable: "SecurityPool",
                        principalColumn: "PoolId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SecurityPool_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "auth",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TenantUri",
                schema: "auth",
                columns: table => new
                {
                    TenantId = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Uri = table.Column<string>(nullable: false),
                    Id = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUri", x => new { x.TenantId, x.Type, x.Uri });
                    table.UniqueConstraint("AK_TenantUri_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TenantUri_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalSchema: "auth",
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LdapDomain",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DirectoryId = table.Column<string>(nullable: true),
                    Domain = table.Column<string>(nullable: true),
                    Host = table.Column<string>(nullable: true),
                    LdapBaseDn = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    UseSsl = table.Column<bool>(nullable: false),
                    User = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LdapDomain", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LdapDomain_Directory_DirectoryId",
                        column: x => x.DirectoryId,
                        principalSchema: "auth",
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "User",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    BirthDate = table.Column<DateTime>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    DirectoryId = table.Column<string>(nullable: true),
                    DisplayName = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    FirstName = table.Column<string>(nullable: true),
                    IsActive = table.Column<bool>(nullable: false, defaultValue: true),
                    LastName = table.Column<string>(nullable: true),
                    LdapGuid = table.Column<string>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    MustChangePassword = table.Column<bool>(nullable: false),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    Suffix = table.Column<string>(nullable: true),
                    Title = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    WirelessProviderId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                    table.ForeignKey(
                        name: "FK_User_Directory_DirectoryId",
                        column: x => x.DirectoryId,
                        principalSchema: "auth",
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DirectoryMap",
                schema: "auth",
                columns: table => new
                {
                    DirectoryId = table.Column<string>(nullable: false),
                    PoolId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DirectoryMap", x => new { x.DirectoryId, x.PoolId });
                    table.ForeignKey(
                        name: "FK_DirectoryMap_Directory_DirectoryId",
                        column: x => x.DirectoryId,
                        principalSchema: "auth",
                        principalTable: "Directory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DirectoryMap_SecurityPool_PoolId",
                        column: x => x.PoolId,
                        principalSchema: "auth",
                        principalTable: "SecurityPool",
                        principalColumn: "PoolId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Role",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    IsDefault = table.Column<bool>(nullable: false),
                    IsLocked = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Path = table.Column<string>(nullable: true),
                    PoolId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Role", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Role_SecurityPool_PoolId",
                        column: x => x.PoolId,
                        principalSchema: "auth",
                        principalTable: "SecurityPool",
                        principalColumn: "PoolId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GroupMembership",
                schema: "auth",
                columns: table => new
                {
                    GroupId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Id = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GroupMembership", x => new { x.GroupId, x.UserId });
                    table.ForeignKey(
                        name: "FK_GroupMembership_Group_GroupId",
                        column: x => x.GroupId,
                        principalSchema: "auth",
                        principalTable: "Group",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GroupMembership_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserLogin",
                schema: "auth",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogin", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_UserLogin_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserMembership",
                schema: "auth",
                columns: table => new
                {
                    PoolId = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false),
                    MemberId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMembership", x => new { x.PoolId, x.UserId });
                    table.UniqueConstraint("AK_UserMembership_MemberId", x => x.MemberId);
                    table.ForeignKey(
                        name: "FK_UserMembership_SecurityPool_PoolId",
                        column: x => x.PoolId,
                        principalSchema: "auth",
                        principalTable: "SecurityPool",
                        principalColumn: "PoolId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMembership_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LdapMapping",
                schema: "auth",
                columns: table => new
                {
                    RoleId = table.Column<string>(nullable: false),
                    ObjectGuid = table.Column<string>(nullable: false),
                    DistinguishedName = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LdapMapping", x => new { x.RoleId, x.ObjectGuid });
                    table.ForeignKey(
                        name: "FK_LdapMapping_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleClaim",
                schema: "auth",
                columns: table => new
                {
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: false),
                    ClaimValue = table.Column<string>(nullable: false),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleClaim", x => new { x.RoleId, x.ClaimType, x.ClaimValue });
                    table.ForeignKey(
                        name: "FK_RoleClaim_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleSecurity",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ResourceId = table.Column<string>(nullable: true),
                    ResourceType = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleSecurity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RoleSecurity_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserClaim",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: false),
                    ClaimValue = table.Column<string>(nullable: false),
                    ClaimScope = table.Column<string>(nullable: true),
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    UserMembershipPoolId = table.Column<string>(nullable: true),
                    UserMembershipUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserClaim", x => new { x.UserId, x.ClaimType, x.ClaimValue });
                    table.ForeignKey(
                        name: "FK_UserClaim_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserClaim_UserMembership_UserMembershipPoolId_UserMembershipUserId",
                        columns: x => new { x.UserMembershipPoolId, x.UserMembershipUserId },
                        principalSchema: "auth",
                        principalTable: "UserMembership",
                        principalColumns: new[] { "PoolId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "auth",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false),
                    MembershipPoolId = table.Column<string>(nullable: true),
                    MembershipUserId = table.Column<string>(nullable: true),
                    RoleId1 = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_Role_RoleId1",
                        column: x => x.RoleId1,
                        principalSchema: "auth",
                        principalTable: "Role",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRole_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_UserMembership_MembershipPoolId_MembershipUserId",
                        columns: x => new { x.MembershipPoolId, x.MembershipUserId },
                        principalSchema: "auth",
                        principalTable: "UserMembership",
                        principalColumns: new[] { "PoolId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserSecurity",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ResourceId = table.Column<string>(nullable: true),
                    ResourceType = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: true),
                    UserMembershipPoolId = table.Column<string>(nullable: true),
                    UserMembershipUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSecurity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSecurity_User_UserId",
                        column: x => x.UserId,
                        principalSchema: "auth",
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserSecurity_UserMembership_UserMembershipPoolId_UserMembershipUserId",
                        columns: x => new { x.UserMembershipPoolId, x.UserMembershipUserId },
                        principalSchema: "auth",
                        principalTable: "UserMembership",
                        principalColumns: new[] { "PoolId", "UserId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Directory_TenantId",
                schema: "auth",
                table: "Directory",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DirectoryMap_PoolId",
                schema: "auth",
                table: "DirectoryMap",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_GroupMembership_UserId",
                schema: "auth",
                table: "GroupMembership",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LdapDomain_DirectoryId",
                schema: "auth",
                table: "LdapDomain",
                column: "DirectoryId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Role_PoolId",
                schema: "auth",
                table: "Role",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleSecurity_RoleId",
                schema: "auth",
                table: "RoleSecurity",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityPool_ParentPoolId",
                schema: "auth",
                table: "SecurityPool",
                column: "ParentPoolId");

            migrationBuilder.CreateIndex(
                name: "IX_SecurityPool_TenantId",
                schema: "auth",
                table: "SecurityPool",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_User_DirectoryId",
                schema: "auth",
                table: "User",
                column: "DirectoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserClaim_UserMembershipPoolId_UserMembershipUserId",
                schema: "auth",
                table: "UserClaim",
                columns: new[] { "UserMembershipPoolId", "UserMembershipUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserLogin_UserId",
                schema: "auth",
                table: "UserLogin",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserMembership_UserId",
                schema: "auth",
                table: "UserMembership",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                schema: "auth",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId1",
                schema: "auth",
                table: "UserRole",
                column: "RoleId1");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_MembershipPoolId_MembershipUserId",
                schema: "auth",
                table: "UserRole",
                columns: new[] { "MembershipPoolId", "MembershipUserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurity_UserId",
                schema: "auth",
                table: "UserSecurity",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSecurity_UserMembershipPoolId_UserMembershipUserId",
                schema: "auth",
                table: "UserSecurity",
                columns: new[] { "UserMembershipPoolId", "UserMembershipUserId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DirectoryMap",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "GroupClaim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "GroupMembership",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "LdapDomain",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "LdapMapping",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "RoleClaim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "RoleSecurity",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "TenantUri",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserClaim",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserLogin",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserSecurity",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "WirelessProvider",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Group",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Role",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserMembership",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "SecurityPool",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "User",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Directory",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "Tenant",
                schema: "auth");
        }
    }
}
