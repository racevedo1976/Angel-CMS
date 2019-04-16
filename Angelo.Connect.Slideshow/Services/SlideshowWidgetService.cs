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
    public class SlideShowWidgetService : IWidgetService<SlideShowWidget>
    {
        private DbContextOptions<SlideShowDbContext> _db;
        private SiteContext _siteContext;

        public SlideShowWidgetService(DbContextOptions<SlideShowDbContext> db,
            SiteContext siteContext)
        {
            _db = db;
            _siteContext = siteContext;
        }

        public void DeleteModel(string widgetId)
        {
            using (var db = new SlideShowDbContext(_db))
            {
                var model = db.Widgets.FirstOrDefault(x => x.Id == widgetId);
                if (model == null) return;      // Another view/session already deleted it (race condition)

                db.Widgets.Remove(model);
                db.SaveChanges();
            }
        }

        public SlideShowWidget GetDefaultModel()
        {
            return new SlideShowWidget
            {
                Id = Guid.NewGuid().ToString(),
                SiteId = _siteContext.SiteId,
                Duration = 9000,
                Height = "100%",
                BackgroundColor = "#00bfff",
                Transition = Transition.Fade
                //DefaultSlideShowDuration = 1000
            };
        }

        public SlideShowWidget GetModel(string widgetId)
        {
            using (var db = new SlideShowDbContext(_db))
            {
                return db.Widgets
                    //.Include(x => x.SlideShow)
                    .FirstOrDefault(x => x.Id == widgetId);
            }
        }

        public void SaveModel(SlideShowWidget model)
        {
            using (var db = new SlideShowDbContext(_db))
            {
                db.Widgets.Add(model);
                db.SaveChanges();
            }
        }

        public void UpdateModel(SlideShowWidget model)
        {
            using (var db = new SlideShowDbContext(_db))
            {
                db.Attach<SlideShowWidget>(model);
                db.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

                db.SaveChanges();
            }
        }

        public SlideShowWidget CloneModel(SlideShowWidget model)
        {
            var clonedWidget = model.Clone();


            // TODO: Update any child models, eg slides, etc.
            using (var db = new SlideShowDbContext(_db))
            {
                
                var slides = db.Slides
                           .Include(x => x.Layers)
                           .Where(x => x.WidgetId == model.Id)
                              .OrderBy(x => x.Position).ToList();

                clonedWidget.Id = Guid.NewGuid().ToString("N");


                var cloneSlides = slides.Select(x => new Slide
                {
                    WidgetId = clonedWidget.Id,
                    DocumentId = Guid.NewGuid().ToString("N"),
                    Title = x.Title ?? "",
                    Duration = x.Duration,
                    ImageUrl = x.ImageUrl ?? "",
                    Description = x.Description,
                    Transition = x.Transition,
                    Color = x.Color,
                    Position = x.Position,
                    Delay = x.Delay,
                    UseVideoBackground = x.UseVideoBackground,
                    VideoUrl = x.VideoUrl,
                    EnableVideoSound = x.EnableVideoSound,
                    VideoSource = x.VideoSource,
                    Layers = x.Layers.Select(l => new SlideLayer
                    {
                        Id = Guid.NewGuid().ToString("N"),
                        //SlideId = x.DocumentId,
                        Title = l.Title,
                        HorizontalAlignment = Alignment.Center,
                        VerticalAlignment = Alignment.Center,
                        Color = l.Color,
                        SourceUrl = l.SourceUrl,
                        FontFamily = l.FontFamily,
                        FontSize = l.FontSize,
                        X = l.X,
                        Y = l.Y,
                        Transition = l.Transition,
                        Position = l.Position,
                        LayerType = l.LayerType,
                        Delay = l.Delay,
                        Target = l.Target
                    }).ToList()
                });
                
                db.Widgets.Add(clonedWidget);
                db.Slides.AddRange(cloneSlides);

                db.SaveChanges();

            }

            return clonedWidget;
        }
    }


}
