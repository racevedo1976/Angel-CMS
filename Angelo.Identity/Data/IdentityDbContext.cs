using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

using Angelo.Identity.Models;

namespace Angelo.Identity
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<Tenant> Tenants { get; set; }

        public DbSet<TenantUri> TenantUris { get; set; }

        public DbSet<SecurityPool> SecurityPools { get; set; }
        public DbSet<Directory> Directories { get; set; }
        public DbSet<DirectoryMap> DirectoryMap { get; set; }
        public DbSet<User> Users { get; set; }        
        public DbSet<UserLogin> UserLogins { get; set; }      
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoleClaim> RoleClaims { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserClaim> UserClaims { get; set; }
        public DbSet<GroupClaim> GroupClaims { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupMembership> GroupMemberships { get; set; }
        public DbSet<UserMembership> UserMemberships { get; set; }
        public DbSet<LdapDomain> LdapDomains { get; set; }
        public DbSet<LdapMapping> LdapMappings { get; set; }
        public DbSet<UserSecurity> UserSecurity { get; set; }
        public DbSet<RoleSecurity> RoleSecurity { get; set; }
        public DbSet<WirelessProvider> WirelessProviders { get; set; }

        // required since rc2
        public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            configureAuthModel(modelBuilder);             
        }

        //TODO: Set Cascade Deletion Rules
        private static void configureAuthModel(ModelBuilder builder)
        {

            builder.Entity<Tenant>(entity => {
                entity.ToTable("Tenant", "auth").HasKey(x => x.Id);

                entity.HasMany(x => x.Users).WithOne(y => y.Tenant).HasForeignKey(y => y.TenantId);
                entity.HasMany(x => x.Directories).WithOne(y => y.Tenant).HasForeignKey(y => y.TenantId);
                entity.HasMany(x => x.SecurityPools).WithOne(y => y.Tenant).HasForeignKey(y => y.TenantId);
                entity.HasMany(x => x.OidcUris).WithOne(y => y.Tenant).HasForeignKey(y => y.TenantId);
            });

            builder.Entity<TenantUri>(entity => {
                // where possible, use a clustered key since ensures data is phyiscally ordered on disk
                // to optimize querying based on the fields of the key. Don't waste the clustered index 
                // on PKs for an "Id" field when a natural key combination exists that better supports
                // how data is looked up

                entity.ToTable("TenantUri", "auth").HasKey(x => new { x.TenantId, x.Type, x.Uri });
                entity.HasAlternateKey(x => x.Id);
            });

            builder.Entity<Directory>(entity => {
                entity.ToTable("Directory", "auth").HasKey(x => x.Id);

                entity.HasOne(x => x.LdapDomain).WithOne(y => y.Directory);
                entity.HasMany(x => x.Users).WithOne(y => y.Directory).HasForeignKey(y => y.DirectoryId);
                entity.HasMany(x => x.DirectoryMap).WithOne(y => y.Directory).HasForeignKey(y => y.DirectoryId);
            });

            builder.Entity<DirectoryMap>(entity => {
                entity.ToTable("DirectoryMap", "auth").HasKey(x => new { x.DirectoryId, x.PoolId });
            });

            builder.Entity<SecurityPool>(pool => {
                pool.ToTable("SecurityPool", "auth").HasKey(x => x.PoolId);
                pool.HasMany(x => x.Roles).WithOne(x => x.SecurityPool).HasForeignKey(x => x.PoolId);
                pool.HasMany(x => x.DirectoryMap).WithOne(x => x.SecurityPool).HasForeignKey(x => x.PoolId);
                pool.HasMany(x => x.Memberships).WithOne(x => x.SecurityPool).HasForeignKey(x => x.PoolId);
                pool.HasMany(x => x.ChildPools).WithOne(x => x.ParentPool).HasForeignKey(x => x.ParentPoolId);
            });

            builder.Entity<User>(user =>
            {
                user.ToTable("User", "auth").HasKey(x => x.Id);
                user.Property(x => x.IsActive).ForSqlServerHasDefaultValue(true);

                user.HasMany(x => x.Claims).WithOne(y => y.User).HasForeignKey(y => y.UserId);
                user.HasMany(x => x.Memberships).WithOne(y => y.User).HasForeignKey(y => y.UserId);
                user.HasMany(x => x.Security).WithOne(y => y.User).HasForeignKey(y => y.UserId);
                user.HasMany(x => x.Roles).WithOne(y => y.User).HasForeignKey(y => y.UserId);

                user.Ignore(x => x.IsLockedOut);
            });
                             
            builder.Entity<Role>(role =>
            {
                role.ToTable("Role", "auth").HasKey(x => x.Id);
                role.HasMany(x => x.UserRoles).WithOne(y => y.Role).HasForeignKey(y =>  y.RoleId );
                role.HasMany(x => x.RoleClaims).WithOne(y => y.Role) .HasForeignKey(y =>  y.RoleId );
                role.HasMany(x => x.LdapMappings).WithOne(y => y.Role).HasForeignKey(y =>  y.RoleId );
                role.Ignore(x => x.NormalizedName);
            });
            
            builder.Entity<UserMembership>( membership =>
            {
                membership.ToTable("UserMembership", "auth").HasKey(x => new { x.PoolId, x.UserId });
                membership.ToTable("UserMembership", "auth").HasAlternateKey(x => x.MemberId);          
            });

            builder.Entity<UserLogin>(userLogin =>
            {
                userLogin.ToTable("UserLogin", "auth").HasKey(x => new { x.LoginProvider, x.ProviderKey });
            });

            builder.Entity<UserClaim>(userClaim =>
            {
                userClaim.ToTable("UserClaim", "auth").HasKey(x => new { x.UserId, x.ClaimType, x.ClaimValue });
                userClaim.Property(x => x.Id).UseSqlServerIdentityColumn();           
            });

            builder.Entity<GroupClaim>(groupClaim =>
            {
                groupClaim.ToTable("GroupClaim", "auth").HasKey(x => new { x.GroupId, x.ClaimType, x.ClaimValue });
                
            });

            builder.Entity<Group>(group =>
            {
                group.ToTable("Group", "auth").HasKey(x => new { x.Id });

            });

            builder.Entity<GroupMembership>(groupMembership =>
            {
                groupMembership.ToTable("GroupMembership", "auth").HasKey(x => new { x.GroupId, x.UserId });
                groupMembership.HasOne(x => x.Group).WithMany(x => x.GroupMemberships).HasForeignKey(x => x.GroupId);
                groupMembership.HasOne(x => x.User).WithMany(x => x.GroupMemberships).HasForeignKey(x => x.UserId);
            });

            builder.Entity<UserRole>(userRole =>
            {
                userRole.ToTable("UserRole", "auth").HasKey(x => new { x.UserId, x.RoleId });
            });

            builder.Entity<RoleClaim>(roleClaim =>
            {
                roleClaim.ToTable("RoleClaim", "auth").HasKey(x => new { x.RoleId, x.ClaimType, x.ClaimValue });
                roleClaim.Property(x => x.Id).UseSqlServerIdentityColumn();
            });

            builder.Entity<LdapMapping>(ldapMapping =>
            {
                ldapMapping.ToTable("LdapMapping", "auth").HasKey(x => new { x.RoleId, x.ObjectGuid });
                ldapMapping.Property(x => x.Id).UseSqlServerIdentityColumn();
            });

            builder.Entity<LdapDomain>(ldapDomain =>
            {
                ldapDomain.ToTable("LdapDomain", "auth").HasKey(x => x.Id);
            });

            builder.Entity<UserSecurity>(userSecurity =>
            {
                userSecurity.ToTable("UserSecurity", "auth").HasKey(x => x.Id);
            });

            builder.Entity<RoleSecurity>(roleSecurity =>
            {
                roleSecurity.ToTable("RoleSecurity", "auth").HasKey(x => x.Id);
            });

            builder.Entity<WirelessProvider>(p =>
            {
                p.ToTable("WirelessProvider", "auth").HasKey(x => x.Id);
            });

        }

        // WJC 2017-05-18: Moved to Angelo.Connect.Web\Extensions to avoid circular reference issues.
        //public void EnsureDbExists()
        //{
        //    Database.Migrate();
        //    this.EnsureSeeded();
        //}
    }

}
