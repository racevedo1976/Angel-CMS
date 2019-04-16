using System.Text.Encodings.Web;
using Microsoft.Extensions.DependencyInjection;

using Angelo.Connect.Calendar.Services;
using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Services;
using Angelo.Connect.Calendar.Data;
using Angelo.Connect.Icons;
using Angelo.Connect.Widgets;
using Angelo.Connect.Calendar.UI.Components;
using Angelo.Connect.Calendar.Components;
using Angelo.Plugins;
using Angelo.Connect.Calendar.Interface;
using Angelo.Connect.Calendar.Security;
using Angelo.Connect.Menus;
using Angelo.Common.Migrations;

namespace Angelo.Connect.Calendar
{
    public class CalendarPlugin: IPlugin
    {
        public CalendarPlugin()
        {
        }

        public string Author { get; } = "SchoolInSites";
        public string Description { get; } = "Initial Calendar Plugin";
        public string Name { get; } = "Calendar Plugin";
        public string Version { get; } = "0.0.1";
        public void Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.ConfigureServices(services =>
            {
                services.AddTransient<CalendarQueryService>();
                services.AddTransient<IFolderManager<CalendarEvent>, FolderManager<CalendarEvent>>();
                services.AddTransient<CalendarSecurityService>();
                services.AddTransient<IRecurringEvent, DailyEventRecurrenceProvider>();
                services.AddTransient<IRecurringEvent, WeeklyEventRecurrenceProvider>();
                services.AddTransient<IRecurringEvent, MonthlyEventRecurrenceProvider>();
                services.AddTransient<IRecurringEvent, YearlyEventRecurrenceProvider>();

                services.AddTransient<ISecurityPermissionProvider, CalendarClientPermissionProvider>();
                services.AddTransient<ISecurityPermissionProvider, CalendarSitePermissionProvider>();
                services.AddTransient<IMenuItemProvider, ContentMenu>();
                services.AddTransient<IMenuItemProvider, EventMenu>();


                // Migrations
                services.AddTransient<IAppMigration, P480000_CreateCalendarTables>();
                services.AddTransient<IAppMigration, P480005_UpdateEventAuthorClaimType>();
                services.AddTransient<IAppMigration, P480006_AddAvailableViews>();
                services.AddTransient<IAppMigration, P480007_AddTargetToEventUrl>();
                


                services.AddTransient<IAppMigration, P490000_CreateInitialUpcomingEventsTables>();

            });

            pluginBuilder.AddDbContext<CalendarDbContext>();

            var validation = "" +
             "  function(button){       " +
             "     if (button != 'save')    " +
             "     {    " +
             "         return true;    " +
             "     }    " +
             "     " +
             "     return saveEvent(function() { });  " +
             "  }  ";

            pluginBuilder.RegisterWidget<CalendarWidgetService, CalendarWidgetSetting>(widget =>
            {
                widget.WidgetType = "calendar";
                widget.WidgetName = "Calendar Events";
                widget.Category = "Advanced";
            })
            .AddForm<CalendarWidgetSettings>(f =>
            {
                f.Title = "Calendar Settings";
                f.AjaxFlags = AjaxFlags.ALL;
            })
            .AddForm<CalendarWidgetGroupsBase>(f =>
            {
                f.Title = "Event Groups";
                f.AjaxFlags = AjaxFlags.ALL;
            })
            .AddMenuOption((context) => {
                 var ru = UrlEncoder.Default.Encode(context.HostUrl);

                 return new MenuItemSecure
                 {
                     Icon = IconType.Add,
                     Title = "Add new event",
                     Url = "javascript: void $.dialog('/admin/calendar/EventEdit',{}, " + validation + ").done(function (button) { " +
                     "  });", // "/admin/calendar/EventEdit", // "javascript: ShowEventEditContainer();",
                 };
             })
            .AddView(view =>
            {
                view.Id = "calendar-month-view";
                view.Title = "Calendar";
                view.Path = "~/Views/Widgets/calendar.cshtml"; // "~/Views/Widgets/MonthView.cshtml";
                view.IconClass = IconType.Calendar.ToString();
            });

            pluginBuilder.RegisterWidget<UpcomingEventsWidgetService, UpcomingEventsWidget>(widget =>
            {
                widget.WidgetType = "Upcoming-Events";
                widget.WidgetName = "Upcoming Events";
                widget.Category = "Advanced";
            })
            .AddForm<UpcomingEventsWidgetSettings>(f => {
                f.Title = "General Settings";
                f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
            })
             .AddForm<UpcomingEventsWidgetGroupsBase>(f =>
             {
                 f.Title = "Calendar Groups";
                 //f.AjaxFlags = AjaxFlags.POST | AjaxFlags.DELETE;
             })
            .AddView(v => {
                v.Id = "calendar-upcoming-events";
                v.Title = "Upcoming Events";
                v.Path = "~/UI/Views/Widgets/UpcomingEvents.cshtml";
                v.IconClass = IconType.Calendar.ToString();
            }); ;
        }
    }
}
