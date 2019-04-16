using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


using Angelo.Connect.Announcement.Data;
using Angelo.Connect.Announcement.Models;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Widgets;
using Angelo.Connect.Services;
using AutoMapper.Extensions;

namespace Angelo.Connect.Announcement.Services
{
    public class AnnouncementWidgetService : IWidgetService<AnnouncementWidget>
    {
        private AnnouncementDbContext _announcementDbContext;
        private ConnectDbContext _connectDbContext;

        public AnnouncementWidgetService(AnnouncementDbContext announcementDbContext, ConnectDbContext connectDbContext)
        {

            _announcementDbContext = announcementDbContext;
            _connectDbContext = connectDbContext;
        }

        public void SaveModel(AnnouncementWidget model)
        {
            _announcementDbContext.AnnouncementWidgets.Add(model);
            _announcementDbContext.SaveChanges();
        }

        public void UpdateModel(AnnouncementWidget model)
        {
            _announcementDbContext.Attach(model);
            _announcementDbContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _announcementDbContext.SaveChanges();
        }

        public AnnouncementWidget CloneModel(AnnouncementWidget model)
        {
            // clone the base widget
            var cloned = new AnnouncementWidget
            {
                Id = KeyGen.NewGuid(),
                Title = model.Title,
                PageSize = model.PageSize,
                AnnouncementId = model.AnnouncementId,
                CreateAnnouncement = model.CreateAnnouncement
            };


            // clone the categories
            var categories = _announcementDbContext.AnnouncementWidgetCategories.AsNoTracking().Where(x => x.WidgetId == model.Id);

            cloned.Categories = categories.Select(x => new AnnouncementWidgetCategory
            {
                Id = Guid.NewGuid().ToString("N"),
                CategoryId = x.CategoryId,
                WidgetId = cloned.Id
            }).ToList();


            // clone the tags
            var tags = _announcementDbContext.AnnouncementWidgetTags.AsNoTracking().Where(x => x.WidgetId == model.Id);

            cloned.Tags = tags.Select(x => new AnnouncementWidgetTag
            {
                Id = Guid.NewGuid().ToString("N"),
                TagId = x.TagId,
                WidgetId = cloned.Id
            }).ToList();

            // Save the model
            _announcementDbContext.AnnouncementWidgets.Add(cloned);
            _announcementDbContext.SaveChanges();

            return cloned;
        }

        public void DeleteModel(string widgetId)
        {
            var model = GetModel(widgetId);

            _announcementDbContext.AnnouncementWidgets.Remove(model);
            _announcementDbContext.SaveChanges();
        }

        public AnnouncementWidget GetModel(string widgetId)
        {
            return _announcementDbContext.AnnouncementWidgets
                .Include(x => x.Categories)
                //.Include(x => x.ConnectionGroups)
                .Include(x => x.Tags)
                .FirstOrDefault(x => x.Id == widgetId);
        }

        public AnnouncementWidget GetDefaultModel()
        {
            return new AnnouncementWidget
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Sample Announcement Posts",
                AnnouncementId = KeyGen.NewGuid(),
                CreateAnnouncement = true,
                PageSize = 5
            };
        }

      
        public void ClearWidgetCategories(string widgetId)
        {
            // Remove old mappings
            _announcementDbContext.AnnouncementWidgetCategories.RemoveRange(
                _announcementDbContext.AnnouncementWidgetCategories.Where(x => x.WidgetId == widgetId));

            _announcementDbContext.SaveChanges();
        }

        public void ClearWidgetTags(string widgetId)
        {
            // Remove old mappings
            _announcementDbContext.AnnouncementWidgetTags.RemoveRange(
                _announcementDbContext.AnnouncementWidgetTags.Where(x => x.WidgetId == widgetId));

            _announcementDbContext.SaveChanges();
        }

        public void SetWidgetCategories(string widgetId, IEnumerable<string> categoryIds)
        {
            // Remove old mappings
            _announcementDbContext.AnnouncementWidgetCategories.RemoveRange(
                _announcementDbContext.AnnouncementWidgetCategories.Where(x => x.WidgetId == widgetId));

            _announcementDbContext.SaveChanges();

            // Add new mappings
            foreach (var categoryId in categoryIds)
            {
                if (!string.IsNullOrEmpty(categoryId))
                {
                    _announcementDbContext.AnnouncementWidgetCategories.Add(new AnnouncementWidgetCategory
                    {
                        WidgetId = widgetId,
                        CategoryId = categoryId,
                    });
                }
            }

            _announcementDbContext.SaveChanges();
        }

        public bool AddWidgetTag(string widgetId, string tagId)
        {
            var existingAnnouncementPostTag = _connectDbContext.Tags.FirstOrDefault(x => x.Id == tagId);

            if (existingAnnouncementPostTag != null)
            {
                return true;
            }

            var newAnnouncementWidgetTag = new AnnouncementWidgetTag
            {
                WidgetId = widgetId,
                TagId = tagId,
            };

            _announcementDbContext.AnnouncementWidgetTags.Add(newAnnouncementWidgetTag);
            _announcementDbContext.SaveChanges();

            return true;
        }

        public void SetWidgetTags(string widgetId, IEnumerable<string> tagIds)
        {
            // Remove old mappings
            _announcementDbContext.AnnouncementWidgetTags.RemoveRange(
                _announcementDbContext.AnnouncementWidgetTags.Where(x => x.WidgetId == widgetId));

            _announcementDbContext.SaveChanges();

            // Add new mappings
            foreach (var tagId in tagIds)
            {
                if (!string.IsNullOrEmpty(tagId))
                {
                    _announcementDbContext.AnnouncementWidgetTags.Add(new AnnouncementWidgetTag
                    {
                        WidgetId = widgetId,
                        TagId = tagId,
                    });
                }
            }

            _announcementDbContext.SaveChanges();
        }

    }
}
