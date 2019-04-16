using Angelo.Connect.Carousel.Data;
using Angelo.Connect.Carousel.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Carousel.Services
{
    public class CarouselSlideService
    {
        private DbContextOptions<CarouselDbContext> _db;

        public CarouselSlideService(DbContextOptions<CarouselDbContext> db)
        {
            _db = db;
        }

        public void AddSlide(CarouselSlide model)
        {
            using (var db = new CarouselDbContext(_db))
            {

                db.CarouselSlides.Add(model);
                db.SaveChanges();
            }
        }

        public CarouselSlide UpdateSlide(CarouselSlide model)
        {
            using (var db = new CarouselDbContext(_db))
            {
                var currentSlide = db.CarouselSlides.FirstOrDefault(x => x.Id == model.Id);

                if (currentSlide != null)
                {
                    currentSlide.Title = model.Title;
                    currentSlide.Description = model.Description;
                    currentSlide.LinkUrl = model.LinkUrl;
                    currentSlide.LinkText = model.LinkText;
                    currentSlide.LinkTarget = model.LinkTarget;
                    db.CarouselSlides.Update(currentSlide);
                    db.SaveChanges();

                    return currentSlide;
                }

                return model;

            }
        }

        internal CarouselSlide Get(string id)
        {
            using (var db = new CarouselDbContext(_db))
            {
                return db.CarouselSlides.FirstOrDefault(x => x.Id == id);
            }
        }

        internal void Delete(CarouselSlide model)
        {
            using (var db = new CarouselDbContext(_db))
            {
                var currentSlide = db.CarouselSlides.FirstOrDefault(x => x.Id == model.Id);

                if (currentSlide != null)
                {
                   
                    db.CarouselSlides.Remove(currentSlide);
                    db.SaveChanges();
                }
                
            }
        }

        public async Task<bool> UpdateSortOrder(string id, int sortNumber)
        {
            using (var db = new CarouselDbContext(_db))
            {
                var slide = await db.CarouselSlides.FirstOrDefaultAsync(x => x.Id == id);

                slide.Sort = sortNumber;
               
                await db.SaveChangesAsync();
            }

            return true;
        }
    }
}
