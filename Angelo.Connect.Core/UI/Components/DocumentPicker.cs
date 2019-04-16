using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Menus;
using Angelo.Connect.Models;
using Angelo.Connect.Widgets;
using Angelo.Connect.Security;

namespace Angelo.Connect.UI.Components
{
    // TODO Which folder/NS does this go into?
    public struct DocumentPickerSettings
    {
        public string FileType { get; set; }
        public string SelectedFolderId { get; set; }
        public bool IsMultiSelect { get; set; }
        public bool IsTreeHidden { get; set; }
        public bool IsContentHidden { get; set; }
        public bool IsTreeRootHidden { get; set; }
        public bool AreContentFoldersHidden { get; set; }
        public bool AreContentDocumentsHidden { get; set; }
        public bool IsContentCrumbTrailHidden { get; set; }
    }

    // TODO: Allow it to be TDocument?
    // Allows a document to be selected via folder navigation
    public class DocumentPicker : ViewComponent
    {
        public const string ItemClickedEventName = "click";
        public const string ItemDoubleClickedEventName = "dblclick";
        // "Output" keys, so consumers know what to look for (*where* to look isn't determinable, but it's ViewData)
        public const string IdDelimiter = ",";
        public const string SelectedDocumentIdsViewDataKey = "SelectedDocIds";
        public const string SelectedFolderIdsViewDataKey = "SelectedFolderIds";

        private UserContext _userContext;
        private IFolderManager<FileDocument> _folderManager;

        public DocumentPicker(UserContext userContext, IFolderManager<FileDocument> folderManager)
        {
            _userContext = userContext;
            _folderManager = folderManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(DocumentPickerSettings settings,
                                                            string rootFolderId = null,
                                                            string selectedFolderId = null)
        {
            var userId = _userContext.Principal.GetUserId() ?? "AFCF7980-4BA7-4DD2-879D-599D058F7E73";// TODO: Remove this once the user starts showing up in the plugin's context

            // Pull the documents and child folders on the page so that I don't have to make a ViewModel to pass IFolder, ChildFolders, and Documents
            // TODO Maybe use a viewModel instead for purer MVVM?
            this.ViewData.Add("Settings", settings);
            if (!string.IsNullOrEmpty(selectedFolderId))
            {
                this.ViewData.Add("SelectedFolderId", selectedFolderId);
                //var selectedName = default(string);
                //switch (fileType)
                //{
                //    case FileType.Image:
                //        selectedName = "Images";
                //        break;
                //    case FileType.Audio:
                //        selectedName = "Music";
                //        break;
                //    case FileType.Document:
                //        selectedName = "Documents";
                //        break;
                //    case FileType.eBook:
                //        selectedName = "Books";
                //        break;
                //    case FileType.Presentation:
                //        break;
                //    case FileType.Video:
                //        selectedName = "Video";
                //        break;
                //}
            }

            return View(await GetFolderAsync(userId, rootFolderId));
        }

        private async Task<IFolder> GetFolderAsync(string userId, string folderId)
        {
            var result = default(IFolder);
            if (string.IsNullOrEmpty(folderId))
            {
                result = await _folderManager.GetRootFolderAsync(userId);
                var images = (await _folderManager.GetFoldersAsync(result)).SingleOrDefault(x => string.Equals("images", x.Title));
                result = images ?? result;
            }
            else
            {
                result = await _folderManager.GetFolderAsync(folderId);
            }

            return result;
        }
    }
}
