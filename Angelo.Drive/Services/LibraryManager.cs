using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Angelo.Connect.Services;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using Angelo.Drive.Jobs;
using Angelo.Jobs;
using Angelo.Connect.Logging;
using Angelo.Connect.Extensions;
using Angelo.Drive.Abstraction;
using Microsoft.AspNetCore.Hosting;

namespace Angelo.Drive.Services
{
    public class LibraryManager
    {
        internal const int DefaultThumbSize = 600;
        internal const int DefaultDocumentQuota = 1000;  // NOTE: This is a user's total, not per-folder!
        #region Nested classes
        private class DocumentFolderEntry
        {
            public FileDocument Document { get; }
            public Folder Folder { get; }
            public DocumentFolderEntry(Folder folder, FileDocument document)
            {
                this.Document = document;
                this.Folder = folder;
            }
        }

      
        #endregion // Nested classes
        #region Dependencies
        private IFolderManager<FileDocument> _folderManager;
        private IDocumentService<FileDocument> _documentService;
        private IDocumentUploadService<FileDocument> _uploadService;
        private IDocumentThumbnailService<FileDocument> _thumbnailService;
        private LibraryZipService _libraryZipService;
        private IJobsManager _jobs;
        private DbLoggerProvider _log;
        private DbLogService _logFetcher;
        private TagManager _tagManager;
        private LibraryIOService _libraryIOService;
        private IEnumerable<IImagePostProcessor> _imageProcessors;
        private IHostingEnvironment _env;
        #endregion // Dependencies
        #region Constructors
        public LibraryManager(
            IFolderManager<FileDocument> folderManager,
            IDocumentService<FileDocument> documentService,
            IDocumentUploadService<FileDocument> uploadService,
            IDocumentThumbnailService<FileDocument> thumbnailService, 
            LibraryZipService libraryZipService,
            DbLoggerProvider log,
            DbLogService logFetcher,
            IJobsManager jobsManager, 
            TagManager tagManager,
            LibraryIOService libraryIOService,
            IEnumerable<IImagePostProcessor> imageProcessors,
            IHostingEnvironment env)
        {
            Ensure.NotNull(folderManager, $"{nameof(folderManager)} cannot be null.");
            Ensure.NotNull(documentService, $"{nameof(documentService)} cannot be null.");
            Ensure.NotNull(libraryIOService, $"{nameof(libraryIOService)} cannot be null.");
            Ensure.NotNull(uploadService, $"{nameof(uploadService)} cannot be null.");
            Ensure.NotNull(thumbnailService, $"{nameof(thumbnailService)} cannot be null.");
            Ensure.NotNull(libraryZipService, $"{nameof(libraryZipService)} cannot be null.");
            Ensure.NotNull(jobsManager, $"{nameof(jobsManager)} cannot be null.");
            Ensure.NotNull(log, $"{nameof(log)} cannot be null.");
            Ensure.NotNull(tagManager, $"{nameof(tagManager)} cannot be null.");

            _folderManager = folderManager;
            _documentService = documentService;
            _libraryIOService = libraryIOService;
            _uploadService = uploadService;
            _thumbnailService = thumbnailService;
            _libraryZipService = libraryZipService;
            _jobs = jobsManager;
            _log = log;
            _tagManager = tagManager;
            _imageProcessors = imageProcessors;
            _env = env;
        }
        #endregion // Constructors
        //#region Public API
        //#region Folders
        //public async Task<IEnumerable<Folder>> GetFoldersAsync(string ownerId, string folderId)
        //{
        //    Ensure.NotNullOrEmpty(ownerId, $"{ownerId} cannot be null or empty.");

        //    if (string.IsNullOrEmpty(folderId))
        //    {
        //        // Only necessary for the root folder
        //        await EnsureSystemFoldersAsync(ownerId);
        //    }

        //    var parent = string.IsNullOrEmpty(folderId)
        //        ? await _folderManager.GetRootFolderAsync(ownerId) 
        //        : await _folderManager.GetFolderAsync(folderId);

