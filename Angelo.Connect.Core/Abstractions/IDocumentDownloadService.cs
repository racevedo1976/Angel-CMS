using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IDocumentDownloadService<TDocument> where TDocument : class, IDocument
    {
        Task<Stream> GetFileStreamAsync(TDocument document, string libraryLocation);
    }
}
