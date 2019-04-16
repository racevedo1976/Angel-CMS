using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Threading.Tasks;

namespace Angelo.Connect.Extensions
{
    public static class FolderExtensions
    {
        public static Folder ToFolder(this IFolder folder)
        {
            Ensure.NotNull(folder);

            var result = folder as Folder;

            if (result == null)
            {
                result = new Folder()
                {
                    Id = folder.Id,
                    Title = folder.Title,
                    ParentId = folder.ParentId,
                    DocumentType = folder.DocumentType,
                    FolderType = folder.FolderType,
                    OwnerLevel = folder.OwnerLevel,
                    FolderFlags = folder.FolderFlags,
                    OwnerId = folder.OwnerId
                };
            }

            return result;
        }

        //public static bool IsSystemFolder(this Folder folder)
        //{
        //    Ensure.NotNull(folder, $"{folder} cannot be null.");

        //    return folder.IsTrashFolder();
        //}

        //public static bool IsRootFolder(this IFolder folder)
        //{
        //    return folder != null && folder.P
        //}

        //public static bool IsTrashFolder(this Folder folder)
        //{
        //    Ensure.NotNull(folder, $"{folder} cannot be null.");

        //    return folder.ParentFolder.IsRoot()
        //        && string.Equals(folder.GetTrashFolderName(), folder.Name, StringComparison.CurrentCultureIgnoreCase);
        //}

        //public static string GetTrashFolderName(this Folder folder)
        //{
        //    return "Trash";// GetString(Folder.TrashFolderResourceKey);
        //}

        //public static string GetAbsolutePath(this Folder folder, string rootPath)
        //{
        //    Ensure.NotNull(folder, $"{folder} can not be null.");
        //    Ensure.NotNullOrEmpty(rootPath, $"{rootPath} can not be null or empty.");

        //    var result = rootPath;
        //    var parentPath = folder.ParentFolder?.Path;
        //    if (!string.IsNullOrEmpty(parentPath))
        //    {
        //        result += "/" + parentPath;
        //    }

        //    return result;
        //}
        private static ResourceManager ResourceManager { get; } = CreateResourceManager();

        private static ResourceManager CreateResourceManager()
        {
            var result = new ResourceManager("Angelo.Drive.Resources.Strings", typeof(Folder).GetTypeInfo().Assembly);

            return result;
        }

        private static string GetString(string key)
        {
            var result = key;
            try
            {
                result = ResourceManager.GetString(key);
            }
            catch { }
            return result;
        }
    }
}