        //    return ((Folder) parent).ChildFolders;  // TODO CLean this up...casting is bad way to do this
        //}
        public async Task<Stream> GetThumbnailStream(string id)
        {
            Ensure.Argument.NotNullOrEmpty(id, "id");

            Stream returnedStream;
            var physicalLocation = await GetLibraryPhysicalLocation(id);
            var document = await GetFileDocument(id);

            if (document.FileType == FileType.Image)
            {
                var imagethumbnailProcessor = _imageProcessors.FirstOrDefault(x => x.GetType().Name == typeof(ThumbnailProcessor).Name);

                returnedStream = imagethumbnailProcessor.GetFileStream(physicalLocation, id);
            }else
            {
                returnedStream = await _thumbnailService.GetThumbnailAsync(document, DefaultThumbSize, DefaultThumbSize, physicalLocation);  
            }

            //No thumbnail was return, then get default file thumnail image from the system.
            if (returnedStream == null)
            {
                var fileThumbPath = Path.Combine(_env.WebRootPath, "systemImages", "FileThumbnail.png");
                
                if (File.Exists(fileThumbPath))
                {
                    returnedStream = new FileStream(fileThumbPath, FileMode.Open, FileAccess.Read);
                }
            }

            return returnedStream;

        }

        public async Task<Stream> GetLowResolutionStream(string id)
        {
            Ensure.Argument.NotNullOrEmpty(id, "id");
            
            var physicalLocation = await GetLibraryPhysicalLocation(id);

            var thumbnailProcessor = _imageProcessors.FirstOrDefault(x => x.GetType().Name == typeof(NominalResolutionProcessor).Name);

            return thumbnailProcessor.GetFileStream(physicalLocation, id);

            //return await _thumbnailService.GetThumbnailAsync(doc, DefaultThumbSize, DefaultThumbSize, physicalLocation);            
        }

        public async Task<Stream> GetCroppedImageStream(string id, int x, int y, int width, int height)
        {
            Ensure.Argument.NotNullOrEmpty(id, "id");
            Ensure.Argument.NotNull(x, "x");
            Ensure.Argument.NotNull(y, "y");
            Ensure.Argument.NotNull(width, "width");
            Ensure.Argument.NotNull(height, "height");

            var physicalLocation = await GetLibraryPhysicalLocation(id);
            
            return _libraryIOService.CropImage(id, x, y, width, height, physicalLocation);
            
        }

        internal async Task DeleteFileDocument(string documentId, string ownerId)
        {
            var library = await _folderManager.GetDocumentLibraryAsync(ownerId);

            //Delete Original
            _libraryIOService.DeleteFileDocument(documentId, library.Location);

            //Delete each version of this file
            foreach (var processor in _imageProcessors)
            {
                await processor.Delete(library.Location, documentId);
            }

        }

      
        public async Task<Stream> GetResizedImageStream(string id, int width, int height)
        {
            var doc = await _documentService.GetAsync(id);
            if (doc == null)
            {
                return null;
            }
            var physicalLocation = await GetLibraryPhysicalLocation(id);

            return await _thumbnailService.GetThumbnailAsync(doc, width, height, physicalLocation);
        }

        //public async Task<Folder> AddFolderAsync(string name, string ownerId, string parentId = null)
        //{
        //    Ensure.NotNullOrEmpty(name, $"{name} cannot be null or empty.");
        //    Ensure.NotNullOrEmpty(ownerId, $"{ownerId} cannot be null or empty.");

        //    var result = new Folder()
        //    {
        //        DocumentType = typeof(FileDocument).FullName,
        //        FolderType = typeof(Folder).FullName,
        //        OwnerLevel = OwnerLevel.User,
        //        OwnerId = ownerId,
        //        CreatedBy = ownerId,
        //        FolderFlags = FolderFlag.Shared,
        //        Id = KeyGenerator.CreateShortGuid(),
        //        ParentFolder = string.IsNullOrEmpty(parentId) ? null : (await GetFolderContentsAsync(parentId)),
        //        ParentId = parentId,
        //        Title = name
        //    };

        //    result = (await _folderManager.CreateFolderAsync(result)).ToFolder();

        //    return result;
        //}

        //public async Task RemoveFolderAsync(string folderId)
        //{
        //    Ensure.NotNullOrEmpty(folderId, $"{folderId} cannot be null or empty.");

        //    var folder = await _folderManager.GetFolderAsync(folderId);
        //    var parent = string.IsNullOrEmpty(folder.ParentId) ? null : await _folderManager.GetFolderAsync(folder.ParentId);

