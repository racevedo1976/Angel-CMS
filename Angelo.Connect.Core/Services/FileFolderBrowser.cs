using Angelo.Connect.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Models;
using Angelo.Identity.Models;
using Angelo.Connect.Extensions;
using Angelo.Connect.Security;
using System;
using Angelo.Connect.Documents;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Services
{
    public class FileFolderBrowser: IContentBrowser
    {
        private IFolderManager<FileDocument> _folderManager;
        private string iconClassForDisplay = "fa fa-folder";
        private string _contentBrowserType = typeof(FileFolderBrowser).Name;
        private string _componentViewName = "FileDetails";
        private string _rootFolderName = "My Library";
        private IEnumerable<ISecurityGroupProvider> _groupProviders;
        private ISharedContent _sharedContentProvider;
        private IDocumentService<FileDocument> _documentService;
        private AdminContext _adminContext;

        public FileFolderBrowser(IFolderManager<FileDocument> folderManager, 
            IEnumerable<ISecurityGroupProvider> groupProviders,
            ISharedContent sharedContentProvider,
            IDocumentService<FileDocument> documentService,
            IContextAccessor<AdminContext> adminContextAccessor)
        {
            _folderManager = folderManager;
            _groupProviders = groupProviders;
            _sharedContentProvider = sharedContentProvider;
            _documentService = documentService;
            _adminContext = adminContextAccessor.GetContext();
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
            var locationResolver = new DocumentPhysicalLocationResolver("User", _adminContext.ClientId, "", userId);

            //ensure users has a library
            if ((await _folderManager.GetDocumentLibraryAsync(userId) == null))
            {
                await _folderManager.CreateDocumentLibrary(userId, "User", locationResolver.Resolve());
            }

            var rootUserFolder = await _folderManager.GetRootFolderAsync(userId);
            var childFolders = await _folderManager.GetFoldersRecursivelyAsync(rootUserFolder,false);
          
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
                        Title = folder.Title == "" ? "My Library" : folder.Title,
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

        public async Task<bool> IsAnythingShared(string userId)
        {
            return (await _sharedContentProvider.IsAnythingShared(userId, typeof(FileDocument).Name)) || await IsAnythingSharedInUserGroups(userId);

            //return (await _folderManager.GetSharedDocuments(userId)).Any();
        }

        public async Task<IEnumerable<IContentType>> GetContent(string id)
        {
            IEnumerable<IContentType> results = new List<FileDocument>();
            IEnumerable<IContentType> resultFolders = new List<Folder>();

            var folder = await _folderManager.GetFolderAsync(id, true);

            //if showing trash items, for deleted folder, we need to get them
            //directly as they are not link as documents are to the trash folder.
            //documents are just added to the folder and can be retrieve as usual. 
            if(folder.IsSystemFolder && folder.Title == "Trash")
            {
                resultFolders = await _folderManager.GetDeletedFolders(folder.OwnerId);
            }else
            {
                resultFolders = ((Folder)folder).ChildFolders.OrderBy(x => x.Title);
            }
            results = (await _folderManager.GetDocumentsAsync(folder)).ToList().OrderBy(x => x.Title);
            

            return resultFolders.Union(results);
        }

        public async Task<IEnumerable<IContentType>> GetSharedContent(string userId)
        {
            // ***** Get FileDocuments
            var sharedDocumentList = new List<IContentType>();
            var sharedItems = (await _sharedContentProvider.GetSharedContent<FileDocument>(userId)).ToList();
            //add group shared ones
            sharedItems.AddRange(await GetSharedClaimsInUserGroups<FileDocument>(userId));

            foreach (var sharedDocumentClaim in sharedItems)
            {
                var sharedDocument = (await _documentService.GetAsync(sharedDocumentClaim.ResourceId));
                if (sharedDocument != null)
                {
                    sharedDocumentList.Add(sharedDocument);
                }
            }

            
            IEnumerable<IContentType> results = sharedDocumentList;

            //****** Get Shared Folders
            var folderList = new List<Folder>();
            var sharedFolderClaims = (await _sharedContentProvider.GetSharedContent<Folder>(userId)).ToList();
            //add in share folder as well
            sharedFolderClaims.AddRange(await GetSharedClaimsInUserGroups<Folder>(userId));

            foreach (var folderClaim in sharedFolderClaims)
            {
                var folder = (await _folderManager.GetFolderAsync(folderClaim.ResourceId));
                if (folder != null)
                {
                    folderList.Add((Folder)folder);
                }
            }
            sharedDocumentList.AddRange(folderList);

            return sharedDocumentList;
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
            return (await GetSharedClaimsInUserGroups<FileDocument>(userId)).Any() || (await GetSharedClaimsInUserGroups<Folder>(userId)).Any();
        }

        private async Task<List<ResourceClaim>> GetSharedClaimsInUserGroups<IContentType>(string userId)
        {
            var sharedGroupClaims = new List<GroupResourceClaim>();
            var sharedDocuments = new List<FileDocument>();
            foreach (var provider in _groupProviders)
            {
                var groupsForUser = await provider.GetGroups();

                foreach (var group in groupsForUser)
                {
                    sharedGroupClaims.AddRange(await _sharedContentProvider.GetGroupSharedContent(group.Id, typeof(IContentType).Name, provider.GetType().Name));
                }
            }

            return sharedGroupClaims.Select(x => new ResourceClaim {
                ClaimType = x.ClaimType,
                ResourceId = x.ResourceId
            }).ToList();
        }

        public async Task<bool> HasAccessToLibrary()
        {
            return await Task.FromResult<bool>(true);
        }

        public string GetLibraryRootName()
        {
            return _rootFolderName;
        }

        public async Task<bool> CanManageLibrary()
        {
            return await Task.FromResult<bool>(true);
        }
    }
}
