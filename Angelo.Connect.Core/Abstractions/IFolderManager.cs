using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IFolderManager
    {
        //Document Library
        Task<DocumentLibrary> CreateDocumentLibrary(string ownerId, string libraryType, string location);
        Task<DocumentLibrary> GetDocumentLibraryAsync(string ownerId);

        // Folder Operations
        Task<IFolder> GetRootFolderAsync(string ownerId, bool ignoreTrash = false);
        Task<IFolder> GetFolderAsync(string folderId, bool ignoreTrash = false);
        Task<IEnumerable<IFolder>> GetFoldersAsync(string ownerId);
        Task<IEnumerable<IFolder>> GetFoldersAsync(IFolder folder, bool ignoreTrash = false);
        Task<IEnumerable<Folder>> GetDeletedFolders(string ownerId);
        Task<IFolder> CreateFolderAsync(IFolder folder);
        Task<IFolder> UpdateFolderAsync(IFolder folder);
        Task RemoveFolderAsync(IFolder folder);
        Task DeleteFolderAsync(IFolder folder);
        Task RestoreFolder(string id);
        Task MoveFolderAsync(IFolder folder, IFolder destination);
        //Task<IFolder> GetFolderContainItemAsync(string itemId);

        // Document Service Interop for Retrieving Documents (generic definition)
        Task<IEnumerable<IDocument>> GetDocumentsAsync(IFolder folder);
       
    }

}