        //    if (await IsTrashFolderAsync(parent)) //(parent.IsTrashFolder())
        //    {
        //        await _folderManager.RemoveFolderAsync(folder);
        //    }
        //    else
        //    {
        //        // Recycle
        //        await _folderManager.MoveFolderAsync(folder, await GetTrashFolderAsync(folder.OwnerId));
        //    }
        //}

        //public async Task RenameFolderAsync(string ownerId, string folderId, string newName)
        //{
        //    // NOT WORKING YET (Requires Federation to work) Ensure.NotNullOrEmpty(ownerId, $"{ownerId} cannot be null or empty.");
        //    Ensure.NotNullOrEmpty(folderId, $"{folderId} cannot be null or empty.");
        //    Ensure.NotNullOrEmpty(newName, $"{newName} cannot be null or empty.");

        //    var folder = await _folderManager.GetFolderAsync(folderId);
        //    folder.Title = newName;
        //    await _folderManager.UpdateFolderAsync(folder);
        //}

        //public async Task MoveFolderAsync(string ownerId, string folderId, string destinationId)
        //{
        //    // NOT WORKING YET (Requires Federation to work) Ensure.NotNullOrEmpty(ownerId, $"{ownerId} cannot be null or empty.");
        //    Ensure.NotNullOrEmpty(folderId, $"{folderId} cannot be null or empty.");
        //    Ensure.NotNullOrEmpty(destinationId, $"{destinationId} cannot be null or empty.");

        //    var folder = await _folderManager.GetFolderAsync(folderId);
        //    var destination = await _folderManager.GetFolderAsync(destinationId);

        //    await _folderManager.MoveFolderAsync(folder, destination);
        //}

        //public async Task CopyFolderAsync(string ownerId, string folderId, string destinationId, string newName)
        //{
        //    // NOT WORKING YET (Requires Federation to work) Ensure.NotNullOrEmpty(ownerId, $"{ownerId} cannot be null or empty.");
        //    Ensure.NotNullOrEmpty(folderId, $"{folderId} cannot be null or empty.");
        //    Ensure.NotNullOrEmpty(destinationId, $"{destinationId} cannot be null or empty.");

        //    var folder = await _folderManager.GetFolderAsync(folderId);
        //    var destination = await _folderManager.GetFolderAsync(destinationId);

        //    await CopyFolderAsync(folder, destination, newName);
        //}

        //private async Task CopyFolderAsync(IFolder folder, IFolder destination, string newName)
        //{
        //    newName = (newName ?? string.Empty).Trim();

        //    folder.Id = KeyGenerator.CreateShortGuid();
        //    folder.Title = newName ?? folder.Title;
        //    folder.ParentId = destination.Id;

        //    await _folderManager.CreateFolderAsync(folder);

        //    var contents = await GetFolderContentsAsync(folder.Id);
        //    // Copy the files in this folder
        //    foreach (var document in await GetDocumentsInFolder(folder.Id))
        //    {
        //        await CopyDocumentAsync(document.DocumentId, folder.ParentId, folder.Id, null);
        //    }

        //    // Copy the folders in this folder
        //    foreach (var subFolder in contents.ChildFolders)
        //    {
        //        await CopyFolderAsync(subFolder, folder, null);
        //    }
        //}

        //public async Task AddFolderTag(string id, int tagId)
        //{
        //    Ensure.Argument.NotNullOrEmpty(id, "id");
        //    Ensure.Argument.NotNullOrEmpty(tagId.ToString(), "tagId");

        //    var tag = await _tagManager.GetById(tagId);
        //    var folder = await _folderManager.GetFolderAsync(id);

        //    await _tagManager.AddTag(folder, tag);
        //}

        //public async Task RemoveFolderTag(string id, int tagId)
        //{
        //    Ensure.Argument.NotNullOrEmpty(id, "id");
        //    Ensure.Argument.NotNullOrEmpty(tagId.ToString(), "tagId");

        //    var tag = await _tagManager.GetById(tagId);
        //    var folder = await _folderManager.GetFolderAsync(id);

        //    await _tagManager.RemoveTag(folder, tag);
        //}
        //#endregion // Folders
        //#region Documents

        //public async Task UpdateDocument(FileDocument document, string userId)
        //{
        //    Ensure.Argument.NotNull(document);

        //    await _documentService.UpdateAsync(document);

        //    await _log.LogEventWriteAsync(document, userId);
        //}

