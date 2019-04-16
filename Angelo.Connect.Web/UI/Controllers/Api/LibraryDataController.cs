using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Services;
using Kendo.Mvc.Extensions;
using Microsoft.AspNetCore.Http;
using Angelo.Connect.Models;
using System.Net.Http;
using System.IO;
using Angelo.Connect.Extensions;
using Angelo.Connect.Abstractions;
using Microsoft.Extensions.Options;
using Angelo.Connect.Logging;
using Angelo.Connect.Security;
using Newtonsoft.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Angelo.Connect.Configuration;
using Angelo.Connect.Documents;

namespace Angelo.Connect.Web.UI.Controllers
{
    public class LibraryDataController : BaseController
    {
        #region Dependencies
        private IFolderManager<FileDocument> _folderManager;
        private IDocumentService<FileDocument> _documentService;
        private TagManager _tagManager;
        private DbLoggerProvider _log;
        private DbLogService _logFetcher;
        private const string SharedDocumentsFolderId = "sharedcontent";

        private DriveOptions _driveOptions;
        private ResourceManager _resourceManager;
        private FileFolderBrowser _fileFolderBrowser;
        private IAuthorizationService _authorizationService;
        private AdminContext _adminContext;
        private SiteContext _siteContext;
        #endregion // Dependencies
        #region Constructors
        public LibraryDataController(
            IFolderManager<FileDocument> folderManager,
            IDocumentService<FileDocument> documentService,
            IOptions<DriveOptions> driveOptions,
            TagManager tagManager,
            DbLoggerProvider log,
            DbLogService logFetcher,
            ILogger<LibraryDataController> logger,
            ResourceManager resourceManager,
            FileFolderBrowser fileFolderBrowser,
            IAuthorizationService authorizationService,
            IContextAccessor<AdminContext> adminContextAccessor,
            IContextAccessor<SiteContext> siteContextAccessor) : base(logger)
        {
            _folderManager = folderManager;
            _documentService = documentService;
            _driveOptions = driveOptions.Value;
            _tagManager = tagManager;
            _log = log;
            _logFetcher = logFetcher;
            _resourceManager = resourceManager;
            _fileFolderBrowser = fileFolderBrowser;
            _authorizationService = authorizationService;
            _adminContext = adminContextAccessor.GetContext();
            _siteContext = siteContextAccessor.GetContext();
        }
        #endregion // Constructors
        #region Public actions

