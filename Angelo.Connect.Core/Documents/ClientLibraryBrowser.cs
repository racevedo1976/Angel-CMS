using Angelo.Connect.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Models;
using Angelo.Identity.Models;
using Angelo.Connect.Extensions;
using Angelo.Connect.Security;
using Angelo.Connect.Configuration;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Claims;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Documents;

namespace Angelo.Connect.Services
{
    public class ClientLibraryBrowser : IContentBrowser
    {
        private IFolderManager<FileDocument> _folderManager;
        private string iconClassForDisplay = "fa fa-folder";
        private string _contentBrowserType = typeof(ClientLibraryBrowser).Name;
        private string _componentViewName = "FileDetails";
        private string _rootFolderName = "Client Library";
        private AdminContext _adminContext;
        private string ownerId;
        private UserContext _userContext;

        public ClientLibraryBrowser(IFolderManager<FileDocument> folderManager,
           IContextAccessor<AdminContext> adminContextAccessor,
           IContextAccessor<UserContext> userContextAccessor)
        {
            _folderManager = folderManager;
            _adminContext = adminContextAccessor.GetContext();
            _userContext = userContextAccessor.GetContext();

            ownerId = _adminContext?.ClientId;
        }

        public async Task<bool> HasAccessToLibrary()
        {
            //Claim types required to have access to this library browser
            var ValidClaimTypes = new string[]
           {
                ClientClaimTypes.AppLibraryOwner,
                ClientClaimTypes.AppLibraryRead,
                ClientClaimTypes.PrimaryAdmin
           };

            var hasAccess = ResolveAuthorization(ValidClaimTypes);

            return await Task.FromResult<bool>(hasAccess);
        }

        private bool ResolveAuthorization(string[] claimTypes)
        {
            // Build Claims with Client and Corp values since any would be valid
            var validClaims = claimTypes.SelectMany(type => new Claim[]
            {
                    new Claim(type, _adminContext?.ClientId),
                    new Claim(type, _adminContext.CorpId)
            });

            // Aegis does not return permission level claims to keep the ticket small, etc.
            // Instead, these are loaded locally into the UserContext
            return _userContext.SecurityClaims.Any(
                userClaim => validClaims.Any(
                    validClaim => userClaim.Type == validClaim.Type && userClaim.Value == validClaim.Value
                )
            );
        }

        public string ContentComponentView
        {
            get
            {
                return _componentViewName;
            }
        }

        public async Task<TreeView> GetRootTreeView(string userId)
        {
            if (!await HasAccessToLibrary())
                return null;

            var locationResolver = new DocumentPhysicalLocationResolver("Client", _adminContext.ClientId, _adminContext.SiteId, ownerId);

            //ensure Client library is created if not done yet for whatever reason
            if ((await _folderManager.GetDocumentLibraryAsync(ownerId) == null))
            {
                await _folderManager.CreateDocumentLibrary(ownerId, "Client", locationResolver.Resolve());
            }

            var rootUserFolder = await _folderManager.GetRootFolderAsync(ownerId);
            var childFolders = await _folderManager.GetFoldersRecursivelyAsync(rootUserFolder, false);
            ((Folder)rootUserFolder).ChildFolders = childFolders;
            
            return FolderToTreeView(rootUserFolder);
        }


        public TreeView FolderToTreeView(IFolder folder)
        {
            var folderInstance = folder as Folder;
            List<TreeView> folderItems = new List<TreeView>();

            //var menu = [];
            //var menuItems = [];
            var hasItems = false;
            var folderNode = new TreeView()
            {
                Title = folder.Title == "" ? _rootFolderName : folder.Title,
                Id = folder.Id,
                IconClass = folder.IsSystemFolder && folder.Title == "Trash" ? Icons.IconType.Trashcan.ToString() : Icons.IconType.Folder.ToString(),
                ContentBrowserType = _contentBrowserType
            };

            foreach (var folderItem in folderInstance?.ChildFolders)
            {
                hasItems = true;
                folderItems.Add(FolderToTreeView(folderItem));
            };

            if (hasItems)
                folderNode.Items = folderItems;

            return folderNode;


        }

