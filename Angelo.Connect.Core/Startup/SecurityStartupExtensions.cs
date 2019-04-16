using System;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using Angelo.Connect.Security.Services;
using SiteLevel = Angelo.Connect.Security.Policies.SiteLevel;
using ClientLevel = Angelo.Connect.Security.Policies.ClientLevel;
using CorpLevel = Angelo.Connect.Security.Policies.CorpLevel;


using Angelo.Connect.Security.Policies;
using Angelo.Connect.UI.Components;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class SecurityStartupExtensions
    {
        public static IServiceCollection AddUserContext<TUserContextAccessor>(this IServiceCollection services)
        where TUserContextAccessor : class, IContextAccessor<UserContext>
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.AddScoped<IContextAccessor<UserContext>, TUserContextAccessor>();
            services.AddScoped<TUserContextAccessor>();
           
            services.AddTransient<UserContext>(serviceProvider => {
                var userContextAccessor = serviceProvider.GetService<TUserContextAccessor>();

                return userContextAccessor.GetContext();
            });

            return services;
        }
        
        public static IServiceCollection AddSecurityPolicies(this IServiceCollection services)
        {
            // Corp Level Policies
            services.RegisterPolicy<CorpLevel.ClientsCreateRequirements, CorpLevel.ClientsCreateHandler>(PolicyNames.CorpClientsCreate);
            services.RegisterPolicy<CorpLevel.ClientsEditRequirements, CorpLevel.ClientsEditHandler>(PolicyNames.CorpClientsEdit);
            services.RegisterPolicy<CorpLevel.ClientsReadRequirements, CorpLevel.ClientsReadHandler>(PolicyNames.CorpClientsRead);
            services.RegisterPolicy<CorpLevel.ClientsDeleteRequirements, CorpLevel.ClientsDeleteHandler>(PolicyNames.CorpClientsDelete);
            services.RegisterPolicy<CorpLevel.CorpSiteCanManageRequirement, CorpLevel.CorpSiteCanManageHandler>(PolicyNames.CorpSiteCanManage);

            services.RegisterPolicy<CorpLevel.ProductsEditRequirements, CorpLevel.ProductsEditHandler>(PolicyNames.CorpProductsEdit);
            services.RegisterPolicy<CorpLevel.ProductsReadRequirements, CorpLevel.ProductsReadHandler>(PolicyNames.CorpProductsRead);
            services.RegisterPolicy<CorpLevel.ProductsDeleteRequirements, CorpLevel.ProductsDeleteHandler>(PolicyNames.CorpProductsDelete);
            services.RegisterPolicy<CorpLevel.ProductsCreateRequirements, CorpLevel.ProductsCreateHandler>(PolicyNames.CorpProductsCreate);
            services.RegisterPolicy<CorpLevel.ProductsAssignRequirements, CorpLevel.ProductsAssignHandler>(PolicyNames.CorpProductsMap);

            services.RegisterPolicy<CorpLevel.LibraryBrowseRequirement, CorpLevel.LibraryBrowseHandler>(PolicyNames.CorpLibraryBrowse);
            services.RegisterPolicy<CorpLevel.LibraryCreateFoldersRequirement, CorpLevel.LibraryCreateFoldersHandler>(PolicyNames.CorpLibraryCreateFolders);
            services.RegisterPolicy<CorpLevel.LibraryDeleteFilesRequirement, CorpLevel.LibraryDeleteFilesHandler>(PolicyNames.CorpLibraryDeleteFiles);
            services.RegisterPolicy<CorpLevel.LibraryDeleteFoldersRequirement, CorpLevel.LibraryDeleteFoldersHandler>(PolicyNames.CorpLibraryDeleteFolders);
            services.RegisterPolicy<CorpLevel.LibraryEditFilesRequirement, CorpLevel.LibraryEditFilesHandler>(PolicyNames.CorpLibraryEditFiles);
            services.RegisterPolicy<CorpLevel.LibraryEditFoldersRequirement, CorpLevel.LibraryEditFoldersHandler>(PolicyNames.CorpLibraryEditFolders);
            services.RegisterPolicy<CorpLevel.LibraryManageSecurityRequirement, CorpLevel.LibraryManageSecurityHandler>(PolicyNames.CorpLibraryManageSecurity);
            services.RegisterPolicy<CorpLevel.LibraryUploadFilesRequirement, CorpLevel.LibraryUploadFilesHandler>(PolicyNames.CorpLibraryUploadFiles);
            services.RegisterPolicy<CorpLevel.CorpUserRequirement, CorpLevel.CorpUserHandler>(PolicyNames.CorpUser);

            // Client Level Policies
            services.RegisterPolicy<ClientLevel.AnyAdminClaimRequirement, ClientLevel.AnyAdminClaimCoreHandler>(PolicyNames.ClientLevelAny);

            services.RegisterPolicy<ClientLevel.SitesReadRequirement, ClientLevel.SitesReadHandler>(PolicyNames.ClientSitesRead);
            services.RegisterPolicy<ClientLevel.SitesEditRequirement, ClientLevel.SitesEditHandler>(PolicyNames.ClientSitesEdit);
            services.RegisterPolicy<ClientLevel.SitesCreateRequirement, ClientLevel.SitesCreateHandler>(PolicyNames.ClientSitesCreate);
            services.RegisterPolicy<ClientLevel.SitesDeleteRequirement, ClientLevel.SitesDeleteHandler>(PolicyNames.ClientSitesDelete);

            services.RegisterPolicy<ClientLevel.DirectoriesReadRequirement, ClientLevel.DirectoriesReadHandler>(PolicyNames.ClientDirectoriesRead);
            services.RegisterPolicy<ClientLevel.DirectoriesEditRequirement, ClientLevel.DirectoriesEditHandler>(PolicyNames.ClientDirectoriesEdit);
            services.RegisterPolicy<ClientLevel.DirectoriesCreateRequirement, ClientLevel.DirectoriesCreateHandler>(PolicyNames.ClientDirectoriesCreate);
            services.RegisterPolicy<ClientLevel.DirectoriesDeleteRequirement, ClientLevel.DirectoriesDeleteHandler>(PolicyNames.ClientDirectoriesDelete);

            services.RegisterPolicy<ClientLevel.UserProfileCreateRequirement, ClientLevel.UserProfileCreateHandler>(PolicyNames.ClientUsersCreate);
            services.RegisterPolicy<ClientLevel.UserProfileReadRequirement, ClientLevel.UserProfileReadHandler>(PolicyNames.ClientUsersRead);
            services.RegisterPolicy<ClientLevel.UserProfileEditRequirement, ClientLevel.UserProfileEditHandler>(PolicyNames.ClientUsersEdit);

            services.RegisterPolicy<ClientLevel.RolesCreateRequirement, ClientLevel.RolesCreateHandler>(PolicyNames.ClientRolesCreate);
            services.RegisterPolicy<ClientLevel.RolesReadRequirement, ClientLevel.RolesReadHandler>(PolicyNames.ClientRolesRead);
            services.RegisterPolicy<ClientLevel.RolesEditRequirement, ClientLevel.RolesEditHandler>(PolicyNames.ClientRolesEdit);
            services.RegisterPolicy<ClientLevel.RolesDeleteRequirement, ClientLevel.RolesDeleteHandler>(PolicyNames.ClientRolesDelete);

            services.RegisterPolicy<ClientLevel.GroupsReadRequirements, ClientLevel.GroupsReadHandler>(PolicyNames.ClientGroupsRead);
            services.RegisterPolicy<ClientLevel.GroupsEditRequirements, ClientLevel.GroupsEditHandler>(PolicyNames.ClientGroupsEdit);
            services.RegisterPolicy<ClientLevel.GroupsCreateRequirements, ClientLevel.GroupsCreateHandler>(PolicyNames.ClientGroupsCreate);
            services.RegisterPolicy<ClientLevel.GroupsDeleteRequirements, ClientLevel.GroupsDeleteHandler>(PolicyNames.ClientGroupsDelete);

            services.RegisterPolicy<ClientLevel.NotificationGroupsReadRequirement, ClientLevel.NotificationGroupsReadHandler>(PolicyNames.ClientNotificationGroupRead);
            services.RegisterPolicy<ClientLevel.NotificationGroupsEditRequirement, ClientLevel.NotificationGroupsEditHandler>(PolicyNames.ClientNotificationGroupEdit);
            services.RegisterPolicy<ClientLevel.NotificationGroupsCreateRequirement, ClientLevel.NotificationGroupsCreateHandler>(PolicyNames.ClientNotificationGroupCreate);
            services.RegisterPolicy<ClientLevel.NotificationGroupsDeleteRequirement, ClientLevel.NotificationGroupsDeleteHandler>(PolicyNames.ClientNotificationGroupDelete);

            services.RegisterPolicy<ClientLevel.LibraryReadRequirement, ClientLevel.LibraryReadHandler>(PolicyNames.ClientLibraryRead);
            services.RegisterPolicy<ClientLevel.LibraryOwnerRequirement, ClientLevel.LibraryOwnerHandler>(PolicyNames.ClientLibraryOwner);
            

            // Site Level Policies
            services.RegisterPolicy<SiteLevel.AnyAdminClaimRequirement, SiteLevel.AnyAdminClaimCoreHandler>(PolicyNames.SiteLevelAny);

            services.AddAuthorization(options =>
            {
                options.AddPolicy(PolicyNames.SiteAnyClaimToSite, policy =>
                    policy.Requirements.Add(new SiteLevel.AnyAdminClaimToSiteRequirement()));
            });
            services.AddSingleton<IAuthorizationHandler, SiteLevel.AnyAdminClaimToSiteHandler>();


            services.RegisterPolicy<SiteLevel.UserProfileCreateRequirement, SiteLevel.UserProfileCreateHandler>(PolicyNames.SiteUsersCreate);
            services.RegisterPolicy<SiteLevel.UserProfileReadRequirement, SiteLevel.UserProfileReadHandler>(PolicyNames.SiteUsersRead);
            services.RegisterPolicy<SiteLevel.UserProfileEditRequirement, SiteLevel.UserProfileEditHandler>(PolicyNames.SiteUsersEdit);

            services.RegisterPolicy<SiteLevel.RolesCreateRequirement, SiteLevel.RolesCreateHandler>(PolicyNames.SiteRolesCreate);
            services.RegisterPolicy<SiteLevel.RolesReadRequirement, SiteLevel.RolesReadHandler>(PolicyNames.SiteRolesRead);
            services.RegisterPolicy<SiteLevel.RolesEditRequirement, SiteLevel.RolesEditHandler>(PolicyNames.SiteRolesEdit);
            services.RegisterPolicy<SiteLevel.RolesDeleteRequirement, SiteLevel.RolesDeleteHandler>(PolicyNames.SiteRolesDelete);

            services.RegisterPolicy<SiteLevel.PagesCreateRequirement, SiteLevel.PagesCreateHandler>(PolicyNames.SitePagesCreate);
            services.RegisterPolicy<SiteLevel.PagesReadRequirement, SiteLevel.PagesReadHandler>(PolicyNames.SitePagesRead);
            services.RegisterPolicy<SiteLevel.PagesEditRequirement, SiteLevel.PagesEditHandler>(PolicyNames.SitePagesEdit);
            services.RegisterPolicy<SiteLevel.PagesDeleteRequirement, SiteLevel.PagesDeleteHandler>(PolicyNames.SitePagesDelete);
            services.RegisterPolicy<SiteLevel.PagesDesignRequirement, SiteLevel.PagesDesignHandler>(PolicyNames.SitePagesDesign);
            services.RegisterPolicy<SiteLevel.PagesPublishRequirement, SiteLevel.PagesPublishHandler>(PolicyNames.SitePagesPublish);

            services.RegisterPolicy<SiteLevel.SiteSettingsReadRequirement, SiteLevel.SiteSettingsReadHandler>(PolicyNames.SiteSettingsRead);
            services.RegisterPolicy<SiteLevel.SiteSettingsEditRequirement, SiteLevel.SiteSettingsEditHandler>(PolicyNames.SiteSettingsEdit);

            services.RegisterPolicy<SiteLevel.GroupsReadRequirements, SiteLevel.GroupsReadHandler>(PolicyNames.SiteGroupsRead);
            services.RegisterPolicy<SiteLevel.GroupsEditRequirements, SiteLevel.GroupsEditHandler>(PolicyNames.SiteGroupsEdit);
            services.RegisterPolicy<SiteLevel.GroupsDeleteRequirements, SiteLevel.GroupsDeleteHandler>(PolicyNames.SiteGroupsDelete);
            services.RegisterPolicy<SiteLevel.GroupsCreateRequirements, SiteLevel.GroupsCreateHandler>(PolicyNames.SiteGroupsCreate);

            services.RegisterPolicy<SiteLevel.NotificationGroupsReadRequirement, SiteLevel.NotificationGroupsReadHandler>(PolicyNames.SiteNotificationGroupRead);
            services.RegisterPolicy<SiteLevel.NotificationGroupsEditRequirement, SiteLevel.NotificationGroupsEditHandler>(PolicyNames.SiteNotificationGroupEdit);
            services.RegisterPolicy<SiteLevel.NotificationGroupsCreateRequirement, SiteLevel.NotificationGroupsCreateHandler>(PolicyNames.SiteNotificationGroupCreate);
            services.RegisterPolicy<SiteLevel.NotificationGroupsDeleteRequirement, SiteLevel.NotificationGroupsDeleteHandler>(PolicyNames.SiteNotificationGroupDelete);

            services.RegisterPolicy<SiteLevel.MasterPagesReadRequirement, SiteLevel.MasterPagesReadHandler>(PolicyNames.SiteMasterPagesRead);
            services.RegisterPolicy<SiteLevel.MasterPagesEditRequirement, SiteLevel.MasterPagesEditHandler>(PolicyNames.SiteMasterPagesEdit);
            services.RegisterPolicy<SiteLevel.MasterPagesCreateRequirement, SiteLevel.MasterPagesCreateHandler>(PolicyNames.SiteMasterPagesCreate);
            services.RegisterPolicy<SiteLevel.MasterPagesDeleteRequirement, SiteLevel.MasterPagesDeleteHandler>(PolicyNames.SiteMasterPagesDelete);

            services.RegisterPolicy<SiteLevel.SiteTemplatesEditRequirement, SiteLevel.SiteTemplatesEditHandler>(PolicyNames.SiteTemplatesEdit);
            services.RegisterPolicy<SiteLevel.SiteTemplatesReadRequirement, SiteLevel.SiteTemplatesReadHandler>(PolicyNames.SiteTemplatesRead);

            services.RegisterPolicy<SiteLevel.NavMenusReadRequirement, SiteLevel.NavMenusReadHandler>(PolicyNames.SiteNavMenusRead);
            services.RegisterPolicy<SiteLevel.NavMenusEditRequirement, SiteLevel.NavMenusEditHandler>(PolicyNames.SiteNavMenusEdit);
            services.RegisterPolicy<SiteLevel.NavMenusCreateRequirement, SiteLevel.NavMenusCreateHandler>(PolicyNames.SiteNavMenusCreate);
            services.RegisterPolicy<SiteLevel.NavMenusDeleteRequirement, SiteLevel.NavMenusDeleteHandler>(PolicyNames.SiteNavMenusDelete);

            services.RegisterPolicy<SiteLevel.LibraryReadRequirement, SiteLevel.LibraryReadHandler>(PolicyNames.SiteLibraryRead);
            services.RegisterPolicy<SiteLevel.LibraryOwnerRequirement, SiteLevel.LibraryOwnerHandler>(PolicyNames.SiteLibraryOwner);
           

            #region Sample / Stub Policies 

            //TODO: Cleanup / Remove all sample policies except StubPolicy

            services.AddAuthorization(options => {

                // stub policy created as placeholder until actual policies are written
                options.AddPolicy(PolicyNames.StubPolicy, policy => policy.RequireAuthenticatedUser());

                //sample policy for "Can I access roles" page
                options.AddPolicy("CanAccessRoles", policy => policy.RequireClaim("access-corporate-roles"));
            });

            #endregion

            return services;
        }

        public static IServiceCollection AddClaimsSecurity(this IServiceCollection services)
        {
            services.AddTransient<SecurityUserRoleClaims>();
            services.AddTransient<SecurityClaimManager>();

            services.AddTransient<ISecurityPermissionProvider, PermissionProviderSiteLevel>();
            services.AddTransient<ISecurityPermissionProvider, PermissionProviderClientLevel>();
            services.AddTransient<ISecurityPermissionProvider, PermissionProviderCorpLevel>();
            services.AddTransient<PermissionProviderFactory>();

            // Other Security Services
            services.AddTransient<PageSecurityService>();

            return services;
        }

        public static IServiceCollection RegisterPolicy<TRequirement, THandler>(this IServiceCollection services, string policyName)
        where TRequirement : IAuthorizationRequirement
        where THandler : AuthorizationHandler<TRequirement>       
        {
            // Define Policy
            //
            // TODO: Ensure claims are cached (per user) to optimize handler performance & invalidate/reload if the user's security is updated
            var requirement = Activator.CreateInstance<TRequirement>();

            services.AddAuthorization(options => {
                options.AddPolicy(policyName, policy => policy.Requirements.Add(requirement));
            });

            // Register Handler
            //
            // Handlers should be scoped per request since security should be deterministic throughout the duration of the request.
            // Otherwise could lead to problems with partial request fulfillment 
            // 
            // E.g. Some components pass while others fail if user security changes mid-request (plausible long term)
            //
            services.AddScoped<IAuthorizationHandler, THandler>();

            return services;
        }
    }
}
