using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Connect.Abstractions;
using Angelo.Common.Extensions;
using Angelo.Common.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Models;

namespace Angelo.Connect.Services
{
    public class FolderMetaManager : IFolderMetaManager
    {

        private ConnectDbContext _db;

        public FolderMetaManager(ConnectDbContext db)
        {
            Ensure.NotNull(db, $"{nameof(db)} cannot be null.");
            _db = db;
        }

        // Tags
        public async Task<IEnumerable<string>> GetFolderTagsAsync(IFolder folder)
        {
            var result = await _db.Folders
                .Include(x => x.Tags)
                .FirstOrDefaultAsync(x => x.Id == folder.Id);

            return result?.Tags?.Select(x => x.TagName);
        }

        public async Task AddFolderTagAsync(IFolder folder, string tag)
        {
            var exists = await HasTagAsync(folder, tag);

            // TODO: Consider throwing an error vs. not adding
            if (!exists)
            {
                var entity = BuildFolderTag(folder, tag);

                _db.FolderTags.Add(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveFolderTagAsync(IFolder folder, string tag)
        {
            var entity = await _db.FolderTags.FirstOrDefaultAsync(x =>
                x.FolderId == folder.Id
                && x.TagName == tag
            );

            // TODO: Consider throwing an error vs. not adding
            if (entity != null)
            {
                _db.FolderTags.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task SetFolderTagsAsync(IFolder folder, IEnumerable<string> tags)
        {
            _db.FolderTags.RemoveRange(
                await _db.FolderTags.Where(x => x.FolderId == folder.Id).ToListAsync()
            );

            _db.FolderTags.AddRange(
                tags.Select(tag => BuildFolderTag(folder, tag))
            );

            await _db.SaveChangesAsync();
        }


        // Categories
        public async Task<IEnumerable<Category>> GetFolderCategoriesAsync(IFolder folder)
        {
            var result = await _db.Folders
                .Include(x => x.CategoryMap)
                .ThenInclude(x => x.Category)
                .FirstOrDefaultAsync(x => x.Id == folder.Id);

            return result?.CategoryMap?.Select(x => x.Category);
        }

        public async Task AddFolderCategoryAsync(IFolder folder, Category category)
        {
            var exists = await HasCategoryAsync(folder, category);

            // TODO: Consider throwing an error vs. not adding
            if (!exists)
            {
                var entity = BuildFolderCategory(folder, category);

                _db.FolderCategories.Add(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task RemoveFolderCategoryAsync(IFolder folder, Category category)
        {
            var entity = await _db.FolderCategories.FirstOrDefaultAsync(x =>
                x.FolderId == folder.Id
                && x.CategoryId == category.Id
            );

            // TODO: Consider throwing an error vs. not adding
            if (entity != null)
            {
                _db.FolderCategories.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task SetFolderCategoriesAsync(IFolder folder, IEnumerable<Category> categories)
        {
            _db.FolderCategories.RemoveRange(
                await _db.FolderCategories.Where(x => x.FolderId == folder.Id).ToListAsync()
            );

            _db.FolderCategories.AddRange(
                categories.Select(cat => BuildFolderCategory(folder, cat))
            );

            await _db.SaveChangesAsync();
        }


        // Private
        private async Task<bool> HasTagAsync(IFolder folder, string tag)
        {
            return await _db.FolderTags.AnyAsync(x =>
                x.FolderId == folder.Id
                && x.TagName == tag
            );
        }

        private async Task<bool> HasCategoryAsync(IFolder folder, Category category)
        {
            return await _db.FolderCategories.AnyAsync(x =>
                x.FolderId == folder.Id
                && x.CategoryId == category.Id
            );
        }

        private FolderCategory BuildFolderCategory(IFolder folder, Category category)
        {
            return new FolderCategory
            {
                Id = KeyGen.NewGuid(),
                FolderId = folder.Id,
                CategoryId = category.Id
            };
        }

        private FolderTag BuildFolderTag(IFolder folder, string tag)
        {
            return new FolderTag
            {
                Id = KeyGen.NewGuid(),
                FolderId = folder.Id,
                TagName = tag              
            };
        }
    }

   
 
}
