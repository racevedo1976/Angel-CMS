using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Extensions;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.UI.ViewModels;
using Angelo.Connect.Web.UI.UserConsole.ViewModels;
using Angelo.Connect.UserConsole;
using Angelo.Identity;

namespace Angelo.Connect.Web.UI.UserConsole.Components
{
    public class UserPageComponent : IUserConsoleTreeComponent
    {
        IContextAccessor<UserContext> _userContextAccessor;
        private PageSecurityManager _pageSecurity;
        private PageMasterManager _masterPageManager;
        private PageManager _pageManager;
        private SiteManager _siteManager;
        private ConnectDbContext _connectDbContext;
        private IdentityDbContext _identityDbContext;

        public string ComponentType { get; } = "pages";

        public int ComponentOrder { get; } = 200;

        public string TreeTitle { get; } = "My Pages";

       
        public UserPageComponent(PageManager pageManager, PageSecurityManager pageSecurity, PageMasterManager masterPageManager, SiteManager siteManager, IContextAccessor<UserContext> userContextAccessor, ConnectDbContext connectDbContext, IdentityDbContext identityDbContext)
        {
            _userContextAccessor = userContextAccessor;
            _masterPageManager = masterPageManager;
            _pageSecurity = pageSecurity;
            _pageManager = pageManager;
            _siteManager = siteManager;

            // TODO: Move logic to services
            _connectDbContext = connectDbContext;
            _identityDbContext = identityDbContext;         
        }

        // IUserConsoleTreeInterface
        public async Task<IEnumerable<GenericMenuItem>> GetTreeMenu()
        {
            var menuItems = new List<GenericMenuItem>();

            return await Task.FromResult(menuItems);
        }

        public async Task<IEnumerable<GenericTreeNode>> GetRootNodes()
        {
            var sites = await GetCurrentUserSites();

            return sites.Select(x => new GenericTreeNode
            {
                Id = x.Id,
                Title = x.Title,
                IconCss = Icons.IconType.Globe.ToString(),
                NodeType = "site",
                LinkUrl = "/sys/console/pages?siteId=" + x.Id,
                HasChildren = true
            });
        }

        public async Task<IEnumerable<GenericTreeNode>> GetChildNodes(string nodeId, string nodeType)
        {
            IEnumerable<Page> childPages = null;
            var nodes = new List<GenericTreeNode>();

            if (nodeType == "site")
                childPages = await GetChildPagesBySite(nodeId);
            else
                childPages = await GetChildPagesByPage(nodeId);

            // convert to nodes
            foreach(var page in childPages)
            {
                nodes.Add(new GenericTreeNode
                {
                    Id = page.Id,
                    Title = page.Title,
                    NodeType = "page",
                    LinkUrl = "/sys/console/pages/" + page.Id,
                    IconCss = Icons.IconType.File.ToString(),
                    HasChildren = await TestForChildPages(page.Id)
                });
            }

            return nodes;
        }
              
    
        // Public Methods used by UserPageController
        public async Task<IEnumerable<UserPageViewModel>> GetPageListViewModel(Site site)
        {
            var userPageIds = await GetUserManagedPageIds();

            var sitePages = await _connectDbContext.Pages
                .Where(page =>
                    userPageIds.Contains(page.Id)
                    && page.SiteId == site.Id
                ).ToListAsync();

            return sitePages.ProjectTo<UserPageViewModel>();
        }

        public async Task<UserPageViewModel> GetPageEditViewModel(string pageId)
        {
            var pageIds = await GetUserManagedPageIds();

            if (!pageIds.Contains(pageId))
                throw new UnauthorizedAccessException($"User not unauthorized to edit Page: {pageId}");

            var page = await _pageManager.GetByIdAsync(pageId);
            var site = await _siteManager.GetByIdAsync(page.SiteId);
            var model = page.ProjectTo<UserPageViewModel>();


            model.MasterPages = await GetMasterPagesSelectList(page.SiteId);
            model.Versions = await _pageManager.GetVersions(pageId);
            model.DefaultDomain = await _pageManager.GetDefaultDomain(page.SiteId);

            model.PageSecurityConfig = BuildPageSecurityOptions(site, page);
            model.PagePrivacyConfig = BuildPagePrivacyOptions(site, page);

            return model;
        }

