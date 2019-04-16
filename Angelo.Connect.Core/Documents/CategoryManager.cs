using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security;

namespace Angelo.Connect.Services
{
    public class CategoryManager
    {
        private ConnectDbContext _db;

        public CategoryManager(ConnectDbContext db)
        {
            _db = db;
        }

        public async Task<string> CreateCategoryAsync(Category category)
        {
            Ensure.That(category != null);

            try
            {
                category.Id = KeyGen.NewGuid();
                _db.Categories.Add(category);

                await _db.SaveChangesAsync();
                return category.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<IEnumerable<Category>> GetGlobalCategoriesAsync()
        {
            return await _db.Categories
                .Where(x => x.OwnerLevel == OwnerLevel.Global)
                .ToListAsync();
        }

        public async Task<Category> GetClientCategoryByIdAsync(string categoryId)
        {
            return await _db.Categories
                .Where(x => x.Id == categoryId)
                .FirstOrDefaultAsync();
        }

        public async Task<Category> GetSiteCategoryByIdAsync(string categoryId)
        {
            return await _db.Categories
                .Where(x => x.Id == categoryId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Category>> GetClientCategoriesAsync()
        {
            return await _db.Categories
                .Where(x => x.OwnerLevel == OwnerLevel.Client || x.OwnerLevel == OwnerLevel.Global)
                .OrderBy(x => x.Title)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetClientCategoriesAsync(bool inherit)
        {
            if (inherit)
            {
                return await _db.Categories
                .Where(x => 
                    x.OwnerLevel == OwnerLevel.Global
                    || (x.OwnerLevel == OwnerLevel.Client)
                )
                .ToListAsync();
            }

            return await GetClientCategoriesAsync();
        }

        public async Task<IEnumerable<Category>> GetSiteCategoriesAsync(string siteId)
        {
            return await _db.Categories
                .Where(x => x.OwnerLevel == OwnerLevel.Global || x.OwnerLevel == OwnerLevel.Client || x.OwnerLevel == OwnerLevel.Site && x.OwnerId == siteId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Category>> GetSiteCategoriesAsync(string siteId, bool inherit)
        {
            if (inherit)
            {
                var site = await _db.Sites.FirstOrDefaultAsync(x => x.Id == siteId);
                var clientId = site?.ClientId;

                return await _db.Categories
                    .Where(x => 
                        x.OwnerLevel == OwnerLevel.Global
                        || (x.OwnerLevel == OwnerLevel.Client && x.OwnerId == clientId)
                        || (x.OwnerLevel == OwnerLevel.Site && x.OwnerId == siteId)
                    )
                    .ToListAsync();
            }

            return await GetSiteCategoriesAsync(siteId);
        }

        public async Task<bool> DeleteCategoryAsync(string categoryId)
        {
            Ensure.Argument.NotNull(categoryId);

            var category = await _db.Categories.Where(x => x.Id == categoryId).FirstOrDefaultAsync();

            try
            {
                _db.Categories.Remove(category);
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateCategoryAsync(Category model)
        {
            Ensure.Argument.NotNull(model);

            var tempModel = await _db.Categories.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (tempModel == null)
            {
                return false;
            }

            tempModel.Title = model.Title;

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<IEnumerable<Category>> GetFilterMenuTypesAsync()
        {
            var model = new List<Category>();
            return model;
        }
    }
}
