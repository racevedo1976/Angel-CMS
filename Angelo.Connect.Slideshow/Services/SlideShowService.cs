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
    public class SlideShowService
    {
        // This should solve the dueling problems of concurrency errors with a shared DB and the desire to keep the DB context
        // open to allow for IQueryable combiners.
        private DbContextOptions<SlideShowDbContext> _writeDb;
        private SlideShowDbContext _readDb;
        private ConnectDbContext _connectDb;

        public SlideShowService(SlideShowDbContext readDb, DbContextOptions<SlideShowDbContext> writeDb, ConnectDbContext connectDb)
        {
            _readDb = readDb;
            _writeDb = writeDb;
            _connectDb = connectDb;
        }

        public async Task<SlideLayer> GetLayerAsync(string layerId)
        {
            return await _readDb.SlideLayers.FirstOrDefaultAsync(x => x.Id == layerId);
        }

        public SlideLayer SaveLayer(SlideLayer layer)
        {
            _readDb.SlideLayers.Add(layer);
            _readDb.SaveChanges();

            return layer;
        }
        public void UpdateLayer(SlideLayer layer)
        {
            _readDb.Attach(layer);
            _readDb.Entry(layer).State = EntityState.Modified;

            _readDb.SaveChanges();
        }

        internal void DeleteLayer(SlideLayer layer)
        {
            _readDb.SlideLayers.Remove(layer);
            _readDb.SaveChanges();
        }

        public int GetSlidesMaxPosition(string widgetId)
        {
            return _readDb.Slides.Where(x => x.WidgetId == widgetId).Max(x => x.Position);
        }
        //public async Task<Models.SlideShow> CreateSlideShowAsync(Models.SlideShow slideshow)
        //{
        //    using (var db = new SlideShowDbContext(_writeDb))
        //    {
        //        await db.AddAsync(slideshow);
        //        await db.SaveChangesAsync();
        //    }

        //    return slideshow;
        //}

        // NOTE: Unable to return IQueryable, as it has to cross DB contexts to get the ownerId.
        //public IEnumerable<Models.SlideShow> List(string userId)
        //{
        //    // TODO Fix this...majorly hacked due to null userId in the UserContext
        //    return _readDb.SlideShows
        //        .Where(x => !string.IsNullOrEmpty(x.FolderId))//  TODO Put the candle...back.   && GetOwnerId(x) == userId)

        //        .GroupBy(x => x.Title) // Hack
        //        .Select(x => x.First())

        //        .ToArray();
        //}
        //public Models.SlideShow Get(string id)
        //{
        //    return _readDb.SlideShows
        //        .SingleOrDefault(x => x.Id == id);
        //}

        //private string GetOwnerId(SlideShowWidget widget)
        //{
        //    var folderId = widget.FolderId;

        //    var folder = _connectDb
        //        .Folders
        //        .SingleOrDefault(x => x.Id == folderId);

        //    if (folder == null) throw new InvalidOperationException($"Folder not found for widget: '{widget.Title}'");

        //    return folder.OwnerId;
        //}
    }
}
