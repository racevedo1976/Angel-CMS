using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using AutoMapper.Extensions;

using Angelo.Connect.Services;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Angelo.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Angelo.Connect.Models;
using System.Net.Http;
using System.IO;
using Angelo.Connect.Extensions;
using Angelo.Connect.Abstractions;
using Microsoft.Extensions.Options;
using Angelo.Connect.Services;
using Angelo.Connect.Logging;
using Angelo.Connect.UI.Components;

namespace Angelo.Connect.UI.Controllers
{
    public class DocumentPickerController : Controller
    {
        #region Dependencies
        private IFolderManager<FileDocument> _folderManager;
        private IDocumentService<FileDocument> _documentService;
        private TagManager _tagManager;
        private DbLoggerProvider _log;
        private DbLogService _logFetcher;

        private ILogger<DocumentPickerController> _logger;

        private DriveOptions _driveOptions;
        #endregion // Dependencies
        #region Constructors
        public DocumentPickerController(
            IFolderManager<FileDocument> folderManager,
            IDocumentService<FileDocument> documentService,
            IOptions<DriveOptions> driveOptions,
            TagManager tagManager,
            DbLoggerProvider log,
            DbLogService logFetcher,
            ILogger<DocumentPickerController> logger) : base()
        {
            _folderManager = folderManager;
            _documentService = documentService;
            _driveOptions = driveOptions.Value;
            _tagManager = tagManager;
            _log = log;
            _logFetcher = logFetcher;
            _logger = logger;
        }
        #endregion // Constructors
        #region Public actions
        #region Folder API
        //[Authorize]
        [HttpGet, Route("/api/docpicker/folders")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        //[Authorize]
        public async Task<IEnumerable<IFolder>> GetFolders(string userId, string id = null)
        {
            return await _folderManager.GetFoldersAsync(string.IsNullOrEmpty(id)
                ? await _folderManager.GetRootFolderAsync(userId)
                : await _folderManager.GetFolderAsync(id));
        }

        [Authorize]
        [HttpGet, Route("/api/docpicker/folderTree")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IEnumerable<Folder>> GetFolderTree(string userId, string id = null, bool ignoreTrash = true)
        {
            return await _folderManager.GetFoldersRecursivelyAsync(string.IsNullOrEmpty(id)
                    ? await _folderManager.GetRootFolderAsync(userId)
                    : await _folderManager.GetFolderAsync(id),
                ignoreTrash);
        }

        //[Authorize]
        [HttpGet, Route("/api/docpicker/folderTreePath")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IEnumerable<IFolder>> GetFolderTreePath(string id)
        {
            return await _folderManager.GetFoldersPathAsync(await _folderManager.GetFolderAsync(id));
        }

        [HttpGet, Route("/api/docpicker/getTrashFolder")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IFolder> GetTrashFolder(string userId)
        {
            return await _folderManager.GetTrashFolderAsync(userId);
        }

        [Authorize]
        [HttpGet]
        [Route("/api/docpicker/folder")]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<IFolder> GetFolder(string userId, string id = null)
        {
            return string.IsNullOrEmpty(id)
                ? await _folderManager.GetRootFolderAsync(userId)
                : await _folderManager.GetFolderAsync(id);
        }

        [Authorize]
        [HttpPost, Route("/api/docpicker/folder/update")]
        public async Task UpdateDocument([FromBody] IFolder folder)
        {
            await _folderManager.UpdateFolderAsync(folder);
        }

        [Authorize]
        [HttpPost, Route("/api/docpicker/folder")]
        public async Task<IFolder> AddFolder(string name, string userId, string parentId = null)
        {
            return await _folderManager.AddFolderAsync(name, userId, parentId);
        }

        [Authorize]
        [HttpDelete, Route("/api/docpicker/folder")]
        public async Task RemoveFolder(string id)
        {
            await _folderManager.RemoveFolderAsync(await _folderManager.GetFolderAsync(id));
        }
        #endregion // Folder API
        #region Document API
        [Authorize]
        [HttpGet, Route("/api/docpicker/doc")]
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

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        [Route("/api/docpicker/docs")]
        public async Task<IEnumerable<FileDocument>> GetDocuments(string folderId)
        {
            var folder = await _folderManager.GetFolderAsync(folderId);
            var results = await _folderManager.GetDocumentsAsync(folder);

            await Task.WhenAll(results.Select(doc => _log.LogEventReadAsync(doc, folderId)));

            return results;
        }


        [HttpGet]
        [Authorize]
        [Route("/api/docpicker/docs/{userId}")]
        public async Task<IList<FileDocument>> ListDocuments(string userId)
        {
            Ensure.NotNullOrEmpty(userId, $"{nameof(userId)} cannot be null.");

            var folders = await _folderManager.GetFoldersAsync(userId);
            var results = folders.SelectMany(x => _folderManager.GetDocumentsAsync(x).GetAwaiter().GetResult())
                .ToList();

            await Task.WhenAll(results.Select(doc => _log.LogEventReadAsync(doc, userId)));

            return results;
        }

        [HttpGet, Route("/api/docpicker/downloadSize")]
        //[Authorize]
        public async Task<long> GetDocumentContentLength(string id)
        {
            var userId = GetCurrentUserId();
            var document = await _documentService.GetAsync(id);
            if (document == null) throw new ArgumentException($"Document not found: {id}");

            return document.ContentLength;
        }
        #endregion Document API
        #region View API
        //[HttpGet, Route("/docpicker/crumbs")]
        //public async Task<IActionResult> GetCrumbTrailAsync(string folderId)
        //{
        //    return PartialView("~/UI/Views/Components/DocumentPicker/_Crumbs.cshtml", await _folderManager.GetFolderAsync(folderId));
        //}
        [Route("/docpicker/content")]
        public async Task<IActionResult> GetFolderContent(string folderId)
        {
            var form = this.Request.Form;
            this.ViewData.Add("Settings", new DocumentPickerSettings() {
                AreContentDocumentsHidden = bool.Parse(form[$"settings[{nameof(DocumentPickerSettings.AreContentDocumentsHidden)}]"].FirstOrDefault()),
                AreContentFoldersHidden = bool.Parse(form[$"settings[{nameof(DocumentPickerSettings.AreContentFoldersHidden)}]"].FirstOrDefault()),
                FileType = form[$"settings[{nameof(DocumentPickerSettings.FileType)}]"].FirstOrDefault(),
                IsContentCrumbTrailHidden = bool.Parse(form[$"settings[{nameof(DocumentPickerSettings.IsContentCrumbTrailHidden)}]"].FirstOrDefault()),
                IsContentHidden = bool.Parse(form[$"settings[{nameof(DocumentPickerSettings.IsContentHidden)}]"].FirstOrDefault()),
                IsMultiSelect = bool.Parse(form[$"settings[{nameof(DocumentPickerSettings.IsMultiSelect)}]"].FirstOrDefault()),
                IsTreeHidden = bool.Parse(form[$"settings[{nameof(DocumentPickerSettings.IsTreeHidden)}]"].FirstOrDefault()),
                IsTreeRootHidden = bool.Parse(form[$"settings[{nameof(DocumentPickerSettings.IsTreeRootHidden)}]"].FirstOrDefault()),
                SelectedFolderId = form[$"settings[{nameof(DocumentPickerSettings.SelectedFolderId)}]"].FirstOrDefault()
            });
            return PartialView("~/UI/Views/Components/DocumentPicker/_Content.cshtml", await _folderManager.GetFolderAsync(folderId));
        }
        #endregion // View API
        // Moving the atomic Drive.AddDocument here, since Connect now livese here, the doc is created here
        // and then Drive is called to upload the document, followed by an update to the file size
        [Authorize]
        [HttpPost, Route("/api/docpicker/doc")]
        public async Task CreateAndUploadDocument(string folderId, IFormFile file = null)
        {
            foreach (var f in (file == null ? Request.Form.Files.Cast<IFormFile>() : new[] { file }))
            {
                var document = await CreateDocumentAsync(f);
                var folder = await _folderManager.GetFolderAsync(folderId);
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
            await _documentService.UpdateAsync(document);
        }
        #endregion // Private methods
    }
}


