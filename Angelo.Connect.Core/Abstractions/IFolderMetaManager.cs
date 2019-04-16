using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.Abstractions
{
    public interface IFolderMetaManager
    {
        Task<IEnumerable<string>> GetFolderTagsAsync(IFolder folder);

        Task AddFolderTagAsync(IFolder folder, string tag);
        Task RemoveFolderTagAsync(IFolder folder, string tag);
        Task SetFolderTagsAsync(IFolder folder, IEnumerable<string> tags);

        Task<IEnumerable<Category>> GetFolderCategoriesAsync(IFolder folder);

        Task AddFolderCategoryAsync(IFolder folder, Category category);
        Task RemoveFolderCategoryAsync(IFolder folder, Category category);
        Task SetFolderCategoriesAsync(IFolder folder, IEnumerable<Category> categories);
    }
}
