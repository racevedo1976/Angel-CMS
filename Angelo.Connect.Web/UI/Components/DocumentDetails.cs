using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Angelo.Connect.Web.UI.ViewModels.Extensions;
using Microsoft.Extensions.Options;
using Angelo.Identity;
using Angelo.Connect.Logging;

namespace Angelo.Connect.Web.UI.Components
{
    public class DocumentDetails : ViewComponent
    {
        private IFolderManager<FileDocument> _folderManager;
        private IDocumentService<FileDocument> _documentService;
        private DriveOptions _driveOptions;
        private UserManager _userManager;
        private DbLoggerProvider _log;

        public DocumentDetails(IFolderManager<FileDocument> folderManager, 
            IDocumentService<FileDocument> documentService, 
            IOptions<DriveOptions> driveOptions, 
            UserManager userManager,
            DbLoggerProvider log)
        {
            Ensure.NotNull(folderManager);
            Ensure.NotNull(documentService);
            Ensure.NotNull(driveOptions);

            this._folderManager = folderManager;
            this._documentService = documentService;
            this._driveOptions = driveOptions.Value;
            this._log = log;
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string documentId, string folderId, string userId, bool isShared)
        {
            DocumentViewModel documentObject = null;
            FolderViewModel folderObject = null;
            string viewName = "FolderDetails";  //default view will render documents only
            var userDetails = (await _userManager.GetUserAsync(userId));

            ViewBag.DriveUrl = _driveOptions.Authority;
            ViewBag.Directory = userDetails.DirectoryId;
            ViewBag.DisplayName = userDetails.DisplayName;

            folderObject = (await _folderManager.GetFolderAsync(folderId)).ToFolderViewModel();

            ViewBag.Location = folderObject.Title;
            ViewBag.IsShared = isShared;

            if (isShared)
            {
                ViewBag.DisplayName = (await _userManager.GetUserAsync(folderObject.OwnerUserId)).DisplayName;
                ViewBag.Location = "Shared";
            }

            //Get logs
           

            if (!string.IsNullOrEmpty(documentId))
            {
                viewName = "default";
                documentObject = (await _documentService.GetAsync(documentId)).ToDocumentViewModel();

                return await Task.Run(() => View(viewName, documentObject));
            }
            else
            {
                return await Task.Run(() => View(viewName, folderObject));
            }
            
        }
    }
}
