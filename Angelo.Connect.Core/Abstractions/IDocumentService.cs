using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IDocumentService<TDocument> where TDocument: class, IDocument
    {
        Task<TDocument> CreateAsync(TDocument document);
        Task<TDocument> UpdateAsync(TDocument document);
       
        Task<TDocument> GetAsync(string documentId);
        Task<TDocument> GetAsync(TDocument document);
        //Task<IEnumerable<TDocument>> ListAsync(string documentId);
        Task<TDocument> CloneAsync(TDocument document);
        Task<TDocument> RenameAsync(TDocument document);

        Task DeleteAsync(string documentId);
        Task DeleteAsync(TDocument document);
        Task DeleteByIdsAsync(IEnumerable<string> documentIds);

        IQueryable<TDocument> Query();
        IQueryable<TDocument> QueryByIds(IEnumerable<string> documentIds);

        Task<string> GetDocumentLibraryLocation(string documentId);
        Task<DocumentLibrary> GetDocumentLibraryAsync(string id);
    }
}
