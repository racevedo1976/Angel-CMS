using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.ViewModels.Extensions
{
    public static class DriveViewModelExtensions
    {
        public static FolderViewModel ToFolderViewModel(this IFolder folder)
        {
            string path = null;
            string parentFolder = null;
            ICollection<DocumentViewModel> documents = null;
            ICollection<FolderViewModel> folders = null;

            var folderInstance = folder as Folder;
            if (folderInstance is Folder)
            {
                //path = ???;
                parentFolder = folderInstance.ParentFolder?.Title;
                //documents = ???;
                folders = folderInstance.ChildFolders?.Select(x => ToFolderViewModel(x)).ToArray();
            }

            return new FolderViewModel()
            {
                Id = folder.Id,
                Title = string.IsNullOrEmpty(folder.Title) ? "My Root" : folder.Title,
                OwnerUserId = folder.OwnerId,
                ParentFolderId = folder.ParentId,

                Path = path,
                ParentFolder = parentFolder,
                Documents = documents,
                ChildFolders = folders
            };
        }

        public static FolderViewModel ToFolderViewModel(this FileDocument document)
        {
            return new FolderViewModel()
            {
                Id = document.DocumentId,
                Title = document.Title
            };
        }

        public static DocumentViewModel ToDocumentViewModel(this FileDocument document)
        {
            return new DocumentViewModel()
            {
                ContentLength = document.ContentLength,
                DocumentId = document.DocumentId,
                FileExtension = document.FileExtension,
                FileName = document.FileName,
                FileSize = document.FileSize,
                FileType = document.FileType,
                Title = document.Title,
                Description = document.Description,
                CreatedDate = document.CreatedDateString
            };
        }

        public static IList<DocumentViewModel> ToDocumentViewModel(this IEnumerable<FileDocument> documents)
        {

            return documents.Select(x => x.ToDocumentViewModel()).ToList();
        }

        public static IList<FileExplorerViewModel> ToFileExplorerViewModel(this IEnumerable<IContentType> documents)
        {

            return documents.Select(x => x.ToFileExplorerViewModel()).ToList();
        }

        public static FileExplorerViewModel ToFileExplorerViewModel(this IContentType item)
        {
            if (item is Folder)
            {
                var folder = item as Folder;

                return new FileExplorerViewModel()
                {
                    Id = folder.Id,
                    Name = folder.Title,
                    ObjectType = typeof(Folder).Name,
                    Size = "",
                    ParentFolderId = folder.ParentId,
                    FileType = ""
                   
                };
            }
            else
            {
                var document = item as FileDocument;

                return new FileExplorerViewModel()
                {
                    Size = document.ContentLength.ToString(),
                    Id = document.DocumentId,
                    ObjectType = typeof(FileDocument).Name,
                    Name = document.FileName,
                    FileType = document.FileType,
                    Extension = document.FileExtension,
                    CreatedDateString = document.Created.ToString("O")
                };
            }
           
        }
    }
}
