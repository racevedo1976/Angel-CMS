using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Connect.Abstractions;
using Angelo.Common.Extensions;
using Angelo.Common.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Extensions;
using Angelo.Connect.Security;

namespace Angelo.Connect.Services
{
    public class FolderManager<TDocument> : IFolderManager<TDocument> where TDocument : class, IDocument 
    {
        private const int _defaultFolderQuota = 10000;  
        private const string _trashFolderName = "Trash";
        
        private DbContextOptions<ConnectDbContext> _db; 
        private IDocumentService<TDocument> _documentService;
        private string _documentType;
        private ResourceManager _resourceManager;

        public int FolderQuota { get; set; } = _defaultFolderQuota;

        public FolderManager(DbContextOptions<ConnectDbContext> db, 
            IDocumentService<TDocument> documentService, 
            DbContextOptions<ConnectDbContext> options,
            ResourceManager resourceManager)
        {
            Ensure.NotNull(db, $"{nameof(db)} cannot be null.");
            Ensure.NotNull(documentService, $"{nameof(documentService)} cannot be null.");

            _db = db;    // Keith: This is a hack to prevent threading errors, since I can only ensure locking here, but other _db writes are periodically breaking
                                                    // TODO: Maybe replace with a DbContextProvider?
            _documentService = documentService;
            _documentType = typeof(TDocument).FullName;
            _resourceManager = resourceManager;
            var x = new List<string>();
        }

        #region Public API
        public async Task<IFolder> GetRootFolderAsync(string ownerId, bool ignoreTrash = false)
        {
            Ensure.NotNullOrEmpty(ownerId);

            //await EnsureLibraryCreated(ownerId);

            await EnsureRootFolderAsync(ownerId);
            using (var db = new ConnectDbContext(_db))
            {
                var result = db.Folders
                        .Include(x => x.ChildFolders)
                        .FirstOrDefault(x =>               // Prevents seeding errors from crashing startup
                            x.DocumentType == _documentType
                            && x.OwnerId == ownerId
                            && x.ParentFolder == null
                            && !x.IsDeleted
                           
                        );

                result.ChildFolders = result.ChildFolders.Where(x => !x.IsDeleted).ToList();
                if (ignoreTrash)
                    result.ChildFolders = result.ChildFolders.Where(x => (!x.IsSystemFolder && x.Title != _trashFolderName)).ToList();

                return result;
            }
        }

        //public async Task EnsureLibraryCreated(string ownerId)
        //{
        //    using (var db = new ConnectDbContext(_db))
        //    {
        //        if(!db.DocumentLibraries.Any(x => x.OwnerId == ownerId))
        //        {
        //            await CreateDocumentLibrary(ownerId, "User", "userfolders");
        //        }
        //    }
        //}
        public async Task<IFolder> GetFolderAsync(string folderId, bool ignoreTrash = false)
        {
            Ensure.NotNullOrEmpty(folderId);

            using (var db = new ConnectDbContext(_db))
            {
                var folder = await db.Folders
                        .Include(x => x.ChildFolders)
                        .SingleOrDefaultAsync(x => x.Id == folderId);

                folder.ChildFolders = folder.ChildFolders.Where(x => !x.IsDeleted).ToList();
                
                if(ignoreTrash)
                    folder.ChildFolders = folder.ChildFolders.Where(x => (!x.IsSystemFolder && x.Title != _trashFolderName)).ToList();

                return folder;
            }
        }

        public async Task<IFolder> GetFolderAsync(TDocument document)
        {
            Ensure.NotNull(document, "Document cannot be null.");

            using (var db = new ConnectDbContext(_db))
            {
                return db.Folders
                        .Include(x => x.ChildFolders)
                        .FirstOrDefault(f => f.Items.Any(i => i.DocumentId == document.DocumentId));


            }
        }

