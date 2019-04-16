using System;
using System.Collections.Generic;
using System.Linq;
using Angelo.Connect.Carousel.Models;
using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Configuration;
using Angelo.Connect.Widgets;
using Angelo.Connect.Carousel.Data;

namespace Angelo.Connect.Carousel.Services
{
    public class CarouselWidgetService : IWidgetService<CarouselWidget>
    {
        private DbContextOptions<CarouselDbContext> _db;
        private SiteContext _siteContext;

        public CarouselWidgetService(
            DbContextOptions<CarouselDbContext> db,
            SiteContext siteContext)
        {
            _db = db;
            _siteContext = siteContext;
        }

        public void DeleteModel(string widgetId)
        {
            using (var db = new CarouselDbContext(_db))
            {
                var model = db.CarouselWidgets.Include(x => x.Slides).FirstOrDefault(x => x.Id == widgetId);
                if (model == null) return;      // Another view/session already deleted it (race condition)

                db.CarouselWidgets.Remove(model);
                db.SaveChanges();
            }
        }

        public CarouselWidget GetDefaultModel()
        {
            return new CarouselWidget
            {
                Id = Guid.NewGuid().ToString(),
                SiteId = _siteContext.SiteId
            };
        }

        public CarouselWidget GetModel(string widgetId)
        {
            using (var db = new CarouselDbContext(_db))
            {
                return db.CarouselWidgets
                    .Include(x => x.Slides)
                    .FirstOrDefault(x => x.Id == widgetId);
            }
           
        }

        public void SaveModel(CarouselWidget model)
        {
            using (var db = new CarouselDbContext(_db))
            {
                db.CarouselWidgets.Add(model);
                db.SaveChanges();
            }
        }

        public void UpdateModel(CarouselWidget model)
        {
            using (var db = new CarouselDbContext(_db))
            {
                //db.Attach<CarouselWidget>(model);
                //db.Entry(model).State = EntityState.Modified;
                db.CarouselWidgets.Update(model);
                db.SaveChanges();
            }
        }

        public CarouselWidget CloneModel(CarouselWidget model)
        {
            var originalWidgetId = model.Id;
            var cloned = new CarouselWidget()
            {
                Id = Guid.NewGuid().ToString("N"),
                SiteId = model.SiteId,
                Title = model.Title,
                Slides = new List<CarouselSlide>()
            };

            cloned.Slides = model.Slides.Select(x =>
                new CarouselSlide()
                {
                    Id = Guid.NewGuid().ToString("N"),
                    Sort = x.Sort,
                    Title = x.Title,
                    WidgetId = cloned.Id,
                    Description = x.Description,
                    LinkText = x.LinkText,
                    LinkUrl = x.LinkUrl,
                    LinkTarget = x.LinkTarget
                }).ToList();


            using (var db = new CarouselDbContext(_db))
            {

                db.CarouselWidgets.Add(cloned);
                db.SaveChanges();
            }

            return cloned;
            
        }


    }
}
