using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;

using Angelo.Connect.Configuration;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using Angelo.Identity.Models;
using Angelo.Connect.Services;
using Angelo.Common.Extensions;

namespace Angelo.Connect.Web.Extensions
{
    public static class AutoMapperStartupExtensions
    {
        public static IServiceCollection AddAutoMapperMappings(this IServiceCollection services)
        {

            Mapper.Initialize(config =>
            {

                config.CreateMissingTypeMaps = true;
                

                // Example how to let automapper discover the mappings based on property names
                config.CreateMap<NavigationMenu, NavigationMenuViewModel>();
                config.CreateMap<UserClaim, ClaimViewModel>();
                config.CreateMap<RoleClaim, ClaimViewModel>();
                config.CreateMap<Role, RoleViewModel>();
                config.CreateMap<Role, PoolRoleViewModel>();
                config.CreateMap<UserRole, RoleUserViewModel>().ReverseMap();
                config.CreateMap<UserMembership, UserViewModel>();
                config.CreateMap<SiteDomain, CorpSiteDomainViewModel>().ReverseMap();
                config.CreateMap<SiteCulture, SiteCultureViewModel>().ReverseMap();
                config.CreateMap<SiteCollection, SiteCollectionsViewModel>().ReverseMap();
                config.CreateMap<UserGroup, UserGroupViewModel>().ReverseMap();
                config.CreateMap<UserGroupMembership, UserGroupMembershipViewModel>().ReverseMap();
                config.CreateMap<User, UserViewModel>();
                config.CreateMap<User, UserProfileViewModel>().ReverseMap();
                config.CreateMap<UserMembership, UserViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.User.Id))
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                    .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                    .ForMember(dest => dest.EmailConfirmed, opt => opt.MapFrom(src => src.User.EmailConfirmed))
                    .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                    .ForMember(dest => dest.PhoneNumberConfirmed, opt => opt.MapFrom(src => src.User.PhoneNumberConfirmed))
                    .ReverseMap();
                config.CreateMap<Product, ProductViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                    .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category == null? string.Empty: src.Category.Name))
                    .ReverseMap();
                config.CreateMap<ClientProductApp, ProductViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductId))
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Product.Name))
                    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Product.Description))
                    .ForMember(dest => dest.SchemaFile, opt => opt.MapFrom(src => src.Product.SchemaFile))
                    .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Product.CategoryId))
                    .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Product.Category == null ? string.Empty : src.Product.Category.Name))
                    .ReverseMap();
                config.CreateMap<ClientProductApp, ClientProductAppViewModel>()
                    .ForMember(dest => dest.ClientProductAppId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
                    .ForMember(dest => dest.SubscriptionType, opt => opt.MapFrom(src => src.SubscriptionType))
                    .ForMember(dest => dest.SubscriptionStartUTC, opt => opt.MapFrom(src => src.SubscriptionStartUTC))
                    .ForMember(dest => dest.SubscriptionEndUTC, opt => opt.MapFrom(src => src.SubscriptionEndUTC))
                    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                    .ReverseMap();

                config.CreateMap<NavigationMenu, NavigationMenuViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.SiteId))
                    .ReverseMap();

                config.CreateMap<Folder, ClientSharedFolderViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ReverseMap();

                config.CreateMap<NavigationMenuItem, NavigationMenuItemViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.NavMenuId, opt => opt.MapFrom(src => src.NavMenuId))
                    .ForMember(dest => dest.ParentId, opt => opt.MapFrom(src => src.ParentId))
                    .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                    .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
                    .ForMember(dest => dest.ExternalURL, opt => opt.MapFrom(src => src.ExternalURL))
                    .ForMember(dest => dest.ContentId, opt => opt.MapFrom(src => src.ContentId))
                    .ReverseMap();

                //Custom mapping of a Complex Model to a Simplified view model
                config.CreateMap<ClientProductApp, ClientProductAppViewModel>()
                    .ForMember(dest => dest.ClientProductAppId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
                    //.ForMember(dest => dest.Disabled, opt => opt.MapFrom(src => src.Disabled))
                    //.ForMember(dest => dest.SubscriptionType, opt => opt.MapFrom(src => src.SubscriptionType))
                    //.ForMember(dest => dest.SubscriptionStart, opt => opt.MapFrom(src => src.SubscriptionStart))
                    //.ForMember(dest => dest.SubscriptionEnd, opt => opt.MapFrom(src => src.SubscriptionEnd))
                    .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Product.Id))
                    .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                    //.ForMember(dest => dest.Product.Id, opt => opt.MapFrom(src => src.ProductId))
                    //.ForMember(dest => dest.Product.Type, opt => opt.MapFrom(src => src.Product.Type))
                    //.ForMember(dest => dest.Product.CategoryName, opt => opt.MapFrom(src => src.Product.Category.Name))
                    .ReverseMap();

                // Site Context Used by Saas
                config.CreateMap<Site, SiteContext>()
                    .ForMember(dest => dest.SiteId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.SiteTitle, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.SiteBanner, opt => opt.MapFrom(src => src.Banner))
                    .ForMember(dest => dest.TenantKey, opt => opt.MapFrom(src => src.TenantKey))
                    .ForMember(dest => dest.SecurityPoolId, opt => opt.MapFrom(src => src.SecurityPoolId))
                    .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client))
                    .ForMember(dest => dest.Template, opt => opt.MapFrom(src => src.SiteTemplate));


                config.CreateMap<Site, SiteViewModel>()
                    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.TenantKey, opt => opt.MapFrom(src => src.TenantKey))
                    .ForMember(dest => dest.SecurityPoolId, opt => opt.MapFrom(src => src.SecurityPoolId))
                    .ForMember(dest => dest.ClientId, opt => opt.MapFrom(src => src.ClientId))
                    .ForMember(dest => dest.ClientName, opt => opt.MapFrom(src => src.Client.Name))
                    .ForMember(dest => dest.ClientProductAppId, opt => opt.MapFrom(src => src.ClientProductAppId))
                    .ForMember(dest => dest.AppId, opt => opt.MapFrom(src => src.ClientProductAppId))
                    .ForMember(dest => dest.AppName, opt => opt.MapFrom(src => src.ClientProductApp == null ? string.Empty : src.ClientProductApp.Title))
                    //.ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                    //.ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ClientProductApp == null ? string.Empty : (src.ClientProduct.Product == null ? string.Empty : src.ClientProduct.Product.Name)))
                    .ForMember(dest => dest.Banner, opt => opt.MapFrom(src => src.Banner))
                    .ForMember(dest => dest.TemplateId, opt => opt.MapFrom(src => src.SiteTemplateId))
                    .ForMember(dest => dest.TemplateName, opt => opt.MapFrom(src => src.SiteTemplate == null ? string.Empty : src.SiteTemplate.Title))
                    .ForMember(dest => dest.Published, opt => opt.MapFrom(src => src.Published))
                    .ForMember(dest => dest.ThemeId, opt => opt.MapFrom(src => src.ThemeId))
                    .ForMember(dest => dest.Domains, opt => opt.MapFrom(src => src.Domains))
                    .ForMember(dest => dest.CultureKeys, opt => opt.MapFrom(src => src.Cultures.Select(x => x.CultureKey)))
                    .ForMember(dest => dest.SiteCollections, opt => opt.MapFrom(src => src.SiteCollectionMaps.Select(x => x.SiteCollection)))
                    .ReverseMap();

                config.CreateMap<Folder, SharedFolderViewModel>()
                    .ForMember(dest => dest.DocumentType, opt => opt.MapFrom(src => src.DocumentType))
                    .ForMember(dest => dest.FolderId, opt => opt.MapFrom(src => src.Id))
                    .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                    .ForMember(dest => dest.ParentFolderId, opt => opt.MapFrom(src => src.ParentId == null ? string.Empty : src.ParentId))
                    .ReverseMap();

                config.CreateMap<Page, PageViewModel>()
                    .ForMember(dest => dest.PageMasterTitle, opt => opt.MapFrom(src => src.MasterPage.Title))
                    .ForMember(dest => dest.ParentPageTitle, opt => opt.Ignore());

                config.CreateMap<PageViewModel, Page>();

                config.CreateMap<UserSecurity, PageSecurityUserViewModel>()
                    .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                    .ReverseMap();

                // Notification Models
                config.CreateMap<Notification, NotificationListItemViewModel>()
                    .ForMember(dest => dest.CreatedDT, opt => opt.MapFrom(src => TimeZoneHelper.ConvertFromUTC(src.CreatedUTC, src.TimeZoneId, DateTime.Now)))
                    .ForMember(dest => dest.ScheduledDT, opt => opt.MapFrom(src => TimeZoneHelper.ConvertFromUTC(src.ScheduledUTC, src.TimeZoneId, DateTime.Now)));

                config.CreateMap<NotificationListItemViewModel, Notification>()
                    .ForMember(dest => dest.CreatedUTC, opt => opt.MapFrom(src => TimeZoneHelper.ConvertToUTC(src.CreatedDT, src.TimeZoneId, DateTime.UtcNow)))
                    .ForMember(dest => dest.ScheduledUTC, opt => opt.MapFrom(src => TimeZoneHelper.ConvertToUTC(src.ScheduledDT, src.TimeZoneId, DateTime.UtcNow)));

                config.CreateMap<Notification, NotificationDetailsViewModel>()
                   .ForMember(dest => dest.CreatedUTC, opt => opt.MapFrom(src => TimeZoneHelper.ConvertFromUTC(src.CreatedUTC, src.TimeZoneId, DateTime.Now)))
                   .ForMember(dest => dest.ScheduledDT, opt => opt.MapFrom(src => TimeZoneHelper.ConvertFromUTC(src.ScheduledUTC, src.TimeZoneId, DateTime.Now)));

                config.CreateMap<NotificationDetailsViewModel, Notification>()
                   .ForMember(dest => dest.CreatedUTC, opt => opt.MapFrom(src => TimeZoneHelper.ConvertToUTC(src.CreatedUTC, src.TimeZoneId, DateTime.UtcNow)))
                   .ForMember(dest => dest.ScheduledUTC, opt => opt.MapFrom(src => TimeZoneHelper.ConvertToUTC(src.ScheduledDT, src.TimeZoneId, DateTime.UtcNow)));

            });


            return services;
        }


    }
}