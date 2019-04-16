using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IDocumentUploadService<TDocument> : IDocumentDownloadService<TDocument> where TDocument : class, IDocument
    {
        Task SetFileStreamAsync(TDocument document, Stream fileStream, string libraryLocation);
    }
}
