using Angelo.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using Angelo.Drive.Services;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;

namespace Angelo.Drive.Jobs
{
    public class JobZip
    {
        private IDocumentService<FileDocument> _docService; // For lookups
        private IDocumentUploadService<FileDocument> _ulService;
        private LibraryZipService _zipService;
        private LibraryManager _manager;
        private IFolderManager<FileDocument> _folderManager;

        public JobZip(LibraryManager manager,
            IDocumentService<FileDocument> docService,
            IDocumentUploadService<FileDocument> ulService,
            LibraryZipService zipService,
            IFolderManager<FileDocument> folderManager)
        {
            this._manager = manager;
            this._docService = docService;
            this._ulService = ulService;
            this._zipService = zipService;
            this._folderManager = folderManager;
        }

        //public Guid ID { get; set; }
        //public Folder[] Folders { get; set; }
        //public Document[] Documents { get; set; }

        // Required: because EnqueueAsync takes an Expression, and Expression has no async member, there has to be a synchronous version
        public void Execute(FileDocument zipDocument, string ownerId, string targetFolderId, KeyValuePair<string, string>[] documents)
        {
            // Quick n dirty
            ExecuteAsync(zipDocument, ownerId, targetFolderId, documents).GetAwaiter().GetResult();
        }

        // Trying to pass a doc path (including filename) and its ID. that gives me folder information, file name, and a key to grab the stream in a 
        // nice, JSON-friendly format. that is relatively minimalist in terms of data xferred/persisted.
        public async Task ExecuteAsync(FileDocument zipDocument, string ownerId, string targetFolderId, KeyValuePair<string, string>[] documents)
        {
            await ZipDocumentsAsync(zipDocument, ownerId, targetFolderId, documents);
        }

        private Stream GetFileStream(string documentId, string ownerId)
        {
            var libraryLocation = _folderManager.GetDocumentLibraryAsync(ownerId).Result.Location;
            var document = _docService.GetAsync(documentId).Result;
            var result = _ulService.GetFileStreamAsync(document, libraryLocation).Result;

            return result;
        }

        private async Task ZipDocumentsAsync(FileDocument zipDocument, string ownerId, string targetFolderId, KeyValuePair<string, string>[] documents)
        {
            
            var kvs = documents
                .ToDictionary(document => document.Key,
                              document => GetFileStream(document.Value, ownerId));
            try
            {
                using (var result = await _zipService.ZipAsync(kvs))
                {
                    result.Position = 0;
                    await WriteToFolder(zipDocument, result, ownerId, targetFolderId);
                    await UpdateContentLengthAsync(zipDocument, result.Length, ownerId);
                }
            }
            finally
            {
                foreach (var kv in kvs)
                {
                    var stream = kv.Value;
                    if (stream == null) continue;

                    stream.Dispose();
                }
            }
        }

        private async Task WriteToFolder(FileDocument document, Stream documentStream, string ownerId, string folderId)
        {
            var libraryLocation = (await _folderManager.GetDocumentLibraryAsync(ownerId)).Location;
            await _ulService.SetFileStreamAsync(
                document,
                documentStream,
                libraryLocation);
        }

        private async Task UpdateContentLengthAsync(FileDocument document, long length, string ownerId)
        {
            Ensure.NotNull(document, nameof(document));

            document.ContentLength = length;

            await _docService.UpdateAsync(document);
        }
    }
}
