using Angelo.Connect.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public class NavMenuItemContent
    {
        public string Id { get; set; }
        public string ParentId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool HasChildren { get; set; }
        public string Link { get; set; }
    }

    public interface INavMenuContentProvider
    {
        string Name { get; }
        string Title { get; }
        IEnumerable<NavMenuItemContent> GetRootItems(string siteId);
        IEnumerable<NavMenuItemContent> GetChildItems(string siteId,  string parentContentId);
        NavMenuItemContent GetContentItem(string contentId);
        Task<bool> Authorize(string contentId);

        //string ToUrl(TreeNode item);
        //IContent FromUrl(string url);
    }

}

