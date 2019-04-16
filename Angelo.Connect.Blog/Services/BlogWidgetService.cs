using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


using Angelo.Connect.Blog.Data;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Widgets;
using Angelo.Connect.Services;
using AutoMapper.Extensions;

namespace Angelo.Connect.Blog.Services
{
    public class BlogWidgetService : IWidgetService<BlogWidget>
    {
        private BlogDbContext _blogDbContext;
        private ConnectDbContext _connectDbContext;

        public BlogWidgetService(BlogDbContext blogDbContext, ConnectDbContext connectDbContext)
        {

            _blogDbContext = blogDbContext;
            _connectDbContext = connectDbContext;
        }

        public void SaveModel(BlogWidget model)
        {
            _blogDbContext.BlogWidgets.Add(model);
            _blogDbContext.SaveChanges();
        }

        public void UpdateModel(BlogWidget model)
        {
            _blogDbContext.Attach(model);
            _blogDbContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _blogDbContext.SaveChanges();
        }

        public BlogWidget CloneModel(BlogWidget model)
        {
            // clone the base widget
            var cloned = new BlogWidget
            {
                Id = KeyGen.NewGuid(),
                Title = model.Title,
                PageSize = model.PageSize,
                BlogId = model.BlogId,
                CreateBlog = model.CreateBlog
            };


            // clone the categories
            var categories = _blogDbContext.BlogWidgetCategories.AsNoTracking().Where(x => x.WidgetId == model.Id);

            cloned.Categories = categories.Select(x => new BlogWidgetCategory
            {
                Id = Guid.NewGuid().ToString("N"),
                CategoryId = x.CategoryId,
                WidgetId = cloned.Id
            }).ToList();


            // clone the tags
            var tags = _blogDbContext.BlogWidgetTags.AsNoTracking().Where(x => x.WidgetId == model.Id);

            cloned.Tags = tags.Select(x => new BlogWidgetTag
            {
                Id = Guid.NewGuid().ToString("N"),
                TagId = x.TagId,
                WidgetId = cloned.Id
            }).ToList();

            // Save the model
            _blogDbContext.BlogWidgets.Add(cloned);
            _blogDbContext.SaveChanges();

            return cloned;
        }

        public void DeleteModel(string widgetId)
        {
            var model = GetModel(widgetId);

            _blogDbContext.BlogWidgets.Remove(model);
            _blogDbContext.SaveChanges();
        }

        public BlogWidget GetModel(string widgetId)
        {
            return _blogDbContext.BlogWidgets
                .Include(x => x.Categories)
                //.Include(x => x.ConnectionGroups)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == widgetId);
        }

        public BlogWidget GetDefaultModel()
        {
            return new BlogWidget
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Sample Blog Posts",
                BlogId = KeyGen.NewGuid(),
                CreateBlog = true,
                PageSize = 5
            };
        }

      
        public void ClearWidgetCategories(string widgetId)
        {
            // Remove old mappings
            _blogDbContext.BlogWidgetCategories.RemoveRange(
                _blogDbContext.BlogWidgetCategories.Where(x => x.WidgetId == widgetId));

            _blogDbContext.SaveChanges();
        }

        public void ClearWidgetTags(string widgetId)
        {
            // Remove old mappings
            _blogDbContext.BlogWidgetTags.RemoveRange(
                _blogDbContext.BlogWidgetTags.Where(x => x.WidgetId == widgetId));

            _blogDbContext.SaveChanges();
        }

        public void SetWidgetCategories(string widgetId, IEnumerable<string> categoryIds)
        {
            // Remove old mappings
            _blogDbContext.BlogWidgetCategories.RemoveRange(
                _blogDbContext.BlogWidgetCategories.Where(x => x.WidgetId == widgetId));

            _blogDbContext.SaveChanges();

            // Add new mappings
            foreach (var categoryId in categoryIds)
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    _blogDbContext.BlogWidgetCategories.Add(new BlogWidgetCategory
                    {
                        WidgetId = widgetId,
                        CategoryId = categoryId,
                    });
                }
            }

            _blogDbContext.SaveChanges();
        }

        public bool AddWidgetTag(string widgetId, string tagId)
        {
            var existingBlogPostTag = _connectDbContext.Tags.FirstOrDefault(x => x.Id == tagId);

            if (existingBlogPostTag != null)
            {
                return true;
            }

            var newBlogWidgetTag = new BlogWidgetTag
            {
                WidgetId = widgetId,
                TagId = tagId,
            };

            _blogDbContext.BlogWidgetTags.Add(newBlogWidgetTag);
            _blogDbContext.SaveChanges();

            return true;
        }

        public void SetWidgetTags(string widgetId, IEnumerable<string> tagIds)
        {
            // Remove old mappings
            _blogDbContext.BlogWidgetTags.RemoveRange(
                _blogDbContext.BlogWidgetTags.Where(x => x.WidgetId == widgetId));

            _blogDbContext.SaveChanges();

            // Add new mappings
            foreach (var tagId in tagIds)
            {
                if (!string.IsNullOrEmpty(tagId))
                {
                    _blogDbContext.BlogWidgetTags.Add(new BlogWidgetTag
                    {
                        WidgetId = widgetId,
                        TagId = tagId,
                    });
                }
            }

            _blogDbContext.SaveChanges();
        }

    }
}
