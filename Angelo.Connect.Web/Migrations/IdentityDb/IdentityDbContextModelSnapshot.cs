using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Angelo.Identity;
using Angelo.Identity.Models;

namespace Angelo.Connect.Web.Migrations.IdentityDb
{
    [DbContext(typeof(IdentityDbContext))]
    partial class IdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Angelo.Identity.Models.Directory", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("TenantId");

                    b.HasKey("Id");

                    b.HasIndex("TenantId");

                    b.ToTable("Directory","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.DirectoryMap", b =>
                {
                    b.Property<string>("DirectoryId");

                    b.Property<string>("PoolId");

                    b.HasKey("DirectoryId", "PoolId");

                    b.HasIndex("PoolId");

                    b.ToTable("DirectoryMap","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.Group", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Group","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.GroupClaim", b =>
                {
                    b.Property<string>("GroupId");

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("Id");

                    b.HasKey("GroupId", "ClaimType", "ClaimValue");

                    b.ToTable("GroupClaim","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.GroupMembership", b =>
                {
                    b.Property<string>("GroupId");

                    b.Property<string>("UserId");

                    b.Property<string>("Id");

                    b.HasKey("GroupId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("GroupMembership","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.LdapDomain", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("DirectoryId");

                    b.Property<string>("Domain");

                    b.Property<string>("Host");

                    b.Property<string>("LdapBaseDn");

                    b.Property<string>("Password");

                    b.Property<bool>("UseSsl");

                    b.Property<string>("User");

                    b.HasKey("Id");

                    b.HasIndex("DirectoryId")
                        .IsUnique();

                    b.ToTable("LdapDomain","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.LdapMapping", b =>
                {
                    b.Property<string>("RoleId");

                    b.Property<string>("ObjectGuid");

                    b.Property<string>("DistinguishedName");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name");

                    b.HasKey("RoleId", "ObjectGuid");

                    b.ToTable("LdapMapping","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.Role", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<bool>("IsDefault");

                    b.Property<bool>("IsLocked");

                    b.Property<string>("Name");

                    b.Property<string>("Path");

                    b.Property<string>("PoolId");

                    b.HasKey("Id");

                    b.HasIndex("PoolId");

                    b.ToTable("Role","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.RoleClaim", b =>
                {
                    b.Property<string>("RoleId");

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.HasKey("RoleId", "ClaimType", "ClaimValue");

                    b.ToTable("RoleClaim","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.RoleSecurity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ResourceId");

                    b.Property<string>("ResourceType");

                    b.Property<string>("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("RoleSecurity","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.SecurityPool", b =>
                {
                    b.Property<string>("PoolId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("LdapFilterGroup");

                    b.Property<string>("Name");

                    b.Property<string>("ParentPoolId");

                    b.Property<int>("PoolType");

                    b.Property<string>("TenantId");

                    b.HasKey("PoolId");

                    b.HasIndex("ParentPoolId");

                    b.HasIndex("TenantId");

                    b.ToTable("SecurityPool","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.Tenant", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("OidcBanner");

                    b.Property<string>("OidcTitle");

                    b.HasKey("Id");

                    b.ToTable("Tenant","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.TenantUri", b =>
                {
                    b.Property<string>("TenantId");

                    b.Property<int>("Type");

                    b.Property<string>("Uri");

                    b.Property<string>("Id")
                        .IsRequired();

                    b.HasKey("TenantId", "Type", "Uri");

                    b.HasAlternateKey("Id");

                    b.ToTable("TenantUri","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<DateTime>("BirthDate");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("DirectoryId");

                    b.Property<string>("DisplayName");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("FirstName");

                    b.Property<bool>("IsActive")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:DefaultValue", true);

                    b.Property<string>("LastName");

                    b.Property<string>("LdapGuid");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<bool>("MustChangePassword");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("Suffix");

                    b.Property<string>("Title");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.Property<string>("WirelessProviderId");

                    b.HasKey("Id");

                    b.HasIndex("DirectoryId");

                    b.ToTable("User","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserClaim", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("ClaimScope");

                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("UserMembershipPoolId");

                    b.Property<string>("UserMembershipUserId");

                    b.HasKey("UserId", "ClaimType", "ClaimValue");

                    b.HasIndex("UserMembershipPoolId", "UserMembershipUserId");

                    b.ToTable("UserClaim","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserLogin", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("UserLogin","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserMembership", b =>
                {
                    b.Property<string>("PoolId");

                    b.Property<string>("UserId");

                    b.Property<bool>("Disabled");

                    b.Property<string>("MemberId")
                        .IsRequired();

                    b.HasKey("PoolId", "UserId");

                    b.HasAlternateKey("MemberId");

                    b.HasIndex("UserId");

                    b.ToTable("UserMembership","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserRole", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.Property<string>("MembershipPoolId");

                    b.Property<string>("MembershipUserId");

                    b.Property<string>("RoleId1");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.HasIndex("RoleId1");

                    b.HasIndex("MembershipPoolId", "MembershipUserId");

                    b.ToTable("UserRole","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserSecurity", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ResourceId");

                    b.Property<string>("ResourceType");

                    b.Property<string>("UserId");

                    b.Property<string>("UserMembershipPoolId");

                    b.Property<string>("UserMembershipUserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.HasIndex("UserMembershipPoolId", "UserMembershipUserId");

                    b.ToTable("UserSecurity","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.WirelessProvider", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("SmsDomain");

                    b.HasKey("Id");

                    b.ToTable("WirelessProvider","auth");
                });

            modelBuilder.Entity("Angelo.Identity.Models.Directory", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Tenant", "Tenant")
                        .WithMany("Directories")
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.DirectoryMap", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Directory", "Directory")
                        .WithMany("DirectoryMap")
                        .HasForeignKey("DirectoryId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Identity.Models.SecurityPool", "SecurityPool")
                        .WithMany("DirectoryMap")
                        .HasForeignKey("PoolId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Identity.Models.GroupMembership", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Group", "Group")
                        .WithMany("GroupMemberships")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Identity.Models.User", "User")
                        .WithMany("GroupMemberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Identity.Models.LdapDomain", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Directory", "Directory")
                        .WithOne("LdapDomain")
                        .HasForeignKey("Angelo.Identity.Models.LdapDomain", "DirectoryId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.LdapMapping", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Role", "Role")
                        .WithMany("LdapMappings")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Identity.Models.Role", b =>
                {
                    b.HasOne("Angelo.Identity.Models.SecurityPool", "SecurityPool")
                        .WithMany("Roles")
                        .HasForeignKey("PoolId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.RoleClaim", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Role", "Role")
                        .WithMany("RoleClaims")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Identity.Models.RoleSecurity", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.SecurityPool", b =>
                {
                    b.HasOne("Angelo.Identity.Models.SecurityPool", "ParentPool")
                        .WithMany("ChildPools")
                        .HasForeignKey("ParentPoolId");

                    b.HasOne("Angelo.Identity.Models.Tenant", "Tenant")
                        .WithMany("SecurityPools")
                        .HasForeignKey("TenantId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.TenantUri", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Tenant", "Tenant")
                        .WithMany("OidcUris")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Identity.Models.User", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Directory", "Directory")
                        .WithMany("Users")
                        .HasForeignKey("DirectoryId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserClaim", b =>
                {
                    b.HasOne("Angelo.Identity.Models.User", "User")
                        .WithMany("Claims")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Identity.Models.UserMembership")
                        .WithMany("UserClaims")
                        .HasForeignKey("UserMembershipPoolId", "UserMembershipUserId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserLogin", b =>
                {
                    b.HasOne("Angelo.Identity.Models.User", "User")
                        .WithMany("Logins")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserMembership", b =>
                {
                    b.HasOne("Angelo.Identity.Models.SecurityPool", "SecurityPool")
                        .WithMany("Memberships")
                        .HasForeignKey("PoolId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Identity.Models.User", "User")
                        .WithMany("Memberships")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserRole", b =>
                {
                    b.HasOne("Angelo.Identity.Models.Role", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Identity.Models.Role")
                        .WithMany("Users")
                        .HasForeignKey("RoleId1");

                    b.HasOne("Angelo.Identity.Models.User", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Angelo.Identity.Models.UserMembership", "Membership")
                        .WithMany("UserRoles")
                        .HasForeignKey("MembershipPoolId", "MembershipUserId");
                });

            modelBuilder.Entity("Angelo.Identity.Models.UserSecurity", b =>
                {
                    b.HasOne("Angelo.Identity.Models.User", "User")
                        .WithMany("Security")
                        .HasForeignKey("UserId");

                    b.HasOne("Angelo.Identity.Models.UserMembership")
                        .WithMany("UserSecurity")
                        .HasForeignKey("UserMembershipPoolId", "UserMembershipUserId");
                });
        }
    }
}