        public string GetComponentContentView()
        {
            return _componentViewName;
        }

        public string GetLibraryRootName()
        {
            return _rootFolderName;
        }

        public async Task<bool> IsAnythingShared(string userId)
        {
            return await Task.FromResult<bool>(false);
            //return (await _sharedContentProvider.IsAnythingShared(userId, typeof(FileDocument).Name)) || await IsAnythingSharedInUserGroups(userId);

        }

        public async Task<IEnumerable<IContentType>> GetContent(string id)
        {

            var folder = await _folderManager.GetFolderAsync(id, true);
            IEnumerable<IContentType> results = (await _folderManager.GetDocumentsAsync(folder)).ToList();
            IEnumerable<IContentType> resultFolders = ((Folder)folder).ChildFolders;

            return resultFolders.Union(results);
        }

        public async Task<IEnumerable<IContentType>> GetSharedContent(string userId)
        {
            return null;
            //// ***** Get FileDocuments
            //var sharedDocumentList = new List<IContentType>();
            //var sharedItems = (await _sharedContentProvider.GetSharedContent<FileDocument>(userId)).ToList();
            ////add group shared ones
            //sharedItems.AddRange(await GetSharedClaimsInUserGroups<FileDocument>(userId));

            //foreach (var sharedDocumentClaim in sharedItems)
            //{
            //    var sharedDocument = (await _documentService.GetAsync(sharedDocumentClaim.ResourceId));
            //    if (sharedDocument != null)
            //    {
            //        sharedDocumentList.Add(sharedDocument);
            //    }
            //}


            //IEnumerable<IContentType> results = sharedDocumentList;

            ////****** Get Shared Folders
            //var folderList = new List<Folder>();
            //var sharedFolderClaims = (await _sharedContentProvider.GetSharedContent<Folder>(userId)).ToList();
            ////add in share folder as well
            //sharedFolderClaims.AddRange(await GetSharedClaimsInUserGroups<Folder>(userId));

            //foreach (var folderClaim in sharedFolderClaims)
            //{
            //    var folder = (await _folderManager.GetFolderAsync(folderClaim.ResourceId));
            //    if (folder != null)
            //    {
            //        folderList.Add((Folder)folder);
            //    }
            //}
            //sharedDocumentList.AddRange(folderList);

            //return sharedDocumentList;
        }

        public TreeView GetSharedContentTreeNode()
        {
            return new TreeView()
            {
                Title = "Document",
                Id = "sharedcontent",
                IconClass = iconClassForDisplay,
                ContentBrowserType = _contentBrowserType
            };
        }

        private async Task<bool> IsAnythingSharedInUserGroups(string userId)
        {
            return await Task.FromResult<bool>(false);  
            //return (await GetSharedClaimsInUserGroups<FileDocument>(userId)).Any() || (await GetSharedClaimsInUserGroups<Folder>(userId)).Any();
        }

        private async Task<List<ResourceClaim>> GetSharedClaimsInUserGroups<IContentType>(string userId)
        {
            throw new NotImplementedException();

            //var sharedGroupClaims = new List<GroupResourceClaim>();
            //var sharedDocuments = new List<FileDocument>();
            //foreach (var provider in _groupProviders)
            //{
            //    var groupsForUser = provider.GetGroups();

            //    foreach (var group in groupsForUser)
            //    {
            //        sharedGroupClaims.AddRange(await _sharedContentProvider.GetGroupSharedContent(((Group)group).Id, typeof(IContentType).Name, provider.GetType().Name));
            //    }
            //}

            //return sharedGroupClaims.Select(x => new ResourceClaim
            //{
            //    ClaimType = x.ClaimType,
            //    ResourceId = x.ResourceId
            //}).ToList();
        }

        public async Task<bool> CanManageLibrary()
        {
            var ValidClaimTypes = new string[]
             {
                    ClientClaimTypes.AppLibraryOwner,
                    ClientClaimTypes.PrimaryAdmin
             };

            var canManage = ResolveAuthorization(ValidClaimTypes);

            return await Task.FromResult<bool>(canManage);
        }
    }
}
