using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.SlideShow.Data;
using Angelo.Connect.SlideShow.Models;
using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Data;

namespace Angelo.Connect.SlideShow.Services
{
    public class GalleryService
    {
        private GalleryDbContext _db;
        private ConnectDbContext _connectDb;

        public GalleryService(GalleryDbContext db, ConnectDbContext connectDb)
        {
            _db = db;
            _connectDb = connectDb;
        }

        public async Task<IList<GalleryItem>> GetItems(string widgetId)
        {
            return await _db.GalleryItems.Where(x => x.WidgetId == widgetId).ToListAsync();
        }

        public async Task<bool> AddItem(GalleryItem item)
        {
            if (_db.GalleryItems.Any(x => x.Url == item.Url && x.WidgetId == item.WidgetId))
            {
                return true;
            }

            item.Id = KeyGen.NewGuid();

            _db.GalleryItems.Add(item);

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveTitle(string id, string title)
        {
            var widget = await  _db.Widgets.FirstOrDefaultAsync(x => x.Id == id);

            widget.Title = title;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveUrl(string id, string url)
        {
            var item = await _db.GalleryItems.FirstOrDefaultAsync(x => x.Id == id);

            item.Url = url;

            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteItem(string id)
        {
            var item = await _db.GalleryItems.FirstOrDefaultAsync(x => x.Id == id);

            _db.GalleryItems.Remove(item);
            await _db.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SaveItemCaption(string id, string caption)
        {
            var galleryItem = await _db.GalleryItems.FirstOrDefaultAsync(x => x.Id == id);

            galleryItem.Title = caption;

            await _db.SaveChangesAsync();

            return true;
        }
    }
}