        //// NOTE: isLogged allows internal usage of GetDocument without showing them as user-level log events. Really only want to log
        //// explicit reads done by the user (i.e. by public controller methods whose purpose is to actually return document information)
        //public async Task<FileDocument> GetDocumentAsync(string documentId, string userId, bool isLogged = true)
        //{
        //    if (String.IsNullOrEmpty(documentId))
        //        throw new ArgumentNullException($"{nameof(documentId)} cannot be null.");

        //    var result = await _documentService.GetAsync(documentId);

        //    if (isLogged && result != null)
        //    {
        //        await _log.LogEventReadAsync(result, userId);
        //    }

        //    return result;
        //}

        //public async Task<List<FileDocument>> GetDocumentsAsync(string ownerId)
        //{
        //    if (String.IsNullOrEmpty(ownerId))
        //        throw new ArgumentNullException("ownerId cannot be null.");

        //    var folders = await _folderManager.GetFoldersAsync(ownerId);
        //    var results = folders.SelectMany(x => _folderManager.GetDocumentsAsync(x).GetAwaiter().GetResult())
        //        .ToList();

        //    await Task.WhenAll(results.Select(doc => _log.LogEventReadAsync(doc, ownerId)));

        //    return results;
        //}

        //public async Task<IEnumerable<FileDocument>> GetDocumentsInFolder(string folderId)
        //{
        //    if (String.IsNullOrEmpty(folderId))
        //        throw new ArgumentNullException("ownerId folderId be null.");

        //    var folder = await _folderManager.GetFolderAsync(folderId);
        //    var results = await _folderManager.GetDocumentsAsync(folder);

        //    await Task.WhenAll(results.Select(doc => _log.LogEventReadAsync(doc, folderId)));

        //    return results;
        //}

        //public async Task<FileDocument> AddDocumentAsync(string fileFullPath, string fileType, string folderId, IFormFile file, string documentId = null)
        //{
        //    Ensure.Argument.Is(file.Length > 0, "Document must be supplied.");
        //    Ensure.Argument.NotNullOrEmpty(folderId);
        //    Ensure.NotNullOrEmpty(fileFullPath);

        //    var fileName = Path.GetFileName(fileFullPath);

        //    var document = new FileDocument()
        //    {
        //        DocumentId = string.IsNullOrEmpty(documentId) ? KeyGen.NewGuid() : documentId,
        //        FileName = fileName,
        //    };
        //    return await AddDocumentAsync(file, folderId, document);
        //}

        //public async Task<FileDocument> AddDocumentAsync(IFormFile file, string folderId, FileDocument document)
        //{
        //    var folder = await _folderManager.GetFolderAsync(folderId);
        //    System.Diagnostics.Debug.Assert(folder != null);    // Shouldn't ever trip due to Ensure, but First() throws an ugly excception

        //    document.ContentLength = file.Length;
        //    document.FileType = document.GetFileType();

        //    using (var stream = file.OpenReadStream())
        //    {
        //        await _uploadService.SetFileStreamAsync(document, stream);
        //    }

        //    await _documentService.CreateAsync(document);
        //    await _folderManager.AddDocumentAsync(document, folder);

        //    await _log.LogEventCreateAsync(document, folder.OwnerId);

        //    return document;
        //}

        //public async Task RemoveDocumentAsync(string id)
        //{
        //    Ensure.Argument.NotNullOrEmpty(id);

        //    await _documentService.DeleteAsync(id);
        //}

        //public async Task MoveDocumentAsync(string documentId, string sourceFolderId, string destinationFolderId)
        //{
        //    Ensure.Argument.NotNullOrEmpty(destinationFolderId);
        //    Ensure.Argument.NotNullOrEmpty(documentId);

        //    var document = await _documentService.GetAsync(documentId);
        //    var source = await _folderManager.GetFolderAsync(sourceFolderId);
        //    var destination = await _folderManager.GetFolderAsync(destinationFolderId);

        //    await _folderManager.MoveDocumentAsync(document, source, destination);

        //    await _log.LogEventWriteAsync(document, destination.OwnerId);
        //}

        //public async Task CopyDocumentAsync(string documentId, string sourceFolderId, string destinationFolderId, string newName)
        //{
        //    Ensure.Argument.NotNullOrEmpty(destinationFolderId);
        //    Ensure.Argument.NotNullOrEmpty(documentId);

