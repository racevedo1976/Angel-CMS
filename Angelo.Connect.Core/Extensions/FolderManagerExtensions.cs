using Angelo.Connect.Abstractions;
using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Extensions
{
    public static class FolderManagerExtensions
    {
        public static async Task<IFolder> AddFolderAsync<TDocument>(this IFolderManager<TDocument> manager, string name, string ownerId, string parentId = null)
            where TDocument : class, IDocument
        {
            Ensure.NotNullOrEmpty(name, $"{name} cannot be null or empty.");
            Ensure.NotNullOrEmpty(ownerId, $"{ownerId} cannot be null or empty.");

            var documentLibrary = await manager.GetDocumentLibraryAsync(ownerId);

            Ensure.NotNull(documentLibrary, $"Document Library cannot be null.");

            var result = new Folder()
            {
                DocumentType = typeof(FileDocument).FullName,
                FolderType = typeof(Folder).FullName,
                OwnerLevel = OwnerLevel.User,
                OwnerId = ownerId,
                CreatedBy = ownerId,
                FolderFlags = FolderFlag.Shared,
                Id = KeyGen.NewGuid(),
                //ParentFolder = string.IsNullOrEmpty(parentId) ? null : (await manager.GetFolderAsync(parentId)),
                ParentId = parentId,
                Title = name,
                DocumentLibraryId = documentLibrary.Id
            };

            return await manager.CreateFolderAsync(result);
        }

        public static async Task AddFolderTag(this TagManager manager, IFolder folder, string tagId)
        {
            Ensure.Argument.NotNull(manager, nameof(manager));
            Ensure.Argument.NotNull(folder, nameof(folder));
            Ensure.Argument.NotNullOrEmpty(tagId.ToString(), nameof(tagId));

            var tag = await manager.GetById(tagId);
            await manager.AddTag(folder, tag);
        }

        public static async Task RemoveFolderTag(this TagManager manager, IFolder folder, string tagId)
        {
            Ensure.Argument.NotNull(manager, nameof(manager));
            Ensure.Argument.NotNull(folder, nameof(folder));
            Ensure.Argument.NotNullOrEmpty(tagId.ToString(), nameof(tagId));

            var tag = await manager.GetById(tagId);
            await manager.RemoveTag(folder, tag);
        }

        public static async Task CopyFolderAsync(this IFolderManager<FileDocument> manager,
            IFolder folder,
            IFolder destination,
            string newName)
        {
            newName = (newName ?? string.Empty).Trim();

            folder.Id = KeyGen.NewGuid();
            folder.Title = newName ?? folder.Title;
            folder.ParentId = destination.Id;

            await manager.CreateFolderAsync(folder);

            var contents = await manager.GetFolderAsync(folder.Id);

            // Copy the files in this folder
            foreach (var document in await manager.GetDocumentsAsync(folder))
            {
                await manager.CopyDocumentAsync(document, destination);
            }

            // Copy every folder in sourceFolder to destinationFolder
            foreach (var subFolder in await manager.GetFoldersAsync(folder))
            {
                IFolder destinationSubFolder = new Folder()
                {
                    Id = KeyGen.NewGuid(),
                    ParentId = destination.Id,
                    Title = folder.Title,
                    OwnerId = folder.OwnerId,
                    OwnerLevel = folder.OwnerLevel,
                    DocumentType = folder.DocumentType,
                    FolderFlags = folder.FolderFlags,
                    FolderType = folder.FolderType
                };
                destinationSubFolder = await manager.CreateFolderAsync(destinationSubFolder);
                await manager.CopyFolderAsync(subFolder, destinationSubFolder, null);
            }
        }

        public static async Task CopyDocumentAsync(this IFolderManager<FileDocument> manager,
            FileDocument document, 
            IFolder sourceFolder, 
            IFolder destinationFolder,
            string newName)
       {
            Ensure.Argument.NotNull(destinationFolder);
            Ensure.Argument.NotNull(document);

            var originalId = document.DocumentId;
            var originalOwnerId = sourceFolder.OwnerId;

            newName = newName?.Trim();
            if (!string.IsNullOrEmpty(newName) && document.FileName != newName)
            {
                // Rename
                document.FileName = newName;
            }

            // Copy the document object
            await manager.CopyDocumentAsync(document, destinationFolder);
        }

        public static async Task<IEnumerable<TDocument>> GetDocumentsInFolderAsync<TDocument>(this IFolderManager<TDocument> manager, string folderId)
            where TDocument : class, IDocument
        {
            if (String.IsNullOrEmpty(folderId))
                throw new ArgumentNullException("ownerId folderId be null.");

            var folder = await manager.GetFolderAsync(folderId);
            return await manager.GetDocumentsAsync(folder);
        }

        public static async Task CopyFolderAsync(this IFolderManager<FileDocument> manager, string ownerId, string folderId, string destinationId, string newName)
        {
            // NOT WORKING YET (Requires Federation to work) Ensure.NotNullOrEmpty(ownerId, $"{ownerId} cannot be null or empty.");
            Ensure.NotNullOrEmpty(folderId, $"{folderId} cannot be null or empty.");
            Ensure.NotNullOrEmpty(destinationId, $"{destinationId} cannot be null or empty.");

            var folder = await manager.GetFolderAsync(folderId);
            var destination = await manager.GetFolderAsync(destinationId);

            await manager.CopyFolderAsync(folder, destination, newName);
        }

        public static async Task<IFolder> GetTrashFolderAsync<TDocument>(this IFolderManager<TDocument> manager, string ownerId)
            where TDocument : class, IDocument
        {
            var root = await manager.GetRootFolderAsync(ownerId);
            var children = await manager.GetFoldersAsync(root);
            var result = children.SingleOrDefault(x => manager.IsTrashFolderAsync(x).Result);

            return result;
        }

        public static async Task<IEnumerable<IFolder>> GetFoldersPathAsync<TDocument>(this IFolderManager<TDocument> manager, IFolder folder)
            where TDocument : class, IDocument
        {
            var parents = folder.ParentId == null
                ? Enumerable.Empty<IFolder>()
                : await manager.GetFoldersPathAsync(await manager.GetFolderAsync(folder.ParentId));
            return parents.Concat(new[] { folder });
        }

        public static async Task<List<Folder>> GetFoldersRecursivelyAsync<TDocument>(this IFolderManager<TDocument> manager, 
            IFolder parentFolder,
            bool ignoreTrash = true)
            where TDocument : class, IDocument
        {
            var folders = (await manager.GetFoldersAsync(parentFolder))
                .Select(x => x.ToFolder());


            foreach (var folder in folders)
            {
                foreach (var subFolder in folder.ChildFolders)
                {
                    subFolder.ChildFolders = await manager.GetFoldersRecursivelyAsync<TDocument>(subFolder, ignoreTrash);
                }
            }

            if (ignoreTrash)
            {
                folders = folders.Where(x => !manager.IsTrashFolderAsync(x).Result);
            }

            return folders.ToList();
        }

        private static async Task<bool> IsTrashFolderAsync<TDocument>(this IFolderManager<TDocument> manager, IFolder folder)
            where TDocument : class, IDocument
        {
            var root = await manager.GetRootFolderAsync(folder.OwnerId);

            return folder != null && folder.ParentId == root.Id && string.Equals("Trash", folder.Title, StringComparison.CurrentCultureIgnoreCase);
        }

        public static async Task<string> GetPathAsync(this IFolderManager<FileDocument> manager, IFolder folder)
        {
            var result = default(string);

            if (folder != null)
            {
                result = $"/{folder.Title}";
                if (!string.IsNullOrEmpty(folder.ParentId))
                {
                    var parent = await manager.GetFolderAsync(folder.ParentId);
                    result = (await manager.GetPathAsync(parent)) + result;
                }
            }

            return result;
        }
    }
}
