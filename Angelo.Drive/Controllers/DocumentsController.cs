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
    public class DocumentsController:ControllerBase
    {
        private LibraryManager _libraryManager;
        private IDocumentService<FileDocument> _documentService;    // TODO Removve this dependency

        public DocumentsController(LibraryManager libraryManager, IDocumentService<FileDocument> documentService)
        {
            _libraryManager = libraryManager;
            _documentService = documentService;
        }

        [ResponseCache(CacheProfileName = "Default")]
        [HttpGet]
        [Route("")]
        [Route("{id}")]
        public async Task<IActionResult> Documents(string id)
        {
            
            id = Path.GetFileNameWithoutExtension(id);
            var userId = GetCurrentUserId();
            var document = await _documentService.GetAsync(id);

            var documentStream = document.FileType == FileType.Image ?
                    await _libraryManager.GetLowResolutionStream(id) :
                    await _libraryManager.DownloadDocumentAsync(id, userId);
            if (documentStream == null) throw new InvalidOperationException($"Physical file missing for document '{id}'  or bad id.");

            byte[] documentByteArray;
            using (MemoryStream ms = new MemoryStream())
            {
                documentStream.CopyTo(ms);
                documentByteArray = ms.ToArray();
            }

           // TODO Figure out how to removet his dependency

           var mimeType = document?.GetMimeType() ?? "application/octet";
            var fileName = document.FileName;

            // WJC: Need this header to allow the browser to download from the Drive domain.
            HttpContext.Response.Headers.Append("Access-Control-Allow-Origin", "*");
            HttpContext.Response.Headers.Append("Content-Disposition", "inline; filename=" + fileName);
            var responseHeaders = HttpContext.Response.GetTypedHeaders();
            responseHeaders.LastModified = document.Created;
            responseHeaders.Date = document.Created;

            return new RangeFileContentResult(
                fileContents: documentByteArray,
                contentType: mimeType
                //fileDownloadName: fileName   //By setting a file download name the framework will automatically add the attachment Content - Disposition header
            );
        }

        private string GetCurrentUserId()
        {
            return Guid.Empty.ToString();// TODO Fix this via plugin security User.GetUserId();
        }

    }
}
