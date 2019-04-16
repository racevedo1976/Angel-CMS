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
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Route("api/[controller]")]

    public class UploadController : ControllerBase
    {
        private LibraryManager _libraryManager;

        public UploadController(LibraryManager libraryManager)
        {
            _libraryManager = libraryManager;
        }

        // NOTE: IFormFile is the .Net Core WebAPI-level file format, as foud in Request.Form.Files
        [HttpPost]
        public async Task Upload(string documentId, IFormFile file = null)
        {
            foreach (var f in (file == null ? Request.Form.Files.Cast<IFormFile>() : new[] { file }))
            {
                using (var stream = f.OpenReadStream()) // Transform the IFormFile (WebAPI) into Stream (universal)
                {
                    await _libraryManager.UploadDocumentAsync(documentId, User.GetUserId(), stream);
                }
            }
        }
    }
}
