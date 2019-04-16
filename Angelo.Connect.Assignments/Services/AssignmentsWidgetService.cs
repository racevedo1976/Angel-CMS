using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Angelo.Connect.Assignments.Data;
using Angelo.Connect.Data;
using Angelo.Connect.Assignments.Models;
using Angelo.Connect.Assignments.ViewModels;
using Angelo.Connect.Widgets;

namespace Angelo.Connect.Assignments.Services
{
    public class AssignmentsWidgetService : IWidgetService<AssignmentsWidgetViewModel>
    {
        private AssignmentsDbContext _assignmentsDb;
        private ConnectDbContext _connectDb;
        private DriveOptions _driveOptions;

        public AssignmentsWidgetService(AssignmentsDbContext assignmentsDb, ConnectDbContext connectDb, IOptions<DriveOptions> driveOptions)
        {
            _assignmentsDb = assignmentsDb;
            _connectDb = connectDb;
            _driveOptions = driveOptions.Value;
        }

        public void DeleteModel(string widgetId)
        {
            var widget = _assignmentsDb.AssignmentWidgets.FirstOrDefault(x => x.Id == widgetId);
            _assignmentsDb.AssignmentWidgets.Remove(widget);
            _assignmentsDb.SaveChanges();
        }

        //protected async Task LoadStreamSourceDataAsync(string videoId, AssignmentsWidgetViewModel model)
        //{
        //    var stream = await _assignmentsDb.VideoStreamLinks.AsNoTracking()
        //                .Where(x => x.Id == videoId)
        //                .FirstOrDefaultAsync();
        //    if (stream != null)
        //    {
        //        model.StreamId = videoId;
        //        model.SourceName = stream.Title;
        //        model.SourceUri = stream.Path;
        //    }
        //}

        //protected async Task LoadDocumentSourceDataAsync(string videoId, VideoWidgetViewModel model)
        //{

        //    var document = await _connectDb.FileDocuments
        //        .Where(x => x.DocumentId == videoId)
        //        .FirstOrDefaultAsync();
        //    if (document != null)
        //    {
        //        model.DocumentId = videoId;
        //        model.SourceName = document.FileName;

        //        var fileExt = System.IO.Path.GetExtension(document.FileName);
        //        if (fileExt.StartsWith(".")) fileExt = fileExt.Substring(1);
        //        model.VideoFileExt = fileExt;

        //        string driveUri = _driveOptions.Authority;
        //        if (!driveUri.EndsWith("/")) driveUri += "/";
        //        driveUri += "download?id=" + WebUtility.UrlEncode(document.DocumentId);
        //        model.SourceUri = driveUri;
        //    }
        //}

        //protected void LoadYouTubeSourceData(string videoId, VideoWidgetViewModel model)
        //{
        //    model.YouTubeVideoId = videoId;
        //    model.SourceUri = @"http://youtube.com/watch?v=" + videoId;
        //}

        //protected async Task LoadVideoSourceDataAsync(string videoSourceType, string videoId, VideoWidgetViewModel model)
        //{
        //    model.VideoSourceType = videoSourceType;
        //    model.SourceName = string.Empty;
        //    model.SourceUri = string.Empty;
        //    model.VideoFileExt = string.Empty;
        //    if (videoSourceType == VideoSourceTypes.Stream) await LoadStreamSourceDataAsync(videoId, model);
        //    else if (videoSourceType == VideoSourceTypes.Document) await LoadDocumentSourceDataAsync(videoId, model);
        //    else if (videoSourceType == VideoSourceTypes.YouTube) LoadYouTubeSourceData(videoId, model);
        //}

        public AssignmentsWidgetViewModel GetDefaultModel()
        {
            var model = new AssignmentsWidgetViewModel()
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Assignments"
            };
            return model;
        }

        public AssignmentsWidgetViewModel GetModel(string widgetId)
        {
            var widget = _assignmentsDb.AssignmentWidgets.AsNoTracking().FirstOrDefault(x => x.Id == widgetId);
            if (widget != null)
            {
                var model = new AssignmentsWidgetViewModel()
                {
                    Id = widget.Id,
                    Title = widget.Title,
                };
                //LoadVideoSourceDataAsync(videoWidget.VideoSourceType, videoWidget.VideoId, model).Wait();
                return model;
            }
            return null;
        }

        public void SaveModel(AssignmentsWidgetViewModel model)
        {
            var widget = new AssignmentWidget()
            {
                Id = model.Id,
                Title = model.Title
            };
            _assignmentsDb.AssignmentWidgets.Add(widget);
            _assignmentsDb.SaveChanges();
        }

        public void UpdateModel(AssignmentsWidgetViewModel model)
        {
            AssignmentWidget widget = new AssignmentWidget()
            {
                Id = model.Id,
                Title = model.Title
            };
            _assignmentsDb.Attach(widget);
            _assignmentsDb.Entry(widget).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _assignmentsDb.SaveChanges();
        }

        public AssignmentsWidgetViewModel CloneModel(AssignmentsWidgetViewModel model)
        {
            throw new NotImplementedException();
        }

    }
}
