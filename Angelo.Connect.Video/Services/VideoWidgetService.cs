using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

using Angelo.Connect.Video.Data;
using Angelo.Connect.Video.Models;
using Angelo.Connect.Configuration;
using Angelo.Connect.Widgets;
using Angelo.Connect.Data;
using AutoMapper.Extensions;



namespace Angelo.Connect.Video.Services
{
    public class VideoWidgetService : IWidgetService<VideoWidgetViewModel>
    {
        private VideoDbContext _videoDb;
        private ConnectDbContext _connectDb;
        private DriveOptions _driveOptions;

        public VideoWidgetService(VideoDbContext videoDb, ConnectDbContext connectDb, IOptions<DriveOptions> driveOptions)
        {
            _videoDb = videoDb;
            _connectDb = connectDb;
            _driveOptions = driveOptions.Value;
        }

        public void DeleteModel(string widgetId)
        {
            var videoWidget = _videoDb.VideoWidgets.FirstOrDefault(x => x.Id == widgetId);
            _videoDb.VideoWidgets.Remove(videoWidget);
            _videoDb.SaveChanges();
        }

        protected async Task LoadStreamSourceDataAsync(string videoId, VideoWidgetViewModel model)
        {
            var stream = await _videoDb.VideoStreamLinks.AsNoTracking()
                        .Where(x => x.Id == videoId)
                        .FirstOrDefaultAsync();
            if (stream != null)
            {
                model.StreamId = videoId;
                model.SourceName = stream.Title;
                model.SourceUri = stream.Path;
            }
        }

        protected async Task LoadDocumentSourceDataAsync(string videoId, VideoWidgetViewModel model)
        {

            var document = await _connectDb.FileDocuments
                .Where(x => x.DocumentId == videoId)
                .FirstOrDefaultAsync();
            if (document != null)
            {
                model.DocumentId = videoId;
                model.SourceName = document.FileName;

                var fileExt = System.IO.Path.GetExtension(document.FileName);
                if (fileExt.StartsWith(".")) fileExt = fileExt.Substring(1);
                model.VideoFileExt = fileExt;

                //string driveUri = _driveOptions.Authority;
                //if (!driveUri.EndsWith("/")) driveUri += "/";
                //driveUri += "download?id=" + WebUtility.UrlEncode(document.DocumentId);
                model.SourceUri = model.SourceUri;
            }
        }

        protected void LoadYouTubeSourceData(string videoId, VideoWidgetViewModel model)
        {
            model.YouTubeVideoId = videoId;
            model.SourceUri = @"http://youtube.com/watch?v=" + videoId;
        }

        protected async Task LoadVideoSourceDataAsync(string videoSourceType, string videoId, VideoWidgetViewModel model)
        {
            model.VideoSourceType = videoSourceType;
            model.SourceName = string.Empty;
           // model.SourceUri = string.Empty;
            model.VideoFileExt = string.Empty;
            if (videoSourceType == VideoSourceTypes.Stream) await LoadStreamSourceDataAsync(videoId, model);
            else if (videoSourceType == VideoSourceTypes.Document) await LoadDocumentSourceDataAsync(videoId, model);
            else if (videoSourceType == VideoSourceTypes.YouTube) LoadYouTubeSourceData(videoId, model);
        }

        public VideoWidgetViewModel GetDefaultModel()
        {
            var model = new VideoWidgetViewModel()
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Video Player",
                VideoSourceType = string.Empty
            };
            return model;
        }

        public VideoWidgetViewModel GetModel(string widgetId)
        {
            var videoWidget = _videoDb.VideoWidgets.AsNoTracking().FirstOrDefault(x => x.Id == widgetId);
            if (videoWidget != null)
            {
                var model = new VideoWidgetViewModel()
                {
                    Id = videoWidget.Id,
                    Title = videoWidget.Title,
                    SourceUri = videoWidget.VideoUrl
                };
                LoadVideoSourceDataAsync(videoWidget.VideoSourceType, videoWidget.VideoId, model).Wait();
                return model;
            }
            return null;
        }

        public void SaveModel(VideoWidgetViewModel model)
        {
            var videoWidget = MapToWidgetModel(model);

            _videoDb.VideoWidgets.Add(videoWidget);
            _videoDb.SaveChanges();
        }

        public void UpdateModel(VideoWidgetViewModel model)
        {
            var videoWidget = MapToWidgetModel(model);

            _videoDb.Attach(videoWidget);
            _videoDb.Entry(videoWidget).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _videoDb.SaveChanges();
        }

        public VideoWidgetViewModel CloneModel(VideoWidgetViewModel model)
        {
            var clonedViewModel = model.Clone();

            clonedViewModel.Id = Guid.NewGuid().ToString("N");

            var clonedWidget = MapToWidgetModel(clonedViewModel);

            _videoDb.VideoWidgets.Add(clonedWidget);
            _videoDb.SaveChanges();

            return clonedViewModel;
        }

        private VideoWidget MapToWidgetModel(VideoWidgetViewModel viewModel)
        {
            return new VideoWidget()
            {
                Id = viewModel.Id,
                Title = viewModel.Title,
                VideoSourceType = viewModel.VideoSourceType,
                VideoId = viewModel.VideoId,
                VideoUrl = viewModel.SourceUri
            };
        }

    }
}