        //    var documentToMove = await _documentService.GetAsync(documentId);
        //    var originFolder = await _folderManager.GetFolderAsync(sourceFolderId);
        //    var destinationFolder = await _folderManager.GetFolderAsync(destinationFolderId);

        //    var originalId = documentToMove.DocumentId;
        //    var originalOwnerId = originFolder.OwnerId;

        //    newName = newName?.Trim();
        //    if (!string.IsNullOrEmpty(newName) && documentToMove.FileName != newName)
        //    {
        //        // Rename
        //        documentToMove.FileName = newName;
        //    }

        //    // Copy the document object
        //    await _folderManager.CopyDocumentAsync(documentToMove, destinationFolder);

        //    // Log it
        //    await _log.LogEventCreateAsync(documentToMove, destinationFolder.OwnerId);
        //}

        //public async Task RenameDocumentAsync(string documentId, string newName, string userId)
        //{
        //    Ensure.Argument.NotNullOrEmpty(documentId);
        //    Ensure.Argument.NotNullOrEmpty(newName);

        //    var document = await _documentService.GetAsync(documentId);
        //    document.FileName = newName;
        //    await _documentService.UpdateAsync(document);

        //    await _log.LogEventWriteAsync(document, userId);
        //}

        public async Task UploadDocumentAsync(string id, string userId, Stream file)
        {
            Ensure.NotNullOrEmpty(id, $"{nameof(id)} cannot be null.");
            Ensure.NotNull(file, $"{nameof(file)} cannot be null.");

            var document = await GetFileDocument(id);

            var physicalLocation = await GetLibraryPhysicalLocation(id);

            //Save original file.
            await _uploadService.SetFileStreamAsync(document, file, physicalLocation);

            //apply post processing tasks
            foreach (var processor in _imageProcessors)
            {
                await processor.Invoke(physicalLocation, document);
            }
           
            await _log.LogEventWriteAsync(document, userId);
        }

        public async Task<Stream> DownloadDocumentAsync(string id, string userId)
        {
            Ensure.NotNullOrEmpty(id, $"{nameof(id)} cannot be null.");

            var document = await GetFileDocument(id);
            return await DownloadDocumentAsync(document, userId); 
        }

        // NOTE As this is fully-async (no waiting), it is not async per se

        public async Task<FileDocument> DownloadItemsAsJobAsync(string ownerId, string[] folderIds, string[] documentIds, string targetFolderId)
        {
            if (folderIds == null) folderIds = new string[0];
            if (documentIds == null) documentIds = new string[0];
            if (folderIds.Length == 1) folderIds = (folderIds[0] ?? string.Empty).Split(',');
            if (documentIds.Length == 1) documentIds = (documentIds[0] ?? string.Empty).Split(',');

            Ensure.NotNullOrEmpty(ownerId, $"{nameof(ownerId)} cannot be null or empty.");
            Ensure.That<ArgumentException>(folderIds.Any(id => !string.IsNullOrEmpty(id)) || documentIds.Any(id => !string.IsNullOrEmpty(id)), "Must download at least one folder and/or document.");

            var folders = await Task.WhenAll(folderIds.Where(id => !string.IsNullOrEmpty(id)).Select(id => _folderManager.GetFolderAsync(id)));
            var documents = await Task.WhenAll(documentIds.Where(id => !string.IsNullOrEmpty(id)).Select(id => _documentService.GetAsync(id)));   // ToArray forces a download now, before the stream is dispo

            Ensure.That<ArgumentException>(folders.Any(x => x != null) || documents.Any(x => x != null), "Folder/document ID(s) not found.");

            var targetFolder = string.IsNullOrEmpty(targetFolderId) ? await _folderManager.GetRootFolderAsync(ownerId) : await _folderManager.GetFolderAsync(targetFolderId);

            // The read audit is already performed as part of the Document DB fetch, so no audit here

            return await DownloadItemsAsJobAsync(ownerId, targetFolder, folders, documents);
        }

        ////public async Task<IEnumerable<DbLogEvent>> GetDocumentLogsAsync(string documentId)
        ////{
        ////    if (string.IsNullOrEmpty(documentId)) return new DbLogEvent[0];    // So the UI component can do an initial load

