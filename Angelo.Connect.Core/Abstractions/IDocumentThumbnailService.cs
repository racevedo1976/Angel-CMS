using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IDocumentThumbnailService<TDocument> where TDocument : class, IDocument
    {
        Task<Stream> GetThumbnailAsync(TDocument document, int width, int height, string libraryLocation);
    }
}