        public async Task<IEnumerable<IFolder>> GetFoldersAsync(IFolder folder, bool ignoreTrash = false)
        {
            Ensure.NotNull(folder, $"{nameof(folder)} cannot be null.");

            using (var db = new ConnectDbContext(_db))
            {
                var folders = await db.Folders
                    .Include(x => x.ChildFolders)
                    .Where(x => x.ParentId == folder.Id && !x.IsDeleted)
                    .OrderBy(x => x.Title)
                    .ToListAsync();

                if (ignoreTrash)
                    folders = folders.Where(x => !x.IsSystemFolder && x.Title != _trashFolderName).ToList();

                foreach (var fold in folders)
                {
                    fold.ChildFolders = fold.ChildFolders.Where(x => !x.IsDeleted).ToList();

                    if (ignoreTrash)
                        fold.ChildFolders = fold.ChildFolders.Where(x => (!x.IsSystemFolder && x.Title != _trashFolderName)).ToList();
                }

                return folders;
            }

            
        }

        public async Task<IEnumerable<IFolder>> GetFoldersAsync(string ownerId)
        {
            Ensure.NotNullOrEmpty(ownerId, $"{nameof(ownerId)} cannot be null or empty.");

            using (var db = new ConnectDbContext(_db))
            {
                return await db.Folders
                    .Include(x => x.ChildFolders)
                    .Where(x => x.OwnerId == ownerId && !x.IsDeleted)
                    .ToListAsync();
            }
        }
           
        public async Task<IFolder> CreateFolderAsync(IFolder folder)
        {
            Ensure.NotNull(folder, $"{nameof(folder)} cannot be null.");

            return await CreateFolderInternalAsync(folder, true);
        }

        private async Task<IFolder> CreateFolderInternalAsync(IFolder folder, bool ensureRoot)
        {
            var ownerId = folder.OwnerId;
            await EnsureFolderQuotaAsync(ownerId);

            Folder result;

            var parentId = folder.ParentId ?? string.Empty;

            if (ensureRoot)
            {
                await EnsureRootFolderAsync(ownerId);
            }

            bool exists;
            Folder parentFolder;
            using (var db = new ConnectDbContext(_db))
            {
                // NOTE: Doing context calls rather than method calls to avoid including ChildFolders
                parentFolder = string.IsNullOrEmpty(parentId)
                    ? await db.Folders.SingleOrDefaultAsync(x => x.DocumentType == typeof(TDocument).FullName && x.OwnerId == ownerId && string.IsNullOrEmpty(x.ParentId))  // NOTE: If the new folder's parent is null, it MUST go in that folder type's root folder. If it was null, then it would be a root, but there can only be 1 root per type 
                    : await db.Folders.SingleOrDefaultAsync(x => x.Id == parentId);

                exists = db.Folders.Any(x =>
                    x.DocumentType == _documentType
                    && x.OwnerId == ownerId
                    && (x.ParentId ?? string.Empty) == parentId
                    && string.Equals(x.Title, folder.Title, StringComparison.CurrentCultureIgnoreCase)
                );

                if (exists) // TODO: Fix potential race condition
                {
                    throw new InvalidOperationException(parentId == null
                            ? $"A folder named '{folder.Title}' in root already exists."
                            : $"A folder named '{folder.Title}' in parent folder {parentId} already exists.");
                }
                else
                {
                    result = folder.ToFolder();
                    result.Id = KeyGen.NewGuid();
                    result.ParentFolder = parentFolder;
                    result.ParentId = parentFolder?.Id;
                    result.DocumentType = _documentType;

                    db.Folders.Add(result);
                    db.SaveChanges();
                }
            }

            return result;
        }
        public async Task<IFolder> UpdateFolderAsync(IFolder folder)
        {
            Ensure.NotNull(folder, $"{nameof(folder)} cannot be null.");

            using (var db = new ConnectDbContext(_db))
            {
                db.Folders.Update(folder.ToFolder());
                db.SaveChanges();
                return await db.Folders.SingleOrDefaultAsync(x => x.Id == folder.Id) ?? folder;
            }
        }

        public async Task RemoveFolderAsync(IFolder folder)
        {
            Ensure.NotNull(folder, $"{folder} cannot be null.");

            if (folder.IsSystemFolder)
            {
                throw new InvalidOperationException("Unable to delete system folders.");
            }

            using (var db = new ConnectDbContext(_db))
            {
                var dbFolder = db.Folders.Single(x => x.Id == folder.Id);
                db.Folders.Remove(dbFolder);
                db.SaveChanges();
            }
        }

