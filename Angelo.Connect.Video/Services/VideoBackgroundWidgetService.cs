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
    public class VideoBackgroundWidgetService : IWidgetService<VideoBackgroundWidgetViewModel>
    {
        private VideoDbContext _videoDb;

        public VideoBackgroundWidgetService(VideoDbContext videoDb)
        {
            _videoDb = videoDb;
        }

        public void DeleteModel(string widgetId)
        {
            var videoWidget = _videoDb.VideoWidgets.FirstOrDefault(x => x.Id == widgetId);
            _videoDb.VideoWidgets.Remove(videoWidget);
            _videoDb.SaveChanges();
        }

        public VideoBackgroundWidgetViewModel GetDefaultModel()
        {
            var model = new VideoBackgroundWidgetViewModel()
            {
                Id = Guid.NewGuid().ToString("N"),
                VideoSourceType = string.Empty
            };
            return model;
        }

        public VideoBackgroundWidgetViewModel GetModel(string widgetId)
        {
            var videoWidget = _videoDb.VideoBackgroundWidgets.AsNoTracking().FirstOrDefault(x => x.Id == widgetId);
            if (videoWidget != null)
            {
                var model = new VideoBackgroundWidgetViewModel()
                {
                    Id = videoWidget.Id,
                    VideoSourceType = videoWidget.VideoSourceType,
                    SourceUri = videoWidget.VideoUrl,
                    YoutubeVideoId = videoWidget.YoutubeVideoId,
                    VimeoVideoId = videoWidget.VimeoVideoId,
                    Positioning = videoWidget.Positioning,
                    Autoplay = videoWidget.Autoplay,
                    ShowPlayerControls = videoWidget.ShowPlayerControls
                };
                return model;
            }
            return null;
        }

        public void SaveModel(VideoBackgroundWidgetViewModel model)
        {
            var videoWidget = MapToWidgetModel(model);

            _videoDb.VideoBackgroundWidgets.Add(videoWidget);
            _videoDb.SaveChanges();
        }

        public void UpdateModel(VideoBackgroundWidgetViewModel model)
        {
            var videoWidget = MapToWidgetModel(model);

            _videoDb.Attach(videoWidget);
            _videoDb.Entry(videoWidget).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _videoDb.SaveChanges();
        }

        public VideoBackgroundWidgetViewModel CloneModel(VideoBackgroundWidgetViewModel model)
        {
            var clonedViewModel = model.Clone();

            clonedViewModel.Id = Guid.NewGuid().ToString("N");

            var clonedWidget = MapToWidgetModel(clonedViewModel);

            _videoDb.VideoBackgroundWidgets.Add(clonedWidget);
            _videoDb.SaveChanges();

            return clonedViewModel;
        }

        private VideoBackgroundWidget MapToWidgetModel(VideoBackgroundWidgetViewModel viewModel)
        {
            return new VideoBackgroundWidget()
            {
                Id = viewModel.Id,
                VideoSourceType = viewModel.VideoSourceType,
                VideoUrl = viewModel.SourceUri,
                YoutubeVideoId = viewModel.YoutubeVideoId,
                VimeoVideoId = viewModel.VimeoVideoId,
                Positioning = viewModel.Positioning,
                Autoplay = viewModel.Autoplay,
                ShowPlayerControls = viewModel.ShowPlayerControls
            };
        }

    }
}
