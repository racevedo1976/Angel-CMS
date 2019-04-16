using System;
using Microsoft.EntityFrameworkCore;
using Angelo.Identity;
using Angelo.Identity.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

using Angelo.Identity.Services;
using Angelo.Identity.Abstractions;
using Angelo.Identity.Validators;
using System.Linq;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IdentityStartupExtensions
    {

        public static IServiceCollection AddAngeloIdentity(this IServiceCollection services)
        {
            return services.AddAngeloIdentity(x => { });
        }


        public static IServiceCollection AddAngeloIdentity(this IServiceCollection services, Action<IdentityOptions> options)
        {

            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromDays(7);
            });

            services.AddIdentity<User, Role>(options)
                .AddRoleStore<RoleStore>()
                .AddUserStore<UserStore>()
                .AddUserManager<UserManager>()
                .AddRoleManager<RoleManager>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<ConnectCustomTokenProvider<User>>(nameof(ConnectCustomTokenProvider<User>)); 
                
           
            services.AddTransient<UserStore>();
            services.AddTransient<RoleStore>();
            services.AddTransient<RoleManager>();
            services.AddTransient<UserManager>();
            services.AddTransient<SignInManager>();
            services.AddTransient<PasswordHasher>();
            services.AddTransient<ClaimsFactory>();
            services.AddTransient<SecurityPoolManager>();
            services.AddTransient<PageSecurityManager>();
            services.AddTransient<DirectoryManager>();
            services.AddTransient<GroupManager>();
            services.AddTransient<LdapManager>();
            services.AddTransient<TenantManager>();

            services.AddTransient<IdentityErrorDescriber>();

            //clear out the core default DI registration for IUserValidators
            services.Remove(services.FirstOrDefault(x => x.ServiceType == typeof(IUserValidator<User>)));
            services.AddTransient<IUserValidator<User>,UserWithTenantValidator >();
            services.AddTransient<IUserValidator<User>, EmailTenantValidator>();

            return services;
        }



        public static IServiceCollection AddLdapServices(this IServiceCollection services)
        {

            services.AddSingleton<ILdapService<LdapUser>, LdapService<LdapUser>>();
            services.AddTransient<ISyncService<User, LdapUser>, SyncService<User, LdapUser>>();

            return services;
        }



    }
}
