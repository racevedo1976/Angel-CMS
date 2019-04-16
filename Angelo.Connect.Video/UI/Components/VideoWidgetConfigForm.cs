using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Video.Models;
using Angelo.Connect.Video.Services;

namespace Angelo.Connect.Video.UI.Components
{
    public class VideoWidgetConfigForm : ViewComponent
    {
        SiteContext _siteContext;
        ConnectDbContext _db;
        VideoWidgetService _videoService;
        VideoStreamLinkService _linkService;
        UserContext _userContext;

        public VideoWidgetConfigForm(SiteContext siteContext, ConnectDbContext db, 
            VideoWidgetService videoService, VideoStreamLinkService linkService, UserContext userContext)
        {
            _siteContext = siteContext;
            _db = db;
            _videoService = videoService;
            _linkService = linkService;
            _userContext = userContext;
        }

        protected async Task<List<SelectListItem>> GetVideoStreamLinksAsSelectListAsync()
        {
            var clientId = _siteContext.Client.Id;
            var list = new List<SelectListItem>();
            var videoStreamLinks = (await _linkService.GetClientVideoStreamLinksAsync(clientId)).OrderBy(x => x.Title);
            foreach (var link in videoStreamLinks)
            {
                var item = new SelectListItem()
                {
                    Value = link.Id,
                    Text = link.Title,
                    Selected = false
                };
                list.Add(item);
            }
            return list;
        }

        protected async Task<List<SelectListItem>> GetFileDocumentsAsSelectListAsync(string ownerId)
        {
            var fileDocs = await _db.FileDocuments
                .Join(_db.FolderItems, l => l.DocumentId, r => r.DocumentId, (fDoc, fItem) => new { fDoc, fItem })
                .Join(_db.Folders, combined => combined.fItem.FolderId, folder => folder.Id,
                (combined, folder) => new
                {
                    DocumentId = combined.fDoc.DocumentId,
                    Title = combined.fDoc.Title,
                    FileName = combined.fDoc.FileName,
                    FileType = combined.fDoc.FileType,
                    OwnerId = folder.OwnerId
                })
                .Where(x => (x.FileType == FileType.Video) && (x.OwnerId == ownerId))
                .OrderBy(x => x.Title)
                .ToListAsync();

            //var fileDocs = await (from fd in _db.FileDocuments
            //                 join fi in _db.FolderItems on fd.DocumentId equals fi.DocumentId
            //                 join f in _db.Folders on fi.FolderId equals f.Id
            //                 where ((fd.FileType == FileType.Video) && (f.OwnerId == ownerId))
            //                 select new
            //                 {
            //                     DocumentId = fd.DocumentId,
            //                     FileName = fd.FileName
            //                 }).ToListAsync();

            var items = new List<SelectListItem>();
            foreach (var doc in fileDocs)
            {
                items.Add(new SelectListItem()
                {
                    Value = doc.DocumentId,
                    Text = string.IsNullOrEmpty(doc.Title) ? doc.FileName : doc.Title,
                });
            }
            return items;
        }

        protected async Task<List<SelectListItem>> GetFileDocumentsAsSelectListAsync()
        {
            var clientId = _siteContext.Client.Id;
            var siteId = _siteContext.SiteId;
            var userId = _userContext.UserId ?? "13B2D0D1-F8A6-487E-9D60-A1E89DCC610B";

            var list = new List<SelectListItem>();
            list.AddRange(await GetFileDocumentsAsSelectListAsync(clientId));
            list.AddRange(await GetFileDocumentsAsSelectListAsync(siteId));
            list.AddRange(await GetFileDocumentsAsSelectListAsync(userId));

            return list;
        }

        public List<SelectListItem> GetVideoSourceTypesAsSelectList()
        {
            var list = new List<SelectListItem>();
            list.Add(new SelectListItem()
            {
                Value = VideoSourceTypes.Document,
                Text = "On Demand Videos",
                Selected = false
            });

            if (isLiveVideoEnabled())
            {
                list.Add(new SelectListItem()
                {
                    Value = VideoSourceTypes.Stream,
                    Text = "Live Videos",
                    Selected = false
                });
            }

            list.Add(new SelectListItem()
            {
                Value = VideoSourceTypes.YouTube,
                Text = "YouTube Videos",
                Selected = false
            });
            return list;
        }

        public async Task<IViewComponentResult> InvokeAsync(VideoWidgetViewModel model)
        {
            ViewBag.VideoSourceTypes = GetVideoSourceTypesAsSelectList();

            if (isLiveVideoEnabled())
            {
                ViewBag.LiveVideoList = await GetVideoStreamLinksAsSelectListAsync();
            }

            //ViewBag.OnDemandVideoList = await GetFileDocumentsAsSelectListAsync();
            return View(model);
        }

        private bool isLiveVideoEnabled()
        {
            return _siteContext.ProductContext.Features.Get(FeatureId.LiveVideo)?.GetSettingValue<bool>("enabled") ?? false;
        }
    }
}
