using Angelo.Common.Mvc;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Calendar.Security;
using Angelo.Connect.Icons;
using Angelo.Connect.Menus;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Models
{
    public class EventMenu : IMenuItemProvider
    {
        public IEnumerable<IMenuItem> MenuItems { get; private set; }

        public string MenuName { get; } = MenuType.EventTools;

        public EventMenu(CalendarSecurityService calendarSecurity, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var returnUrl = httpContext.Request.Headers["Referer"].ToString();

            var validation = "" +
                             "  function(button){       " +
                             "     if (button != 'save')    " +
                             "     {    " +
                             "         return true;    " +
                             "     }    " +
                             "     " +
                             "     return saveEvent(function() { });  " +
                             "  }  ";

            MenuItems = new List<IMenuItem>()
            {
                new MenuItemSecureCustom() {
                    Title = "Delete Event",
                    Url = "javascript: void $('#calendareventform').form().delete('/api/content/calendar/deletecalendarevent?eventId=[eventid]').done(function () { window.location = '" + returnUrl + "' });", // "/admin/calendar/EventEdit", // "javascript: ShowEventEditContainer();",
                    Icon = IconType.Trashcan,
                    AuthorizeCallback = user => {
                        return calendarSecurity.AuthorizeForCreate();
                    }
                },
                new MenuItemSecureCustom() {
                    Title = "Edit Event",
                    Url = "javascript: void $.dialog('/admin/calendar/EventEdit?id=[eventid]',{}, " + validation + ").done(function (button) { " +
                          " if (button === 'save') window.location = '" + returnUrl + "' });", 
                    Icon = IconType.Pencil,
                    AuthorizeCallback = user => {
                        return calendarSecurity.AuthorizeForCreate();
                    }
                },
                new MenuItemSecureCustom() {
                    Title = "Add To Google",
                    Url = "javascript: void window.open('https://www.google.com/calendar/render?action=TEMPLATE&text=[title]&location=&dates=[startdate]/[enddate]&details=[description]&sf=true&output=xml', '_blank');", // "/admin/calendar/EventEdit", // "javascript: ShowEventEditContainer();",
                    Icon = IconType.GooglePlus,
                    AuthorizeCallback = user => {
                        return true;
                    }
                },
                new MenuItemSecureCustom() {
                    Title = "Add To Outlook",
                    Url = "javascript: void window.open('/sys/content/calendar/createoutlookevent?id=[eventid]', '_blank');", // "/admin/calendar/EventEdit", // "javascript: ShowEventEditContainer();",
                    Icon = IconType.Windows,
                    AuthorizeCallback = user => {
                        return true;
                    }
                },
            };
        }
    }
}