        ////    return (await _logFetcher.GetDocumentEventsAsync(documentId));
        ////}

        private async Task<FileDocument> CreateZipDocumentAsync(IFolder targetFolder, IFolder[] folders, FileDocument[] documents)
        {
            Ensure.NotNull(targetFolder, $"{nameof(targetFolder)} cannot be null.");

            const int maxDescription = int.MaxValue;    // nvarchar(max)
            var files = string.Join(",", folders.Select(folder => folder.Title).Concat(documents.Select(doc => doc.FileName)).ToArray());
            if (files.Length > maxDescription)
            {
                files = files.Substring(0, maxDescription - 3) + "...";
            }

            // No database, no audit (deferred)
            var result = new FileDocument()
            {
                FileName = await CreateZipFileName(targetFolder.OwnerId, targetFolder.Id, folders, documents),
                Created = DateTime.Now,
                ContentLength = 0,
                Description = files,
                FileType = typeof(FileDocument).Name,
                DocumentId = Guid.NewGuid().ToString(),
                //Folder = targetFolder,
                //FolderId = targetFolder.Id
            };

            return result;
        }

        //public async Task<Folder> GetTrashFolderAsync(string ownerId)
        //{
        //    var root = await GetRootFolderContentsAsync(ownerId);

        //    var trash = root.ChildFolders.SingleOrDefault(x => IsTrashFolderAsync(x).GetAwaiter().GetResult());

        //    return trash;
        //}

        //private async Task DeleteDocument(FileDocument document, IFolder folder)
        //{
        //    if (await IsTrashFolderAsync(folder))
        //    {
        //        await DeleteDocumentAsync(document, folder);
        //    }
        //    else
        //    {
        //        // Recycle
        //        await RecycleDocumentAsync(document, folder);
        //    }
        //}

        //private async Task RecycleDocumentAsync(FileDocument document, IFolder folder)
        //{
        //    await EnsureTrashFolderAsync(folder.OwnerId);
        //    var trash = await GetTrashFolderAsync(folder.OwnerId);

        //    // Move document in folder to Trash
        //    await MoveDocumentAsync(document.DocumentId, folder.Id, trash.Id);
        //}

        //private async Task DeleteDocumentAsync(FileDocument document, IFolder folder)
        //{
        //    await _folderManager.RemoveDocumentAsync(document, folder);
        //    await _documentService.DeleteAsync(document);

        //    await _log.LogEventDeleteAsync(document, folder.OwnerId);
        //}

        //private async Task RecycleFolder(IFolder folder)
        //{
        //    var ownerId = folder.OwnerId;
        //    var trash = await GetTrashFolderAsync(ownerId);
        //    await MoveFolderAsync(ownerId, folder.Id, trash.Id);
        //}

        //private async Task DeleteFolder(IFolder folder)
        //{
        //    await _folderManager.RemoveFolderAsync(folder);
        //}

        private async Task<string> CreateZipFileName(string ownerId, string targetFolderId, IEnumerable<IFolder> folders, IEnumerable<FileDocument> documents)
        {
            if ((documents == null || !documents.Any()) && (folders != null && folders.Count() == 1))
            {
                return await GetUniqueFileNameAsync(folders.First().Title + ".zip", ownerId, targetFolderId);
            }

            return await GetUniqueFileNameAsync($"Downloaded ({ToFileNameFriendly(DateTime.Now)}).zip", ownerId, targetFolderId);
        }

        private async Task<string> GetUniqueFileNameAsync(string fileName, string ownerId, string targetFolderId)
        {
            var i = 0;
            while (await HasFileNameAsync(ownerId, targetFolderId, fileName))
            {
                fileName = $"{RemoveExtension(fileName)}_{++i}.zip";

                if (i >= DefaultDocumentQuota) throw new InvalidOperationException($"Unable to create a unique filename for '{fileName}'.");
            }

            return fileName;
        }

        private static string RemoveExtension(string fileName)
        {
            var i = fileName.LastIndexOf('.');
            if (i < 0) return fileName;

            return fileName.Substring(0, i);
        }

        private async Task<bool> HasFileNameAsync(string ownerId, string folderId, string fileName)
        {
            var folder = await _folderManager.GetFolderAsync(folderId);
            var folders = await _folderManager.GetFoldersAsync(folder);
            var documents = await _folderManager.GetDocumentsAsync(folder);

            return folders.Any(x => string.Equals(fileName, x.Title, StringComparison.CurrentCultureIgnoreCase))
                || documents.Any(x => string.Equals(fileName, x.Title, StringComparison.CurrentCultureIgnoreCase));
        }