        public async Task DeleteFolderAsync(IFolder folder)
        {
            Ensure.NotNull(folder, $"{folder} cannot be null.");

            if (folder.IsSystemFolder)
            {
                throw new InvalidOperationException("Unable to delete system folders.");
            }

            using (var db = new ConnectDbContext(_db))
            {
                var dbFolder = db.Folders.Single(x => x.Id == folder.Id);
                dbFolder.IsDeleted = true;
                db.Folders.Update(dbFolder);
                db.SaveChanges();
            }
        }


        public async Task MoveFolderAsync(IFolder folder, IFolder destination)
        {
            // NOT WORKING YET (Requires Federation to work) Ensure.NotNullOrEmpty(ownerId, $"{ownerId} cannot be null or empty.");
            Ensure.NotNull(folder, $"{folder} cannot be null.");
            Ensure.NotNull(destination, $"{destination} cannot be null.");

            if (await IsSystemFolderAsync(folder))
            {
                throw new InvalidOperationException("Unable to move system folders.");
            }
            else
            {
                EnsureNotAncestor(folder, destination);
                
                using (var db = new ConnectDbContext(_db))
                {
                    var folderEntity = db.Folders.FirstOrDefault(x => x.Id == folder.Id);

                    folderEntity.ParentId = destination.Id;
                    db.Folders.Update(folderEntity);
                    db.SaveChanges();
                }
            }
        }

        async Task<IEnumerable<IDocument>> IFolderManager.GetDocumentsAsync(IFolder folder)
        {
            return await GetDocumentsAsync(folder);
        }

        public async Task AddDocumentAsync(TDocument document, IFolder folder)
        {
            Ensure.NotNull(document, $"{nameof(document)} cannot be null.");
            Ensure.NotNull(folder, $"{nameof(folder)} cannot be null.");

            var item = await GetItemAsync(document.DocumentId, folder.Id);
            if (item == null)
            {
                using (var db = new ConnectDbContext(_db))
                {
                    item = new FolderItem()
                    {
                        AllowComments = true,
                        DocumentId = document.DocumentId,
                        Folder = folder as Folder,
                        FolderId = folder.Id,
                        Id = KeyGen.NewGuid(),
                        InheritSecurity = false,
                        InheritSharing = false,
                        InheritTags = false,
                        ItemStatus = ModerationStatus.Approved
                    };

                    // No async, since we are in a lock
                    db.Entry(item).State = EntityState.Added;

                    await db.FolderItems.AddAsync(item);
                    db.SaveChanges();
                }
            }
        }

        public async Task RemoveDocumentAsync(TDocument document, IFolder folder)
        {
            Ensure.NotNull(document, $"{nameof(document)} cannot be null.");
            Ensure.NotNull(folder, $"{nameof(folder)} cannot be null.");

            var item = await GetItemAsync(document.DocumentId, folder.Id);
            if (item == null)
            {        // TODO Log information?
                return;   // Document is already there (potential race condition, so no error)
            }

            using (var db = new ConnectDbContext(_db))
            {
                db.FolderItems.Remove(item);
                db.SaveChanges();
            }
        }


        public async Task DeleteDocumentAsync(string documentId)
        {
            Ensure.NotNull(documentId, $"{nameof(documentId)} cannot be null.");

            using (var db = new ConnectDbContext(_db))
            {
                //Get document objects
                var documentItem = db.FolderItems.Include(x => x.Folder).FirstOrDefault(x => x.DocumentId == documentId);
                var document = await _documentService.GetAsync(documentId);

                var ownerId = documentItem.Folder.OwnerId;

                if (documentItem == null)
                {        // TODO Log information?
                    return;   // Document is already there (potential race condition, so no error)
                }

                //make a copy in trash for displaying purposes
                var trashFolder = GetTrashFolder(ownerId);
                var clone = new FolderItem
                {
                    Id = Guid.NewGuid().ToString("N"),
                    DocumentId = documentItem.DocumentId,
                    FolderId = trashFolder.Id,
                    CreatedBy = documentItem.CreatedBy,
                    InheritSecurity = documentItem.InheritSecurity,
                    InheritSharing = documentItem.InheritSharing,
                    InheritTags = documentItem.InheritTags,
                    ItemStatus = documentItem.ItemStatus,
                    AllowComments = documentItem.AllowComments,
                    IsDeleted = false
                };
                db.FolderItems.Add(clone);
                db.SaveChanges();


                //set original document to deleted (soft delete)
                documentItem.IsDeleted = true;

                db.FolderItems.Update(documentItem);
                db.SaveChanges();
            }
        }

