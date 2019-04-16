using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Angelo.Common.Mvc;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Icons;
using Angelo.Connect.Menus;
using Angelo.Connect.Calendar.Security;
namespace Angelo.Connect.Calendar.Services
{
    public class ContentMenu : IMenuItemProvider
    {
        public string MenuName { get; } = MenuType.UserContent;
        public IEnumerable<IMenuItem> MenuItems { get; private set; }
        public ContentMenu(CalendarSecurityService calendarSecurity, IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var returnUrl = httpContext.Request.GetRelativeUrlEncoded();

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
                    Title = "Calendar Event",
                    Url = "javascript: void $.dialog('/admin/calendar/EventEdit',{}, " + validation + ").done(function (button) { " +
                     "  });", // "/admin/calendar/EventEdit", // "javascript: ShowEventEditContainer();",
                    Icon = IconType.Calendar,
                    AuthorizeCallback = user => {
                        return calendarSecurity.AuthorizeForCreate();
                    }
                }
            };
        }
    }
}