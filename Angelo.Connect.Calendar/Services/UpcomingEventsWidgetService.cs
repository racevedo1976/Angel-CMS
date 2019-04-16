using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Widgets;
using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Calendar.Data;
using Angelo.Connect.Configuration;
using Microsoft.EntityFrameworkCore;
using AutoMapper.Extensions;

namespace Angelo.Connect.Calendar.Services
{
    public class UpcomingEventsWidgetService : IWidgetService<UpcomingEventsWidget>
    {
        private CalendarDbContext _calendarDbContext;
        private SiteContext _siteContext;

        public UpcomingEventsWidgetService(CalendarDbContext calendarDbContext,
            SiteContext siteContext)
        {
            _calendarDbContext = calendarDbContext;
            _siteContext = siteContext;
        }
       
        public void DeleteModel(string widgetId)
        {
            var model = GetModel(widgetId);

            _calendarDbContext.UpcomingEventsWidget.Remove(model);
            _calendarDbContext.SaveChanges();
        }


        public UpcomingEventsWidget GetDefaultModel()
        {
            return new UpcomingEventsWidget
            {
                Id = Guid.NewGuid().ToString("N"),
                PostsToDisplay = 5,
                UseTextColor = false,
                TextColor = "#000000"
            };
            
        }

        public UpcomingEventsWidget GetModel(string widgetId)
        {
            return _calendarDbContext.UpcomingEventsWidget.AsNoTracking()
                .FirstOrDefault(x => x.Id == widgetId);
        }

        public void SaveModel(UpcomingEventsWidget model)
        {
            _calendarDbContext.UpcomingEventsWidget.Add(model);
            _calendarDbContext.SaveChanges();
            
        }

        public void UpdateModel(UpcomingEventsWidget model)
        {
            _calendarDbContext.Attach(model);
            _calendarDbContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _calendarDbContext.SaveChanges();
            
        }

        public UpcomingEventsWidget CloneModel(UpcomingEventsWidget model)
        {
            var cloned = model.Clone();

            var groups = _calendarDbContext.UpcomingEventsWidgetEventGroups.Where(x => x.WidgetId == model.Id).ToList();

            cloned.Id = Guid.NewGuid().ToString("N");

            var clonedGroups = groups.Select(x => new UpcomingEventsWidgetEventGroup
            {
                WidgetId = cloned.Id,
                Id = Guid.NewGuid().ToString("N"),
                EventGroupId = x.EventGroupId
            }).ToList();

            _calendarDbContext.UpcomingEventsWidget.Add(model);
            _calendarDbContext.UpcomingEventsWidgetEventGroups.AddRange(clonedGroups);

            _calendarDbContext.SaveChanges();

            return cloned;
        }
    }
}
