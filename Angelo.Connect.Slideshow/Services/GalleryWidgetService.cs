using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.SlideShow.Models;
using Angelo.Connect.SlideShow.Data;
using Angelo.Connect.Configuration;
using Angelo.Connect.Widgets;
using AutoMapper.Extensions;

namespace Angelo.Connect.SlideShow.Services
{
    public class GalleryWidgetService : IWidgetService<GalleryWidget>
    {
        private DbContextOptions<GalleryDbContext> _db;
        private SiteContext _siteContext;

        public GalleryWidgetService(DbContextOptions<GalleryDbContext> db,
            SiteContext siteContext)
        {
            _db = db;
            _siteContext = siteContext;
        }

        public void DeleteModel(string widgetId)
        {
            using (var db = new GalleryDbContext(_db))
            {
                var model = db.Widgets.Include(x => x.GalleryItems).FirstOrDefault(x => x.Id == widgetId);
                if (model == null) return;      // Another view/session already deleted it (race condition)

                db.Widgets.Remove(model);
                db.SaveChanges();
            }
        }

        public GalleryWidget GetDefaultModel()
        {
            return new GalleryWidget
            {
                Id = Guid.NewGuid().ToString(),
                SiteId = _siteContext.SiteId
            };
        }

        public GalleryWidget GetModel(string widgetId)
        {
            using (var db = new GalleryDbContext(_db))
            {
                return db.Widgets
                    .Include(x => x.GalleryItems)
                    .FirstOrDefault(x => x.Id == widgetId);
            }
        }

        public void SaveModel(GalleryWidget model)
        {
            using (var db = new GalleryDbContext(_db))
            {
                db.Widgets.Add(model);
                db.SaveChanges();
            }
        }

        public void UpdateModel(GalleryWidget model)
        {
            using (var db = new GalleryDbContext(_db))
            {
                db.Attach<GalleryWidget>(model);
                db.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                db.SaveChanges();
            }
        }

        public GalleryWidget CloneModel(GalleryWidget model)
        {
            var cloned = model.Clone();
           
            cloned.Id = Guid.NewGuid().ToString("N");
            cloned.GalleryItems = model.GalleryItems.Select(x =>
            {
                // clone the child item so ef won't think it's tracked
                var clonedItem = x.Clone();

                // update the child item's key fields
                clonedItem.Id = Guid.NewGuid().ToString("N");
                clonedItem.WidgetId = cloned.Id;

                return clonedItem;
            }).ToList();

            using (var db = new GalleryDbContext(_db))
            {
                db.Widgets.Add(cloned);
                db.SaveChanges();
            }

            return cloned;
        }
    }
}
