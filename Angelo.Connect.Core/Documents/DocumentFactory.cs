using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public class DocumentFactory
    {
        // To be used by the Startup/extension method

        public void SetDocumentService<TDocument>(IDocumentService<TDocument> documentService) where TDocument : class, IDocument
        {
            throw new NotImplementedException();
        }

        public IDocumentService<TDocument> GetDocumentService<TDocument>() where TDocument : class, IDocument
        {
            throw new NotImplementedException();
        }
    }
}
