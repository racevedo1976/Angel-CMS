using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Services
{
    public class NavMenuTestProvider : INavMenuContentProvider
    {
        private ConnectDbContext _connectDb;

        public string Name
        {
            get
            {
                return "TestProvider";
            }
        }

        public string Title
        {
            get
            {
                return "Test Provider";
            }
        }

        public NavMenuTestProvider(ConnectDbContext connectDbContext)
        {
            _connectDb = connectDbContext;
        }

        private IEnumerable<NavMenuItemContent> GetNavItems(string parentId)
        {
            var items = new List<NavMenuItemContent>();
            for (var x = 0; x < 30; x++)
            {
                items.Add(new NavMenuItemContent()
                {
                    Id = parentId + "-" + x.ToString(),
                    ParentId = parentId,
                    Title = "ITEM-" + parentId + "-" + x.ToString(),
                    Description = "Item-" + parentId + "-" + x.ToString(),
                    HasChildren = true,
                    Link = "ITEM-" + parentId + "-" + x.ToString()
                });
            }
            return items;
        }

        public IEnumerable<NavMenuItemContent> GetRootItems(string siteId)
        {
            return GetNavItems(string.Empty);
        }

        public IEnumerable<NavMenuItemContent> GetChildItems(string siteId, string parentContentId)
        {
            return GetNavItems(parentContentId);
        }

        public NavMenuItemContent GetContentItem(string contentId)
        {
            var item = new NavMenuItemContent();
            item.ParentId = string.Empty;
            item.Title = "CItem-" + contentId;
            item.Description = item.Title;
            item.HasChildren = true;
            item.Link = item.Title;
            return item;
        }

        public async Task<bool> Authorize(string contentId)
        {
            return await Task.FromResult(true);
        }


    }
}


