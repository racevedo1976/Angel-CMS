using Angelo.Connect.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface IContentBrowser
    {
        Task<bool> HasAccessToLibrary();
        Task<bool> CanManageLibrary();
        string GetLibraryRootName();
        string ContentComponentView { get;  }
        string GetComponentContentView();
        Task<TreeView> GetRootTreeView(string userId);
        TreeView GetSharedContentTreeNode();
        Task<bool> IsAnythingShared(string userId);
        Task<IEnumerable<IContentType>> GetContent(string id);
        Task<IEnumerable<IContentType>> GetSharedContent(string userId);
    }
}