        private Folder GetTrashFolder(string ownerId)
        {
            using (var db = new ConnectDbContext(_db))
            {
                var folder = db.Folders.FirstOrDefault(x => x.OwnerId == ownerId && x.Title == _trashFolderName && x.IsSystemFolder);

                if (folder == null)
                    throw new NullReferenceException("Missing Trash Folder.");

                return folder;
            }
        }

        public async Task<TDocument> CopyDocumentAsync(TDocument document, IFolder destination)
        {
            // Copy the document
            document = await _documentService.CloneAsync(document);

            // Create the folder entry
            await AddDocumentAsync(document, destination);

            return document;
        }

        public async Task MoveDocumentAsync(TDocument document, IFolder source, IFolder destination)
        {
            await AddDocumentAsync(document, destination);
            await RemoveDocumentAsync(document, source);
        }

        public async Task<IEnumerable<TDocument>> GetDocumentsAsync(IFolder folder)
        {
            Ensure.NotNull(folder);

            var entries = await GetItemsAsync(folder.Id);

            //HACK: Linq was puking when trying to do a correlated expression between 2 different dbContexts.
            //      Fixing by materializing ID list to primitive array, then peferming query
            var entryIds = entries.Select(x => x.DocumentId);

            return _documentService.QueryByIds(entryIds);
        }


        public async Task<IEnumerable<TDocument>> GetSharedDocuments(string userId)
        {
            var sharedDocs = new List<TDocument>();
            var sharedIds = await _resourceManager.GetResourceClaimsAsync<FileDocument>(userId);

            var entryIds = sharedIds.Select(x => x.ResourceId);

            return _documentService.QueryByIds(entryIds);

           // return sharedDocs;
        }
        //Task<IEnumerable<IDocument>> IFolderManager.GetSharedDocument(string userId)
        //{
        //    throw new NotImplementedException();
        //}

        public async Task<IEnumerable<ResourceClaim>> SharedDocument(string id, string type, string[] users)
        {
            var resourceList = new List<ResourceClaim>();
            foreach (var user in users)
            {
                var addedResource = await _resourceManager.AddResourceClaimAsync(id, type, user, "View");
                if (addedResource != null)
                    resourceList.Add(addedResource);
            }

            return resourceList;

        }
        #endregion // Public API


        #region Private methods
        // Because the context is now short-lived (for concurrency protection), making this now a LINQ-finalized enumerable instead of queryable
        private async Task<IEnumerable<FolderItem>> GetItemsAsync(string folderId)
        {
            using (var db = new ConnectDbContext(_db))
            {
                return await db.FolderItems.Where(x => x.FolderId == folderId && !x.IsDeleted).ToArrayAsync();
            }
        }

        private async Task<FolderItem> GetItemAsync(string documentId, string folderId)
        {
            using (var db = new ConnectDbContext(_db))
            {
                return await db.FolderItems.SingleOrDefaultAsync(x => x.FolderId == folderId && x.DocumentId == documentId);
            }
        }
        
        private async Task<bool> IsSystemFolderAsync(IFolder folder)
        {
            Ensure.NotNull(folder, $"{folder} cannot be null.");

            return await IsTrashFolderAsync(folder);
        }

        private async Task<bool> IsTrashFolderAsync(IFolder folder)
        {
            Ensure.NotNull(folder, $"{folder} cannot be null.");

            var parentFolder = await GetFolderAsync(folder.ParentId);

            return string.IsNullOrEmpty(parentFolder?.ParentId) // TODO folder.IsRoot()
                && string.Equals(_trashFolderName, folder.Title, StringComparison.CurrentCultureIgnoreCase)
                && folder.IsSystemFolder;
        }

        private async Task<Folder> GetFullFolderAsync(string id)
        {
            using (var db = new ConnectDbContext(_db))
            {
                return await db.Folders.SingleOrDefaultAsync(x => x.Id == id);
            }
        }