        public async Task<UserPageViewModel> GetPageCreateViewModel(string parentId)
        {
            var pageIds = await GetUserManagedPageIds();

            if (!pageIds.Contains(parentId))
                throw new UnauthorizedAccessException($"User not unauthorized to create under Page: {parentId}");

            var parentPage = await _pageManager.GetByIdAsync(parentId);
            var siteId = parentPage.SiteId;

            var model = new UserPageViewModel()
            {
                Id = null,
                SiteId = parentPage.SiteId,
                ParentPageId = parentPage.Id,
                PageMasterId = parentPage.PageMasterId,
                Path = parentPage.Path
            };

            model.Path += model.Path.EndsWith("/") ? "child" : "/child";
            model.DefaultDomain = await _pageManager.GetDefaultDomain(siteId);
            model.MasterPages = await GetMasterPagesSelectList(siteId);

            return model;
        }


        // Private helpers for security, etc - consider moving to a security class
        private IEnumerable<SecurityClaimConfig> BuildPageSecurityOptions(Site site, Page page)
        {
            return new List<SecurityClaimConfig>()
            {
                new SecurityClaimConfig
                {
                        Title = "Page Designers",
                        Description = "Manage who can help design this page.",
                        AllowRoles = true,
                        AllowUsers = true,
                        AllowGroups = true,
                        Claim = new Claim(PageClaimTypes.DesignPage, page.Id),
                        SecurityPoolId = site.SecurityPoolId,
                        ResourceType = typeof(Models.Page).ToString()
                }
            };
        }

        private IEnumerable<SecurityClaimConfig> BuildPagePrivacyOptions(Site site, Page page)
        {
            return new List<SecurityClaimConfig>()
            {
                new SecurityClaimConfig
                {
                        Title = "Page Privacy",
                        Description = "Manage who can view page.",
                        AllowRoles = true,
                        AllowUsers = true,
                        AllowGroups = true,
                        Claim = new Claim(PageClaimTypes.ViewPrivatePage, page.Id),
                        SecurityPoolId = site.SecurityPoolId,
                        ResourceType = typeof(Models.Page).ToString()
                }
            };
        }

        private async Task<IEnumerable<Page>> GetChildPagesBySite(string siteId)
        {
            var userPageIds = await GetUserManagedPageIds();

            // all applicable user pages for this site
            var allPages = await _connectDbContext.Pages.Where(x =>
                x.SiteId == siteId &&
                userPageIds.Contains(x.Id)
            ).ToListAsync();

            // filter out child pages 
            return allPages.Where(page => 
                !allPages.Any(other => page.ParentPageId == other.Id)
            );
        }

        private async Task<IEnumerable<Page>> GetChildPagesByPage(string pageId)
        {
            var userPageIds = await GetUserManagedPageIds();

            // all applicable child pages of the specified page
            return await _connectDbContext.Pages.Where(x =>
                x.ParentPageId == pageId &&
                userPageIds.Contains(x.Id)
            ).ToListAsync();
        }

        private async Task<bool> TestForChildPages(string pageId)
        {
            var userPageIds = await GetUserManagedPageIds();

            return await _connectDbContext.Pages.AnyAsync(x => 
                x.ParentPageId == pageId 
                && userPageIds.Contains(x.Id)
            );
        }

        private async Task<IEnumerable<Site>> GetCurrentUserSites()
        {
            var pageIds = await GetUserManagedPageIds();

            return await _connectDbContext.Pages
                .Where(page => pageIds.Contains(page.Id))
                .Select(page => page.Site)
                .Distinct()
                .ToListAsync();
        }

        private async Task<IEnumerable<string>> GetUserManagedPageIds()
        {
            var userContext = _userContextAccessor.GetContext();
            var securityClaims = userContext.SecurityClaims;

            var pageIds = securityClaims
                .Where(x => x.Type == PageClaimTypes.PageOwner)
                .Select(x => x.Value);

            return await Task.FromResult(pageIds);
        }

        private async Task<SelectList> GetMasterPagesSelectList(string siteId)
        {
            var result = await _masterPageManager.GetMasterPagesAsync(siteId);
            var masterPages = result.Select(x => new { Id = x.Id, Value = x.Title });

            return new SelectList(masterPages, "Id", "Value");
        }

    }

}
