using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.UI.ViewModels;
using Angelo.Connect.UserConsole;
using Angelo.Connect.Web.Data.Mock;

namespace Angelo.Connect.Web.UI.UserConsole.Components
{
    public class UserLibraryComponent : IUserConsoleTreeComponent
    {
        public string ComponentType { get; } = "library";

        public int ComponentOrder { get; } = 1;

        public string TreeTitle { get; } = "My Library";
       
        public UserLibraryComponent()
        {
        }

        public async Task<IEnumerable<GenericTreeNode>> GetRootNodes()
        {
            return await GetFoldersAsTreeNodes(null);
        }

        public async Task<IEnumerable<GenericTreeNode>> GetChildNodes(string nodeId, string nodeType)
        {
            return await GetFoldersAsTreeNodes(nodeId);
        }

        public async Task<IEnumerable<GenericMenuItem>> GetTreeMenu()
        {
            var menu = new GenericMenuItem[]
           {
                new GenericMenuItem { Name = "createFolder", Title = "Create Folder", IconCss = "fa fa-plus" },
                new GenericMenuItem { Name = "downloadAll", Title = "Download Library", IconCss = "fa fa-download" },
                new GenericMenuItem { Name = "googleDrive", Title = "Link Google Drive", IconCss = "fa fa-link" },
           };

            return await Task.FromResult(menu);
        }

        private async Task<IEnumerable<GenericTreeNode>> GetFoldersAsTreeNodes(string parentId)
        {
            var nodes = MockData.Folders
               .Where(x => x.ParentId == parentId)
               .Select(x => new GenericTreeNode
               {
                   Id = x.Id,
                   Title = x.Name,
                   IconCss = "fa fa-folder",
                   LinkUrl = "/sys/uc/library/" + x.Id,
                   HasChildren = MockData.Folders.Any(y => y.ParentId == x.Id)
               });

            return await Task.FromResult(nodes);
        }   
    }
}
