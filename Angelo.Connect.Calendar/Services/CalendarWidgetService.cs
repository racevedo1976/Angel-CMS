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
    public class CalendarWidgetService : IWidgetService<CalendarWidgetSetting>
    {
        private CalendarDbContext _calendarDbContext;
        private SiteContext _siteContext;

        public CalendarWidgetService(CalendarDbContext calendarDbContext,
            SiteContext siteContext)
        {
            _calendarDbContext = calendarDbContext;
            _siteContext = siteContext;
        }
       
        public void DeleteModel(string widgetId)
        {
            var model = GetModel(widgetId);

            _calendarDbContext.CalendarWidgetSettings.Remove(model);
            _calendarDbContext.SaveChanges();
        }


        public CalendarWidgetSetting GetDefaultModel()
        {
            return new CalendarWidgetSetting
            {
                Id = Guid.NewGuid().ToString("N"),
                SiteId = _siteContext.SiteId,
                DefaultView = "month",
                Format12 = true,
                StartDayOfWeek = "Sunday",
                HideWeekends = false,
                MonthView = true,
                DayView = true,
                ListView = true,
                WeekView = true
            };
            
        }

        public CalendarWidgetSetting GetModel(string widgetId)
        {
            return _calendarDbContext.CalendarWidgetSettings.AsNoTracking()
                .FirstOrDefault(x => x.Id == widgetId);
        }

        public void SaveModel(CalendarWidgetSetting model)
        {
            _calendarDbContext.CalendarWidgetSettings.Add(model);
            _calendarDbContext.SaveChanges();
            
        }

        public void UpdateModel(CalendarWidgetSetting model)
        {
            _calendarDbContext.Attach(model);
            _calendarDbContext.Entry(model).State = Microsoft.EntityFrameworkCore.EntityState.Modified;

            _calendarDbContext.SaveChanges();
            
        }

        public CalendarWidgetSetting CloneModel(CalendarWidgetSetting model)
        {
            var cloned = model.Clone();

            var groups = _calendarDbContext.CalendarWidgetEventGroups.Where(x => x.WidgetId == model.Id).ToList();

            cloned.Id = Guid.NewGuid().ToString("N");

            var clonedGroups = groups.Select(x => new CalendarWidgetEventGroup
            {
                WidgetId = cloned.Id,
                Id = Guid.NewGuid().ToString("N"),
                EventGroupId = x.EventGroupId,
                Color = x.Color,
                Title = x.Title

            }).ToList();
            //TODO: Update any child models, eg EventGroups to show, etc..
            /*
            foreach(var eventGroup in cloned.EventGroups)
            {
                eventGroup.WidgetId = cloned.Id;
            }
            */

            _calendarDbContext.CalendarWidgetSettings.Add(model);
            _calendarDbContext.CalendarWidgetEventGroups.AddRange(clonedGroups);

            _calendarDbContext.SaveChanges();

            return cloned;
        }
    }
}