        private async Task EnsureRootFolderAsync(string ownerId) 
        {
            Folder root;

            //get library for owner
            DocumentLibrary library;

            using (var db = new ConnectDbContext(_db))
            {
                library = db.DocumentLibraries.FirstOrDefault(x => x.OwnerId == ownerId);

                if (library == null)
                    throw new Exception("Cannot create root folder. Library missing for owner.");

                root = await db.Folders.SingleOrDefaultAsync(x =>
                                       x.DocumentType == _documentType
                                       && x.OwnerId == ownerId
                                       && x.DocumentLibraryId == library.Id
                                       && string.IsNullOrEmpty(x.ParentId)
                                    );
            }

            if (root == null)
            {
                root = new Folder()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Title = string.Empty,
                    OwnerLevel = Security.OwnerLevel.User,
                    DocumentType = _documentType,
                    FolderType = typeof(Folder).Name,
                    OwnerId = ownerId,
                    DocumentLibraryId = library.Id,
                    CreatedBy = ownerId
                };

                await CreateFolderInternalAsync(root, false);
            }
        }

        #region Ensures
        private void EnsureNotAncestor(IFolder folder, IFolder destinationFolder)
        {
            if (IsAncestor(folder, destinationFolder)) throw new InvalidOperationException("Unable to place folder into itself or a descendent folder.");
        }

        // NOTE: Using Folder instead of ID even though they don't populate Folders because I also have Owner stored here
        private bool IsAncestor(IFolder folder, IFolder destinationFolder)
        {
            var result = folder.Id == destinationFolder.Id && folder.OwnerId == destinationFolder.OwnerId;  // Can't copy folder to self
            if (!result)
            {
                using (var db = new ConnectDbContext(_db))
                {
                    // Can't copy folder if destination is one of its children (or grandchildren)
                    var children = db.Folders.Where(x => x.ParentId == folder.Id && x.OwnerId == folder.OwnerId).ToArray();  // ToArray finalizes the LINQ so I can recursively call into the DB
                    foreach (var child in children)
                    {
                        if (IsAncestor(child, destinationFolder))
                        {
                            result = true;
                            break;
                        }
                    }
                }
            }

            return result;
        }

       
        #endregion // Ensures

        #region Quota workers
        private async Task EnsureFolderQuotaAsync(string ownerId)
        {
            var quota = this.FolderQuota;
            var count = await GetTotalFolderCountAsync(ownerId);

            if (count >= quota)
            {
                throw new InvalidOperationException($"User is limited to {quota} folders.");
            }
        }

        private async Task<int> GetTotalFolderCountAsync(string ownerId)
        {
            using (var db = new ConnectDbContext(_db))
            {
                return await db.Folders
                    .Where(x => x.OwnerId == ownerId)
                    .CountAsync();
            }
        }

        /// <summary>
        ///  Ensure a document library exist or it will  create a new one based on type and owner
        /// </summary>
        /// <param name="ownerId"></param>
        /// <param name="libraryType"></param>
        /// <param name="location"></param>
        /// <returns>DocumentLibrary</returns>
        public async Task<DocumentLibrary> CreateDocumentLibrary(string ownerId, string libraryType, string location)
        {
            Ensure.NotNullOrEmpty(ownerId);
            Ensure.NotNullOrEmpty(libraryType);
            Ensure.NotNullOrEmpty(location);

            DocumentLibrary documentLibrary = null;

            using (var db = new ConnectDbContext(_db))
            {
                documentLibrary = db.DocumentLibraries.FirstOrDefault(x => x.OwnerId == ownerId && x.LibraryType == libraryType);
                if (documentLibrary == null)
                {

                    documentLibrary = new DocumentLibrary
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        OwnerId = ownerId,
                        LibraryType = libraryType,
                        Location = location
                    };

                    db.DocumentLibraries.Add(documentLibrary);
                    db.SaveChanges();
                }
            }

            //Ensure there is a Trash folder and Root folder
            await EnsureTrashFolderAsync(ownerId);

