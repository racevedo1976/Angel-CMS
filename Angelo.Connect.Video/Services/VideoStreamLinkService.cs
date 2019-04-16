using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Widgets;
using Angelo.Connect.Video.Data;
using Angelo.Connect.Video.Models;
using Angelo.Connect.Configuration;


namespace Angelo.Connect.Video.Services
{
    public class VideoStreamLinkService
    {
        private VideoDbContext _db;

        public VideoStreamLinkService(VideoDbContext db)
        {
            _db = db;
        }

        public async Task<List<VideoStreamLink>> GetClientVideoStreamLinksAsync(string clientId)
        {
            var links = await _db.VideoStreamLinks.AsNoTracking()
                            .Where(x => x.ClientId == clientId)
                            .ToListAsync();
            return links;
        }

        public async Task<VideoStreamLink> GetVideoStreamLinkAsync(string id)
        {
            var link = await _db.VideoStreamLinks.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
            return link;
        }

        public async Task<VideoStreamLink> InsertVideoStreamLink(VideoStreamLink model)
        {
            if (string.IsNullOrEmpty(model.Id))
                model.Id = Guid.NewGuid().ToString("N");
            _db.VideoStreamLinks.Add(model);
            await _db.SaveChangesAsync();
            return model;
        }

        public async Task UpdateVideoStreamLink(VideoStreamLink model)
        {
            var videoStreamLink = await _db.VideoStreamLinks.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (videoStreamLink != null)
            {
                videoStreamLink.Title = model.Title;
                videoStreamLink.Path = model.Path;
                await _db.SaveChangesAsync();
            }
        }

        public async Task DeleteVideoStreamLink(string id)
        {
            var videoStreamLink = await _db.VideoStreamLinks.FirstOrDefaultAsync(x => x.Id == id);
            if (videoStreamLink != null)
                _db.VideoStreamLinks.Remove(videoStreamLink);
            await _db.SaveChangesAsync();
        }

    }
}
