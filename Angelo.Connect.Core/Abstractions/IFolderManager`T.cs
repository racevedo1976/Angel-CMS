using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IFolderManager<TDocument> : IFolderManager where TDocument : class, IDocument
    {
        // Document Service Interop for Retrieving Documents (type override)
        new Task<IEnumerable<TDocument>> GetDocumentsAsync(IFolder folder);

        // Document Service Interop for Manipulating Documents (type specific)
        Task<TDocument> CopyDocumentAsync(TDocument document, IFolder destination);
        Task RemoveDocumentAsync(TDocument document, IFolder folder);
        Task MoveDocumentAsync(TDocument document, IFolder source, IFolder destination);
        Task AddDocumentAsync(TDocument document, IFolder folder);

        Task<IFolder> GetFolderAsync(TDocument document);
        Task<IEnumerable<ResourceClaim>> SharedDocument(string id, string type, string[] users);

        Task<IEnumerable<TDocument>> GetSharedDocuments(string userId);
        Task<IEnumerable<TDocument>> GetDeletedDocuments(string ownerId);
        Task DeleteDocumentAsync(string id);
        void RestoreDocument(string id);
    }

}