            return documentLibrary;
        }

       
        public async Task<DocumentLibrary> GetDocumentLibraryAsync(string ownerId)
        {
            Ensure.NotNullOrEmpty(ownerId);

            var documentLibrary = new DocumentLibrary();

            using (var db = new ConnectDbContext(_db))
            {
                documentLibrary = db.DocumentLibraries.FirstOrDefault(x => x.OwnerId == ownerId);
            }

            return documentLibrary;
        }

        public async Task<IEnumerable<TDocument>> GetDeletedDocuments(string ownerId)
        {
            IEnumerable<TDocument> documents = new List<TDocument>();
            var library = await GetDocumentLibraryAsync(ownerId);

            using (var db = new ConnectDbContext(_db))
            {
                var deletedDocumentIds = db.FolderItems
                                            .Include(x => x.Folder)
                                            .Where(x => x.Folder.DocumentLibraryId == library.Id && x.IsDeleted)
                                            .Select(x => x.DocumentId);
                if (deletedDocumentIds != null)
                {
                    if (deletedDocumentIds.Any())
                    {
                        //HACK: Linq was puking when trying to do a correlated expression between 2 different dbContexts.
                        //      Fixing by materializing ID list to primitive array, then peferming query

                        documents = _documentService.QueryByIds(deletedDocumentIds).ToList();
                    }
                }

                return documents;
            }
        }

        public async Task<IEnumerable<Folder>> GetDeletedFolders(string ownerId)
        {
            IEnumerable<IFolder> documents = new List<IFolder>();
            
            using (var db = new ConnectDbContext(_db))
            {
                var deletedFolders = await db.Folders
                                            .Where(x => x.IsDeleted && x.OwnerId == ownerId)
                                            .ToListAsync();
                
                return deletedFolders;
            }
        }


        #endregion // Quota workers

        private async Task EnsureTrashFolderAsync(string ownerId)
        {
            var root = (Folder) await GetRootFolderAsync(ownerId);
            
            using (var db = new ConnectDbContext(_db))
            {
                var trashFolder = db.Folders.FirstOrDefault(x => x.ParentId == root.Id && x.Title == _trashFolderName && x.IsSystemFolder);

                if (trashFolder == null)
                {
                    var newTrashFolder = new Folder()
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        Title = _trashFolderName,
                        OwnerLevel = OwnerLevel.User,
                        DocumentType = _documentType,
                        FolderType = typeof(Folder).Name,
                        OwnerId = ownerId,
                        IsSystemFolder = true,
                        DocumentLibraryId = root.DocumentLibraryId,
                        CreatedBy = ownerId
                    };

                    await CreateFolderInternalAsync(newTrashFolder, true);
                }
            }
        }

        /// <summary>
        ///  Restore a document from the trash folder to its original location
        /// </summary>
        /// <param name="id">Document Id</param>
        public void RestoreDocument(string id)
        {
            
            using (var db = new ConnectDbContext(_db))
            {
                //get all folderitems for document. should be 2 items: one doc that is deleted (IsDeleted == true) and 
                //another document that is in the trash folder. 
                var documentItems = db.FolderItems.Include(x => x.Folder).Where(x => x.DocumentId == id).ToList();

                var ownerId = documentItems.First().Folder.OwnerId;

                //Get trash folder for library
                var trashFolder = GetTrashFolder(ownerId);

                foreach (var docItem in documentItems)
                {
                    //if this is the doc in trash folder, then remove it
                    if (docItem.Folder.Id == trashFolder.Id)
                    {
                        db.FolderItems.Remove(docItem);
                        db.SaveChanges();
                    }else
                    {
                        //if this the document Item not in the trash folder and the IsDeleted is true, then just update the flag
                        if (docItem.IsDeleted && docItem.Folder.Id != trashFolder.Id)
                        {
                            docItem.IsDeleted = false;
                            db.FolderItems.Update(docItem);
                            db.SaveChanges();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Restore a deleted Folder
        /// </summary>
        /// <param name="id"></param>
        public async Task RestoreFolder(string id)
        {
            using (var db = new ConnectDbContext(_db))
            {
                var deletedFolder = db.Folders.FirstOrDefault(x => x.Id == id);

                if (deletedFolder != null)
                {
                    deletedFolder.IsDeleted = false;
                    db.Folders.Update(deletedFolder);
                    db.SaveChanges();
                }
            }
        }
        #endregion // Private methods
    }

 
}