        private static string ToFileNameFriendly(DateTime timeStamp)
        {
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0:d} {0:t}",
                timeStamp);
        }

        public async Task<Stream> DownloadItemsAsync(string ownerId, string[] folderIds, string[] documentIds)
        {

            if (folderIds == null) folderIds = new string[0];
            if (documentIds == null) documentIds = new string[0];
            if (folderIds.Length == 1) folderIds = (folderIds[0] ?? string.Empty).Split(',');
            if (documentIds.Length == 1) documentIds = (documentIds[0] ?? string.Empty).Split(',');

            Ensure.NotNullOrEmpty(ownerId, $"{nameof(ownerId)} cannot be null or empty.");
            Ensure.That<ArgumentException>(folderIds.Any(id => !string.IsNullOrEmpty(id)) || documentIds.Any(id => !string.IsNullOrEmpty(id)), "Must download at least one folder and/or document.");

            // TODO: This will have to be fixed to deal with shared folders (other users' folders)
            var folders = await Task.WhenAll(folderIds.Where(id => !string.IsNullOrEmpty(id)).Select(async id => await _folderManager.GetFoldersAsync(await _folderManager.GetFolderAsync(id))));
            var documents = await Task.WhenAll(documentIds.Where(id => !string.IsNullOrEmpty(id)).Select(async id => await _documentService.GetAsync(id)));   // ToArray forces a download now, before the stream is dispo

            Ensure.That<ArgumentException>(folders.Any(x => x != null) || documents.Any(x => x != null), "Folder/document ID(s) not found.");

            return await DownloadItemsAsync(folders.SelectMany(x => x), documents, ownerId);
        }
        //#endregion // Documents
        //#endregion // Public API
        //#region Download workers

        private async Task<Stream> DownloadItemsAsync(IEnumerable<IFolder> folders, IEnumerable<FileDocument> documents, string userId)
        {
            documents = documents.Concat(folders.SelectMany(folder => _folderManager.GetDocumentsAsync(folder).GetAwaiter().GetResult()));

            #region Single-file shortcut
            if ((folders == null || !folders.Any()) && documents != null && documents.Count() == 1)
            {
                // Don't return a zip if you don't have to
                return await DownloadDocumentAsync(documents.Single(), userId);
            }
            #endregion // Single-file shortcut

            throw new NotImplementedException("Need to account for documents not having a FolderId property");
            //var foldersByFolderId = documents.Select(x => x.FolderId).ToDictionary(x => x, x => _folderManager.GetFolderAsync(x).GetAwaiter().GetResult());

            //// TODO: This is all-or-nothing...if any file fails to download, the entire zip fails. Should we perhaps just skip the file?
            //var kvs = GetFlatDocuments(folders).Concat(documents)
            //    .ToDictionary(document => Path.Combine(foldersByFolderId[document.FolderId].ToString(), document.FileName),
            //                  document => _libraryIOService.ReadFileAsStream(foldersByFolderId[document.FolderId], document));
            //try
            //{
            //    var result = await _libraryZipService.ZipAsync(kvs);
            //    result.Position = 0;    // Ensures we have data coming back
            //    return result;
            //}
            //finally
            //{
            //    foreach (var kv in kvs)
            //    {
            //        var stream = kv.Value;
            //        if (stream == null) continue;

            //        stream.Dispose();
            //    }
            //}
        }

        private async Task<FileDocument> DownloadItemsAsJobAsync(string ownerId, IFolder targetFolder, IFolder[] folders, FileDocument[] documents)
        {
            Ensure.NotNullOrEmpty(ownerId, $"");

            // Choose a guid for the zipped output file name
            // NOPE! The Guid should be the documentID for the zip file you just created. 
            var result = await CreateZipDocumentAsync(targetFolder, folders, documents);

            await _documentService.CreateAsync(result);
            await _folderManager.AddDocumentAsync(result, targetFolder);

            await _log.LogEventCreateAsync(result, ownerId);

            // So, I need to create the DB document record (and GUID) immediately. When the JobZip finishes, *it* needs to push
            // that data onto the file system!
            // Documents have their own folder information and are thus self-referencing, which the JobsScheduler (JSON) cannot handle
            // This is more minimalist data, but as the job is async, the extra CPU time to perform lookups is OK here.
            var tf = targetFolder.ToFolder();
            var kvs = GetFlatDocuments(folders).Concat(documents.Select(x => new DocumentFolderEntry(tf, x)))
                .ToDictionary(document => $"{_folderManager.GetPathAsync(document.Folder).Result}/{document.Document.FileName}",
                              document => document.Document.DocumentId)
                .ToArray();

            // NOTE: Do *not* await this, as the whole point is to run this action asynchronously
            _jobs.EnqueueAsync(() => new JobZip(this, _documentService, _uploadService, _libraryZipService, _folderManager)
                .Execute(result, ownerId, targetFolder.Id, kvs));


            // Return the guid to the view
            return result;
        }

        private IEnumerable<DocumentFolderEntry> GetFlatDocuments(IEnumerable<IFolder> folders)
        {
            var childDocuments = folders.Select(folder =>
            {
                var childFolders = _folderManager.GetFoldersAsync(folder).Result;
                if (childFolders == null)
                {
                    return Enumerable.Empty<DocumentFolderEntry>();
                }
                else
                {
                    return GetFlatDocuments(childFolders);
                }
            });
            return folders.SelectMany(folder => _folderManager.GetDocumentsAsync(folder).GetAwaiter().GetResult().Select(x => new DocumentFolderEntry(folder.ToFolder(), x)))
                .Concat(childDocuments.SelectMany(x => x)); 
        }
        //#endregion // Download workers

        //#region Ensures

        //private async Task EnsureSystemFoldersAsync(string ownerId)
        //{
        //    await EnsureTrashFolderAsync(ownerId);
        //}

        //private static object _syncTrash = new object();
        //private async Task EnsureTrashFolderAsync(string ownerId)
        //{
        //    lock (_syncTrash)
        //    {
        //        var root = GetRootFolderContentsAsync(ownerId, false).GetAwaiter().GetResult();   // Do this within the lock so it is part of the atomic operation (consistent)

        //        var trash = root.ChildFolders.SingleOrDefault(x => IsTrashFolderAsync(x).GetAwaiter().GetResult());
        //        if (trash == null)
        //        {
        //            AddFolderAsync("Trash", ownerId, root.Id).GetAwaiter().GetResult();
        //        }
        //    }
        //}

        // REDUNDANT?
        //private async Task EnsureTrashFolderAsync(string ownerId)
        //{
        //    var root = await GetRootUserFolderAsync(ownerId);
        //    var dbRoot = await GetFullFolderAsync(root.Id);
        //    var trash = dbRoot.ChildFolders.SingleOrDefault(x => IsTrashFolderAsync(x).GetAwaiter().GetResult());
        //    if (trash == null)
        //    {
        //        var id = Convert.ToBase64String(Guid.NewGuid().ToByteArray())
        //            .Replace("=", "").Replace("/", "-");
        //        trash = new Folder()
        //        {
        //            Title = GetTrashFolderName(root),
        //            DocumentType = string.Empty,
        //            FolderLevel = FolderLevel.Client,
        //            FolderType = FolderType.Hidden,
        //            Id = id,
        //            OwnerPoolId = ownerId,
        //            ParentFolder = dbRoot,
        //            ParentId = root.Id
        //        };
        //        await CreateFolderAsync(string.Empty, trash);
        //    }
        //}
        //#endregion // Ensures

        private async Task<FileDocument> GetFileDocument(string documentId)
        {
            
            var document = await _documentService.GetAsync(documentId);
            return document;
        }

        private async Task<Stream> DownloadDocumentAsync(FileDocument document, string userId)
        {
            Ensure.NotNull(document, $"{nameof(document)} cannot be null.");

            var physicalLocation = await GetLibraryPhysicalLocation(document.DocumentId);

            var result = await _uploadService.GetFileStreamAsync(document, physicalLocation);

            await _log.LogEventReadAsync(document, userId);

            return result;
        }

        private async Task<string> GetLibraryPhysicalLocation(string documentId)
        {
            //attempt to get the library location
            var libraryLocation = await _documentService.GetDocumentLibraryLocation(documentId);

            return libraryLocation;
        }
    }
}
