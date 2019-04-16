using Angelo.Connect.Abstractions;
using Angelo.Connect.Extensions;
using Angelo.Connect.Models;
using Angelo.Drive.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Angelo.Drive.Controllers
{
    [ResponseCache(CacheProfileName = "Default")]
    [Route("api/[controller]")]

    public class DownloadController : ControllerBase
    {
        private LibraryManager _libraryManager;
        private IDocumentService<FileDocument> _documentService;    // TODO Removve this dependency

        public DownloadController(LibraryManager libraryManager, IDocumentService<FileDocument> documentService)
        {
            _libraryManager = libraryManager;
            _documentService = documentService;
        }

        [HttpGet("{id}"), Route("")]
        public async Task<IActionResult> Download(string id)
        {
            id = Path.GetFileNameWithoutExtension(id);
            var userId = GetCurrentUserId();
            var documentStream = await _libraryManager.DownloadDocumentAsync(id, userId);
            if (documentStream == null) throw new InvalidOperationException($"Physical file missing for document '{id}' or bad id.");

            byte[] documentByteArray;
            using (MemoryStream ms = new MemoryStream())
            {
                documentStream.CopyTo(ms);
                documentByteArray = ms.ToArray();
            }
            
            // WJC: Need this header to allow the browser to download from the Drive domain.
            HttpContext.Response.Headers.Append("Access-Control-Allow-Origin", "*");

            // TODO Figure out how to removet his dependency
            var document = await _documentService.GetAsync(id);
            var mimeType = document?.GetMimeType() ?? "application/octet";
            var fileName = document.FileName;

            return new RangeFileContentResult(
                    fileContents: documentByteArray,
                    contentType: mimeType, 
                    fileDownloadName: fileName   //By setting a file download name the framework will automatically add the attachment Content - Disposition header
                );

        }

        [HttpGet, Route("zipsync")]
        //[Authorize]
        public async Task<FileStreamResult> DownloadItems(string ownerId, string[] folderIds = null, string[] documentIds = null)
        {
            return new FileStreamResult(
                await _libraryManager.DownloadItemsAsync(ownerId, folderIds, documentIds),
                "application/zip");
        }

        [HttpGet, Route("zipasync")]
        //[Authorize]
        public async Task<FileDocument> DownloadItemsAsync(string ownerId, string[] folderIds, string[] documentIds, string targetFolderId = null)
        {
            return await _libraryManager.DownloadItemsAsJobAsync(ownerId, folderIds, documentIds, targetFolderId);
        }

        //[HttpGet]
        //public async Task<IActionResult> DownloadZip(string tempZipFileName)
        //{
        //assumptions:
        // Zip File will be created before this action is called.
        // A temp name will be assigned and created in a temp location

        //step 1
        //get stream from the temp zip file created using the temp name. 
        //assuming all zip files will be created in a temp location and then removed


        //step 2 Convert Stream to Byte Array. (sample code below)
        //byte[] zipFileByteArray;
        //using (MemoryStream ms = new MemoryStream())
        //{
        //    zipFileStream.CopyTo(ms);
        //    zipFileByteArray = ms.ToArray();
        //}


        //return this.File(
        //        fileContents: zipFileByteArray,
        //        contentType: "application/x-zip-compressed",
        //        // By setting a file download name the framework will
        //        // automatically add the attachment Content-Disposition header
        //        fileDownloadName: tempZipFileName + ".zip"
        //    );


        //}

        [ResponseCache(CacheProfileName = "Default")]
        [HttpGet, Route("thumbnail/{id}")]
        [Route("thumbnail")]
        public async Task<IActionResult> GetThumbnail(string id)
        {
            id = Path.GetFileNameWithoutExtension(id);
            var userId = GetCurrentUserId();
            var document = await _documentService.GetAsync(id);
            var stream = await _libraryManager.GetThumbnailStream(id);
            stream.Position = 0;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return this.File(
                       fileContents: ms.ToArray(),
                       contentType: document.GetMimeType(),
                                                 // By setting a file download name the framework will
                                                 // automatically add the attachment Content-Disposition header
                       fileDownloadName: document.FileName//documentModel.FileName
                   );
            }
        }

        [ResponseCache(CacheProfileName = "Default")]
        [HttpGet, Route("lowresimage")]
        public async Task<IActionResult> GetLowResolutionImage(string id)
        {
            id = Path.GetFileNameWithoutExtension(id);
            var userId = GetCurrentUserId();
            var document = await _documentService.GetAsync(id);
            var stream = await _libraryManager.GetLowResolutionStream(id);
            stream.Position = 0;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return this.File(
                       fileContents: ms.ToArray(),
                       contentType: document.GetMimeType(),
                                                 // By setting a file download name the framework will
                                                 // automatically add the attachment Content-Disposition header
                       fileDownloadName: document.FileName //documentModel.FileName
                   );
            }
        }


        [HttpGet, Route("image")]
        public async Task<IActionResult> GetImage(string id, int x, int y)
        {
            var userId = GetCurrentUserId();
            var document = await _documentService.GetAsync(id);
            var stream = await _libraryManager.GetResizedImageStream(id, x, y);
            stream.Position = 0;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return this.File(
                       fileContents: ms.ToArray(),
                       contentType: document.GetMimeType(),
                                                 // By setting a file download name the framework will
                                                 // automatically add the attachment Content-Disposition header
                       fileDownloadName: document.FileName
                   );
            }
        }

        private string GetCurrentUserId()
        {
            return Guid.Empty.ToString();// TODO Fix this via plugin security User.GetUserId();
        }

        [HttpGet, Route("crop")]
        public async Task<IActionResult> DownloadCroppedImage(string id, int x, int y, int width, int height)
        {
            id = Path.GetFileNameWithoutExtension(id);
            var userId = GetCurrentUserId();
            var document = await _documentService.GetAsync(id);
            var stream = await _libraryManager.GetCroppedImageStream(id, x, y, width, height);
            stream.Position = 0;

            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);

                return this.File(
                       fileContents: ms.ToArray(),
                       contentType: document.GetMimeType(),
                                                 // By setting a file download name the framework will
                                                 // automatically add the attachment Content-Disposition header
                       fileDownloadName: document.FileName
                   );
            }
        }
    }
}
