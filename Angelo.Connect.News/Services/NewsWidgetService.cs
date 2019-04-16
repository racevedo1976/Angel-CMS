using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;


using Angelo.Connect.News.Data;
using Angelo.Connect.News.Models;
using Angelo.Connect.Data;
using Angelo.Connect.Widgets;

namespace Angelo.Connect.News.Services
{
    public class NewsWidgetService : IWidgetService<NewsWidget>
    {
        private readonly NewsDbContext _newsDbContext;
        private readonly ConnectDbContext _connectDbContext;

        public NewsWidgetService(NewsDbContext newsDbContext, ConnectDbContext connectDbContext)
        {

            _newsDbContext = newsDbContext;
            _connectDbContext = connectDbContext;
        }

        public void SaveModel(NewsWidget model)
        {
            _newsDbContext.NewsWidgets.Add(model);
            _newsDbContext.SaveChanges();
        }

        public void UpdateModel(NewsWidget model)
        {
            _newsDbContext.Attach(model);
            _newsDbContext.Entry(model).State = EntityState.Modified;

            _newsDbContext.SaveChanges();
        }

        public NewsWidget CloneModel(NewsWidget model)
        {
            // clone the base widget
            var cloned = new NewsWidget
            {
                Id = KeyGen.NewGuid(),
                Title = model.Title,
                PageSize = model.PageSize,
                NewsId = model.NewsId,
                CreateNews = model.CreateNews
            };


            // clone the categories
            var categories = _newsDbContext.NewsWidgetCategories.AsNoTracking().Where(x => x.WidgetId == model.Id);

            cloned.Categories = categories.Select(x => new NewsWidgetCategory
            {
                Id = Guid.NewGuid().ToString("N"),
                CategoryId = x.CategoryId,
                WidgetId = cloned.Id
            }).ToList();


            // clone the tags
            var tags = _newsDbContext.NewsWidgetTags.AsNoTracking().Where(x => x.WidgetId == model.Id);

            cloned.Tags = tags.Select(x => new NewsWidgetTag
            {
                Id = Guid.NewGuid().ToString("N"),
                TagId = x.TagId,
                WidgetId = cloned.Id
            }).ToList();

            // Save the model
            _newsDbContext.NewsWidgets.Add(cloned);
            _newsDbContext.SaveChanges();

            return cloned;
        }

        public void DeleteModel(string widgetId)
        {
            var model = GetModel(widgetId);

            _newsDbContext.NewsWidgets.Remove(model);
            _newsDbContext.SaveChanges();
        }

        public NewsWidget GetModel(string widgetId)
        {
            return _newsDbContext.NewsWidgets
                .Include(x => x.Categories)
                //.Include(x => x.ConnectionGroups)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == widgetId);
        }

        public NewsWidget GetDefaultModel()
        {
            return new NewsWidget
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Sample News Posts",
                NewsId = KeyGen.NewGuid(),
                CreateNews = true,
                PageSize = 5
            };
        }

      
        public void ClearWidgetCategories(string widgetId)
        {
            // Remove old mappings
            _newsDbContext.NewsWidgetCategories.RemoveRange(
                _newsDbContext.NewsWidgetCategories.Where(x => x.WidgetId == widgetId));

            _newsDbContext.SaveChanges();
        }

        public void ClearWidgetTags(string widgetId)
        {
            // Remove old mappings
            _newsDbContext.NewsWidgetTags.RemoveRange(
                _newsDbContext.NewsWidgetTags.Where(x => x.WidgetId == widgetId));

            _newsDbContext.SaveChanges();
        }

        public void SetWidgetCategories(string widgetId, IEnumerable<string> categoryIds)
        {
            // Remove old mappings
            _newsDbContext.NewsWidgetCategories.RemoveRange(
                _newsDbContext.NewsWidgetCategories.Where(x => x.WidgetId == widgetId));

            _newsDbContext.SaveChanges();

            // Add new mappings
            foreach (var categoryId in categoryIds)
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    _newsDbContext.NewsWidgetCategories.Add(new NewsWidgetCategory
                    {
                        WidgetId = widgetId,
                        CategoryId = categoryId,
                    });
                }
            }

            _newsDbContext.SaveChanges();
        }

        public bool AddWidgetTag(string widgetId, string tagId)
        {
            var existingNewsPostTag = _connectDbContext.Tags.FirstOrDefault(x => x.Id == tagId);

            if (existingNewsPostTag != null)
            {
                return true;
            }

            var newNewsWidgetTag = new NewsWidgetTag
            {
                WidgetId = widgetId,
                TagId = tagId,
            };

            _newsDbContext.NewsWidgetTags.Add(newNewsWidgetTag);
            _newsDbContext.SaveChanges();

            return true;
        }

        public void SetWidgetTags(string widgetId, IEnumerable<string> tagIds)
        {
            // Remove old mappings
            _newsDbContext.NewsWidgetTags.RemoveRange(
                _newsDbContext.NewsWidgetTags.Where(x => x.WidgetId == widgetId));

            _newsDbContext.SaveChanges();

            // Add new mappings
            foreach (var tagId in tagIds)
            {
                if (!string.IsNullOrEmpty(tagId))
                {
                    _newsDbContext.NewsWidgetTags.Add(new NewsWidgetTag
                    {
                        WidgetId = widgetId,
                        TagId = tagId,
                    });
                }
            }

            _newsDbContext.SaveChanges();
        }

    }
}