        [Authorize]
        [HttpGet, Route("/sys/library/api/media/folders")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> MediaFolders(string userId, string id = null)
        {
            IList<Folder> foldersToReturn = new List<Folder>();

            if (string.IsNullOrEmpty(id))
            {
                
                if (await _folderManager.GetDocumentLibraryAsync(userId) == null)
                {
                    var locationResolver = new DocumentPhysicalLocationResolver("User", _adminContext.ClientId, _adminContext.SiteId, userId);
                    await _folderManager.CreateDocumentLibrary(userId, "User", locationResolver.Resolve());
                }

                foldersToReturn.Add((Folder)await _folderManager.GetRootFolderAsync(userId, true));

                //check permissions for site library access and add it with the result treeview
                if (await _authorizationService.AuthorizeAsync(User, PolicyNames.SiteLibraryRead))
                {
                    var siteRoot = ((Folder)await _folderManager.GetRootFolderAsync(_adminContext.SiteId));
                    siteRoot.Title = "Site Library";
                    foldersToReturn.Add(siteRoot);
                }

                //check permissions for client library access and add it with the result treeview
                if (await _authorizationService.AuthorizeAsync(User, PolicyNames.ClientLibraryRead))
                {
                    var clientRoot = ((Folder)await _folderManager.GetRootFolderAsync(_adminContext.ClientId));
                    clientRoot.Title = "Client Library";
                    foldersToReturn.Add(clientRoot);
                }

                //get shared Document root folder
                //Get Product Definition flag for sharing Documents
                var isSiteDocumentSharingEnabled = _siteContext.ProductContext.Features.Get(FeatureId.DocumentSharing)?.GetSettingValue<bool>("enabled") ?? false;

                if (isSiteDocumentSharingEnabled && await _fileFolderBrowser.IsAnythingShared(userId))
                {
                    foldersToReturn.Add(new Folder()
                    {
                        Title = "Shared Documents",
                        Id = SharedDocumentsFolderId,
                    });
                }

            }
            else
            {
                var folder = await _folderManager.GetFolderAsync(id);
                foldersToReturn.AddRange((IEnumerable<Folder>)await _folderManager.GetFoldersAsync(folder, true));
            }

            IList<TreeView> mediaTree = new List<TreeView>();

            var model = foldersToReturn.Select(x => new
            {
                Id = x.Id,
                Title = string.IsNullOrEmpty(x.Title) ? "My Library" : x.Title,
                Path = "",
                HasChildren = x.ChildFolders.Any()
            });


            return new JsonResult(model, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

        }

        [Authorize]
        [HttpGet, Route("/sys/library/api/media/files")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IActionResult> MediaFiles(string userId, string id = null)
        {

            IList<FileDocument> files = new List<FileDocument>();

            if (!string.IsNullOrEmpty(id))
            {
                if (id == SharedDocumentsFolderId)
                {
                    //files.AddRange((List<FileDocument>)await _fileFolderBrowser.GetSharedContent(userId));
                    foreach (var contentType in await _fileFolderBrowser.GetSharedContent(userId))
                    {
                        var document = contentType as FileDocument;
                        files.Add(document);
                    }
                }
                else
                {
                    var documents = await _folderManager.GetDocumentsInFolderAsync(id);
                    files.AddRange(documents);
                }
                
            }

            var model = files.Select(x => new
            {
                Id = x.DocumentId,
                Name = x.FileName,
                Size = x.ContentLength,
                ThumbUrl = "",
                Url = "",
                Type = "f",
                ContentType = x.FileType,
                Extension = x.FileExtension,
                CreatedDateString = x.Created.ToString("O")
            });


            return new JsonResult(model, new Newtonsoft.Json.JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() });

        }

        #region Folder API
        [Authorize]
        [HttpGet, Route("/api/user/library/folders")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IEnumerable<IFolder>> GetFolders(string userId, string id = null)
        {
            return await _folderManager.GetFoldersAsync(string.IsNullOrEmpty(id)
                ? await _folderManager.GetRootFolderAsync(userId)
                : await _folderManager.GetFolderAsync(id));
        }

        [Authorize]
        [HttpGet, Route("/api/user/library/folderTree")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IEnumerable<Folder>> GetFolderTree(string userId, string id = null, bool ignoreTrash = true)
        {
            return await _folderManager.GetFoldersRecursivelyAsync(string.IsNullOrEmpty(id)
                    ? await _folderManager.GetRootFolderAsync(userId)
                    : await _folderManager.GetFolderAsync(id),
                ignoreTrash);
        }

        [Authorize]
        [HttpGet, Route("/api/user/library/folderTreePath")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IEnumerable<IFolder>> GetFolderTreePath(string id)
        {
            return await _folderManager.GetFoldersPathAsync(await _folderManager.GetFolderAsync(id));
        }

        [Authorize]
        [HttpGet, Route("/api/user/library/getTrashFolder")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IFolder> GetTrashFolder(string userId)
        {
            return await _folderManager.GetTrashFolderAsync(userId);
        }

        [Authorize]
        [HttpGet, Route("/api/user/library/folder")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IFolder> GetFolder(string userId, string id = null)
        {
            return string.IsNullOrEmpty(id)
                ? await _folderManager.GetRootFolderAsync(userId)
                : await _folderManager.GetFolderAsync(id);
        }

        [Authorize]
        [HttpPost, Route("/api/user/library/folder/update")]
        public async Task UpdateDocument([FromBody] IFolder folder)
        {
            await _folderManager.UpdateFolderAsync(folder);
        }

        [Authorize]
        [HttpPost, Route("/api/user/library/folder")]
        public async Task<IFolder> AddFolder(string name, string ownerId, string parentId = null)
        {
            return await _folderManager.AddFolderAsync(name, ownerId, parentId);
        }

        [Authorize]
        [HttpDelete, Route("/api/user/library/folder")]
        public async Task RemoveFolder(string id, bool softDelete = true)
        {
            if (softDelete)
            {
                await _folderManager.DeleteFolderAsync(await _folderManager.GetFolderAsync(id));
            }
            else
            {
                await _folderManager.RemoveFolderAsync(await _folderManager.GetFolderAsync(id));
            }

        }

        [Authorize]
        [HttpPut, Route("/api/user/library/copy")]
        public async Task CopyFolder(string folderId, string destinationId, string newName = null)
        {
            var userId = User.GetUserId();
            await _folderManager.CopyFolderAsync(userId, folderId, destinationId, newName);
        }

        [Authorize]
        [HttpPut, Route("/api/user/library/move")]
        public async Task MoveFolder(string folderId, string destinationId)
        {
            var folder = await _folderManager.GetFolderAsync(folderId);
            var destination = await _folderManager.GetFolderAsync(destinationId);

            await _folderManager.MoveFolderAsync(folder, destination);
        }

        [Authorize]
        [HttpPut, Route("/api/user/library/rename")]
        public async Task RenameFolder(string id, string newName)
        {
            var folder = await _folderManager.GetFolderAsync(id);
            folder.Title = newName;
            await _folderManager.UpdateFolderAsync(folder);
        }

        [Authorize]
        [HttpPost, Route("/api/user/library/folder/tag")]
        public async Task AddFolderTag(string id, string tagId)
        {
            var tag = await _tagManager.GetById(tagId);
            var folder = await _folderManager.GetFolderAsync(id);

            await _tagManager.AddTag(folder, tag);
            await _tagManager.AddFolderTag(await _folderManager.GetFolderAsync(id), tagId);
        }

        [Authorize]
        [HttpDelete, Route("/api/user/library/folder/tag")]
        public async Task RemoveFolderTag(string id, string tagId)
        {
            await _tagManager.RemoveFolderTag(await _folderManager.GetFolderAsync(id), tagId);
        }
        #endregion // Folder API
        #region Document API
        [Authorize]
        [HttpGet, Route("/api/user/library/doc")]
        public async Task<FileDocument> GetDocument(string id)
        {
            Ensure.NotNullOrEmpty(id, $"{nameof(id)} cannot be null.");

            var result = await _documentService.GetAsync(id);

            if (result != null)
            {
                await _log.LogEventReadAsync(result, GetCurrentUserId());
            }

            return result;
        }

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet, Route("/api/user/library/docs")]
        public async Task<IEnumerable<FileDocument>> GetDocuments(string folderId)
        {
            var folder = await _folderManager.GetFolderAsync(folderId);
            var results = await _folderManager.GetDocumentsAsync(folder);

            await Task.WhenAll(results.Select(doc => _log.LogEventReadAsync(doc, folderId)));

            return results;
        }

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet, Route("/api/user/library/docs/shared")]
        public async Task<IEnumerable<IDocument>> GetShareDocs(string userId)
        {
            var results = await _folderManager.GetSharedDocuments(userId);

            //await Task.WhenAll(results.Select(doc => _log.LogEventReadAsync(doc, )));

            return results;
        }

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost, Route("/api/user/library/sharedocument/users")]
        public async Task<IActionResult> ShareDocumentWithUsers (string[] users, string id, IEnumerable<string> documentsToShare)
        {
            foreach (var item in documentsToShare)
            {
                //pattern for id will be
                //   [Type]_[Id]
                // where Type is FileDocument or Folder  
                //   and the Id is the Id of the content to share
                var details = item.Split('_');
                var type = details[0];
                var contentId = details[1];

                var addedResources = await _folderManager.SharedDocument(contentId, type, users);
            }

            //TODO
            // implement some type of keeping history/activity

            return new NoContentResult();
        }

        [Authorize]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpPost, Route("/api/user/library/sharedocument/groups")]
        public async Task<IActionResult> ShareDocumentWithGroups(string[] groups, IEnumerable<string> documentsToShareWithGroups)
        {
            //Note 
            //
            //pattern for id will be
            //   [Type]_[Id]
            // where Type is FileDocument or Folder  
            //   and the Id is the Id of the content to share

            foreach (var item in documentsToShareWithGroups)
            {
                var details = item.Split('_');
                var type = details[0];
                var contentId = details[1];

                //TODO split Group logic to carry provider type with groups (eg  "[connectionGroup]_[CG1]") 
                //TODO refactor claimtype to be dynamic and selectable, not hardcoded
                foreach (var group in groups)
                {
                    //note: Groups will be in the following pattern
                    //       {grouptype}_{groupId}
                    // grouptype is the security implementation group such as Connection Group
                    // and groupid is just the id
                    var groupDetail = group.Split('_');
                    var groupType = groupDetail[0];
                    var groupId = groupDetail[1];
                    await _resourceManager.AddGroupResourceClaimAsync(contentId, type , groupId, groupType, "View");
                }

            }
            return new JsonResult(groups);

            //var document = await GetDocument(id);

            //foreach (var user in addedResources)
            //{
            //    await _log.LogEventShareAsync(document, GetCurrentUserId(), user.UserId);
            //}

        }

        [Authorize]
        [HttpGet, Route("/api/user/library/docs/{userId}")]
        public async Task<IList<FileDocument>> ListDocuments(string userId)
        {
            Ensure.NotNullOrEmpty(userId, $"{nameof(userId)} cannot be null.");

            var folders = await _folderManager.GetFoldersAsync(userId);
            var results = folders.SelectMany(x => _folderManager.GetDocumentsAsync(x).GetAwaiter().GetResult())
                .ToList();

            await Task.WhenAll(results.Select(doc => _log.LogEventReadAsync(doc, userId)));

            return results;
        }

        [Authorize]
        [HttpDelete, Route("/api/user/library/doc")]
        public async Task RemoveDocument(string id, bool softDelete = true)
        {
            if (softDelete)
            {
                await _folderManager.DeleteDocumentAsync(id);
            }
            else
            {
                var documentLibrary = await _documentService.GetDocumentLibraryAsync(id);

                await _documentService.DeleteAsync(id);

                //send a requst to Drive to remove document
                var url = $"{_driveOptions.Authority}/remove/";
                using (var client = new HttpClient())
                {
                    var content = new MultipartFormDataContent();
                    content.Add(new StringContent(id), "documentId");
                    content.Add(new StringContent(documentLibrary.OwnerId), "ownerId");
                    await client.PostAsync(url, content);

                }
            }

        }

        [Authorize]
        [HttpPost, Route("/api/user/library/doc/restore")]
        public void RestoreDocument(string id)
        {
            _folderManager.RestoreDocument(id);

        }

        [Authorize]
        [HttpPost, Route("/api/user/library/folder/restore")]
        public async Task RestoreFolder(string id)
        {
            await _folderManager.RestoreFolder(id);

        }

        [Authorize]
        [HttpPut, Route("/api/user/library/doc/copy")]
        public async Task CopyDocument(string documentId, string sourceFolderId, string destinationFolderId, string newName = null)
        {
            Ensure.Argument.NotNullOrEmpty(documentId);
            Ensure.Argument.NotNullOrEmpty(sourceFolderId);
            Ensure.Argument.NotNullOrEmpty(destinationFolderId);

            var documentToMove = await _documentService.GetAsync(documentId);
            var originFolder = await _folderManager.GetFolderAsync(sourceFolderId);
            var destinationFolder = await _folderManager.GetFolderAsync(destinationFolderId);

            var originalId = documentToMove.DocumentId;
            var originalOwnerId = originFolder.OwnerId;

            newName = newName?.Trim();
            if (!string.IsNullOrEmpty(newName) && documentToMove.FileName != newName)
            {
                // Rename
                documentToMove.FileName = newName;
            }

            // Copy the document object
            await _folderManager.CopyDocumentAsync(documentToMove, destinationFolder);

            // Log it
            await _log.LogEventCreateAsync(documentToMove, destinationFolder.OwnerId);
        }

        [Authorize]
        [HttpPut, Route("/api/user/library/doc/move")]
        public async Task MoveDocument(string documentId, string sourceFolderId, string destinationFolderId)
        {
            Ensure.Argument.NotNullOrEmpty(sourceFolderId);
            Ensure.Argument.NotNullOrEmpty(destinationFolderId);
            Ensure.Argument.NotNullOrEmpty(documentId);

            var document = await _documentService.GetAsync(documentId);
            var source = await _folderManager.GetFolderAsync(sourceFolderId);
            var destination = await _folderManager.GetFolderAsync(destinationFolderId);

            await _folderManager.MoveDocumentAsync(document, source, destination);

            await _log.LogEventWriteAsync(document, destination.OwnerId);
        }

        [Authorize]
        [HttpPost, Route("/api/user/library/doc/update")]
        public async Task UpdateDocument(DocumentViewModel document)
        {
            Ensure.Argument.NotNull(document);

            var doc = await _documentService.GetAsync(document.DocumentId);

            if (doc != null)
            {
                doc.Description = document.Description;
                doc.FileName = document.FileName;
                await _documentService.UpdateAsync(doc);
                await _log.LogEventWriteAsync(doc, GetCurrentUserId());
            }



        }

        [Authorize]
        [HttpPut, Route("/api/user/library/doc")]
        public async Task RenameDocument(string documentId, string newName)
        {
            Ensure.Argument.NotNullOrEmpty(documentId);
            Ensure.Argument.NotNullOrEmpty(newName);

            var document = await _documentService.GetAsync(documentId);
            document.FileName = newName;
            await _documentService.UpdateAsync(document);

            await _log.LogEventWriteAsync(document, GetCurrentUserId());
        }

        [Authorize]
        [HttpGet, Route("/api/user/library/downloadSize")]
        public async Task<long> GetDocumentContentLength(string id)
        {
            var userId = GetCurrentUserId();
            var document = await _documentService.GetAsync(id);
            if (document == null) throw new ArgumentException($"Document not found: {id}");

            return document.ContentLength;
        }

        [Authorize]
        [HttpPost, Route("/api/user/library/doc/tag")]
        public async Task AddDocumentTag(string id, string tagId)
        {
            Ensure.Argument.NotNullOrEmpty(id, "id");
            Ensure.Argument.NotNullOrEmpty(tagId.ToString(), "tagId");

            var tag = await _tagManager.GetById(tagId);
            var document = await _documentService.GetAsync(id);

            await _tagManager.AddTag(document, tag);
        }

        [Authorize]
        [HttpDelete, Route("/api/user/library/doc/tag")]
        public async Task RemoveDocumentTag(string id, string tagId)
        {
            Ensure.Argument.NotNullOrEmpty(id, "id");
            Ensure.Argument.NotNullOrEmpty(tagId.ToString(), "tagId");

            var tag = await _tagManager.GetById(tagId);
            var document = await _documentService.GetAsync(id);

            await _tagManager.RemoveTag(document, tag);
        }

        [Authorize]
        [HttpGet, Route("/api/user/library/doc/log")]
        public async Task<LogDescriptor> GetDocumentLogs(string documentId)
        {
            var userId = GetCurrentUserId();

            var document = string.IsNullOrEmpty(documentId) ? null : await _documentService.GetAsync(documentId);
            var events = await _logFetcher.GetDocumentEventsAsync(documentId);
            var summary = events.ToSummary(userId);

            return new LogDescriptor() { Summary = summary, Events = events };
        }
        #endregion Document API

        // Moving the atomic Drive.AddDocument here, since Connect now livese here, the doc is created here
        // and then Drive is called to upload the document, followed by an update to the file size
        /// <summary>
        /// 
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost, Route("/api/user/library/doc")]
        public async Task<IActionResult> CreateAndUploadDocument(string folderId, IFormFile file = null)
        {
            if (string.IsNullOrEmpty(folderId))
            {
                return Content("");
            }

            foreach (var f in (file == null ? Request.Form.Files.Cast<IFormFile>() : new[] { file }))
            {

                var folder = await _folderManager.GetFolderAsync(folderId);
                
                //Should return existing document if we are overwriting the file with same name.
                //if new file or new name, then assume new document.
                var document = (await _folderManager.GetDocumentsAsync(folder)).FirstOrDefault(x => x.FileName == System.IO.Path.GetFileName(file.FileName)) ?? await CreateDocumentAsync(f);

                if (folder == null) throw new InvalidOperationException($"Folder not found: '{folderId}'.");

                try
                {
                    await AddDocumentToFolderAsync(document, folder);
                    try
                    {
                        var fileSize = await UploadDocumentAsync(document, f);
                        await UpdateFileSizeAsync(document, fileSize);
                    }
                    catch (Exception exc)
                    {
                        await LogErrorAsync(exc);

                        await RemoveDocumentFromFolderAsync(document, folder);

                        // Cascade so the document is deleted
                        throw;
                    }
                }
                catch (Exception exc)
                {
                    // Log error
                    await LogErrorAsync(exc);

                    // Delete the FileDocument
                    await DeleteDocumentAsync(document);
                }
            }

            return Content("");

        }
        #endregion // Public actions
        #region Private methods
        private string GetCurrentUserId()
        {
            return this.User.GetUserId();
        }

        private Task LogErrorAsync(Exception exc)
        {
            return Task.FromResult(typeof(void));
        }

        private async Task<FileDocument> CreateDocumentAsync(IFormFile file)
        {
            var document = new FileDocument()
            {
                DocumentId = KeyGen.NewGuid(),
                FileName = System.IO.Path.GetFileName(file.FileName)
            };
            document.FileType = document.GetFileType();

            return await _documentService.CreateAsync(document);
        }

        private async Task DeleteDocumentAsync(FileDocument document)
        {
            await _documentService.DeleteAsync(document);
        }

        private async Task AddDocumentToFolderAsync(FileDocument document, IFolder folder)
        {
            await _folderManager.AddDocumentAsync(document, folder);
        }

        private async Task RemoveDocumentFromFolderAsync(FileDocument document, IFolder folder)
        {
            await _folderManager.RemoveDocumentAsync(document, folder);
        }

        private async Task<long> UploadDocumentAsync(FileDocument document, IFormFile file)
        {
            var url = $"{_driveOptions.Authority}/upload";
            using (var client = new HttpClient())
            {
                using (var fileStream = file.OpenReadStream())
                {
                    var content = new MultipartFormDataContent();
                    ByteArrayContent fileContent;
                    using (var br = new BinaryReader(fileStream))
                    {
                        fileContent = new ByteArrayContent(br.ReadBytes((int)fileStream.Length));
                    }

                    content.Add(fileContent, "file", file.FileName);
                    content.Add(new StringContent(document.DocumentId), "documentId");

                    await client.PostAsync(url, content);

                    return fileStream.Length;
                }
            }
        }

        private async Task UpdateFileSizeAsync(FileDocument document, long size)
        {
            document.ContentLength = size;
            document.Created = DateTime.Now;
            await _documentService.UpdateAsync(document);
        }
        #endregion // Private methods
    }
}


