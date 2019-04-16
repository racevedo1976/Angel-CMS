using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using Angelo.Connect.Calendar.Services;
using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Configuration;
using Angelo.Connect.Services;
using Angelo.Connect.Calendar.Security;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Calendar.UI.ViewModels;
using Angelo.Identity;

namespace Angelo.Connect.Calendar.UI.Controllers.Api
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CalendarDataController : Controller
    {
        private CalendarQueryService _calendarQueryService;
        private CalendarWidgetService _calendarWidget;
        private UpcomingEventsWidgetService _upcomingEventsWidget;
        private CategoryManager _categoryManager;
        private IContextAccessor<UserContext> _userContextAccessor;
        private SiteContext _siteContext;
        private CalendarSecurityService _calendarSecurity;
        private UserManager _userManager;

        public CalendarDataController(CategoryManager categoryManager, 
            CalendarQueryService calendarqueryService, 
            CalendarWidgetService calendarWidget,
            UpcomingEventsWidgetService upcomingEventsWidget,
            IContextAccessor<UserContext> userContextAccessor,
            SiteContext siteContext,
            CalendarSecurityService calendarSecurity,
            UserManager userManager)
        {
            _calendarQueryService = calendarqueryService;
            _calendarWidget = calendarWidget;
            _upcomingEventsWidget = upcomingEventsWidget;
            _categoryManager = categoryManager;
            _userContextAccessor = userContextAccessor;
            _siteContext = siteContext;
            _calendarSecurity = calendarSecurity;
            _userManager = userManager;
        }


       // [HttpGet, Route("/api/content/calendar/events")]
        public IActionResult GetEvents(string widgetId, string from, string to)
        {
            if (ModelState.IsValid)
            {
                var eventsFrom = DateTime.Parse(from);
                var eventsTo = DateTime.Parse(to);
                var events = _calendarQueryService.GetEventsByDate(widgetId, eventsFrom, eventsTo).ToList();

                var eventsViewModel = events.ToEventViewModel(_userManager);
                
                return Ok(
                    new CalendarResultViewModel
                    {
                        success = 1,
                        result = eventsViewModel
                    }
                );
            }

            return BadRequest(ModelState);
        }

        public IActionResult CanAddEvents()
        {
            var isAuthorized = _calendarSecurity.AuthorizeForCreateEvents();
            var isAuthenticated = _calendarSecurity.IsUserAuthenticated();

            return new JsonResult(new { isAuthorized = isAuthorized, isAuthenticated = isAuthenticated });
        }

        [Authorize]
        //[HttpPost, Route("/api/content/Calendar/post")]
        public IActionResult UpdateCalendarEvent(CalendarEvent model)
        {
            if (ModelState.IsValid)
            {
                var userContext = _userContextAccessor.GetContext();
                Ensure.NotNull(userContext, "User context is missing.");
                Ensure.NotNullOrEmpty(userContext.UserId, "Cannot resolve user information.");

                var existingEvent = _calendarQueryService.GetEventById(model.EventId);

                if (existingEvent == null)
                {
                    model.Posted = DateTime.Now;
                    model.Title = string.IsNullOrEmpty(model.Title) ? "" : model.Title;
                    model.SiteId = _siteContext.SiteId;
                    model.UserId = userContext.UserId;
                    //model.EventId = Guid.NewGuid().ToString("N");
                    model = _calendarQueryService.SaveEvent(model);
                    return Ok(model);
                }
                else
                {
                    existingEvent.Title = string.IsNullOrEmpty(model.Title) ? "" : model.Title;
                    existingEvent.EventStart = model.EventStart;
                    existingEvent.EventEnd = model.EventEnd;
                    existingEvent.Posted = DateTime.Now;
                    existingEvent.Style = model.Style;
                    existingEvent.Description = model.Description;
                    existingEvent.Phone = model.Phone;
                    existingEvent.BackgroundColor = model.BackgroundColor;
                    existingEvent.AllDayEvent = model.AllDayEvent;
                    existingEvent.IsRecurrent = model.IsRecurrent;
                    existingEvent.Location = model.Location;
                    existingEvent.ShowOrganizerName = model.ShowOrganizerName;
                    existingEvent.ShowPhoneNumber = model.ShowPhoneNumber;
                    existingEvent.Url = model.Url;
                    existingEvent.LinkTarget = model.LinkTarget;
                    existingEvent.RecurrenceDetails.Count = model.RecurrenceDetails?.Count;
                    existingEvent.RecurrenceDetails.DayOfMonth = model.RecurrenceDetails?.DayOfMonth;
                    existingEvent.RecurrenceDetails.DaysOfWeek = model.RecurrenceDetails?.DaysOfWeek;
                    existingEvent.RecurrenceDetails.EndDate = model.RecurrenceDetails?.EndDate;
                    existingEvent.RecurrenceDetails.Frequency = model.RecurrenceDetails?.Frequency;
                    existingEvent.RecurrenceDetails.Interval = model.RecurrenceDetails.Interval;
                    existingEvent.RecurrenceDetails.Months = model.RecurrenceDetails?.Months;
                    
                    _calendarQueryService.UpdateEvent(existingEvent);
                    return Ok(model);
                }

            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/calendar")]
        public IActionResult UpdateCalendarSettings(CalendarWidgetSetting model)
        {
            if (ModelState.IsValid)
            {
                var existingModel = _calendarWidget.GetModel(model.Id);
                if (existingModel == null)
                {
                    _calendarWidget.SaveModel(model);
                    existingModel = model;
                }
                else
                {
                    existingModel.DefaultView = model.DefaultView;
                    existingModel.Format12 = model.Format12;
                    existingModel.StartDayOfWeek = model.StartDayOfWeek;
                    existingModel.HideWeekends = model.HideWeekends;
                    existingModel.MonthView = model.MonthView;
                    existingModel.DayView = model.DayView;
                    existingModel.WeekView = model.WeekView;
                    existingModel.ListView = model.ListView;
                    
                    
                     _calendarWidget.UpdateModel(existingModel);
                }


                return Ok(existingModel);
            }

            return BadRequest();
        }

        [Authorize]
        [HttpPost, Route("/api/widgets/upcomingevents")]
        public IActionResult UpdateUpcomingEventsSettings(UpcomingEventsWidget model)
        {
            if (ModelState.IsValid)
            {
                var existingModel = _upcomingEventsWidget.GetModel(model.Id);
                if (existingModel == null)
                {
                    _upcomingEventsWidget.SaveModel(model);
                    existingModel = model;
                }
                else
                {
                    existingModel.Title = model.Title;
                    existingModel.PostsToDisplay = model.PostsToDisplay;
                    existingModel.UseTextColor = model.UseTextColor;
                    existingModel.TextColor = model.TextColor;

                    _upcomingEventsWidget.UpdateModel(existingModel);
                }

                return Ok(existingModel);
            }

            return BadRequest();
        }

        [Authorize]
        // [HttpPost, Route("/api/posts/blogCategoryCreate")]
        public IActionResult CreateEventGroup([DataSourceRequest] DataSourceRequest request, CalendarEventGroup model)
        {
            if (ModelState.IsValid)
            {
                var userContext = _userContextAccessor.GetContext();
                Ensure.NotNull(userContext, "User context is missing.");
                Ensure.NotNullOrEmpty(userContext.UserId, "Cannot resolve user information.");

                if (string.IsNullOrEmpty(model?.EventGroupId) && !string.IsNullOrEmpty(model?.Title))
                {
                    var eventGroup = new CalendarEventGroup
                    {
                        Title = model.Title,
                        UserId = userContext.UserId,
                        SiteId = _siteContext.SiteId,
                        EventGroupId = KeyGen.NewGuid()
                    };
                    
                    eventGroup = _calendarQueryService.CreateEventGroup(eventGroup);

                    return Json(new[] { eventGroup }.ToDataSourceResult(request, ModelState));
                }

                return Ok(model);
            }

            return BadRequest(ModelState);
        }


        [HttpGet, Route("/api/content/calendar/categories")]
        public IActionResult GetCategories()
        {
            if (ModelState.IsValid)
            {
                
                var allCategories = _categoryManager.GetClientCategoriesAsync().Result;
               
                return Ok(allCategories);
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/content/calendar/saveeventgroups")]
        public IActionResult SaveEventCategories(string eventId, string[] values)
        {
            if (ModelState.IsValid)
            {
                if (values.Any())
                {
                    _calendarQueryService.RemoveEventGroups(eventId);

                    foreach (var groupId in values)
                    {

                        _calendarQueryService.SaveEventGroup(eventId, groupId);
                    }
                }
              
                return Ok();
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/content/calendar/saveeventtags")]
        public IActionResult SaveEventTags(string eventId, string[] values)
        {
            if (ModelState.IsValid)
            {
                if (values.Any())
                {
                    _calendarQueryService.RemoveEventTags(eventId);

                    foreach (var tagId in values)
                    {

                        _calendarQueryService.SaveEventTags(new CalendarEventTag
                        {
                            EventId = eventId,
                            TagId = tagId,
                            Id = Guid.NewGuid().ToString()
                        });
                    }
                }

                return Ok();
            }

            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/content/calendar/savewidgetgroups")]
        public IActionResult SaveWidgetGroups(string widgetId, string[] widgetGroups)
        {
            if (ModelState.IsValid)
            {
                
                _calendarQueryService.RemoveWidgetGroups(widgetId);

                foreach (var group in widgetGroups)
                {
                    //var itemGroup = group.Split('~');
                    
                    //var color = itemGroup[2];
                    //var title = itemGroup[3];
                    
                    _calendarQueryService.SaveWidgetGroup(new CalendarWidgetEventGroup
                    {
                        WidgetId = widgetId,
                        EventGroupId = group,
                        //Color = color,
                        Id = Guid.NewGuid().ToString("N")
                       // Title = title
                    });
                }

                return Ok();

            }

            return BadRequest(ModelState);
            
        }


        [Authorize]
        [HttpPost, Route("/api/content/calendar/upcomingevents/savewidgetgroups")]
        public IActionResult UpdateUpcomingEventsWidgetCategories(UpcomingEventsGroupSubmissionViewModel model)
        {
            if (model.Groups != null && ModelState.IsValid)
            {
                var categoryIds = model.Groups.Split(new char[] { ',' });

                _calendarQueryService.SetUpcomingEventsGroups(model.WidgetId, categoryIds);

                return Ok(model);
            }
            else if (model.Groups == null && model.WidgetId != null)
            {
                _calendarQueryService.ClearUpcomingEventsCategories(model.WidgetId);

                return Ok(model);
            }

            return BadRequest(ModelState);
        }


        [Authorize]
        [HttpDelete, Route("/api/content/calendar/deletecalendarevent")]
        public IActionResult RemoveEvent(string eventId)
        {
            Ensure.NotNull(eventId);

            //remove event from all groups
            _calendarQueryService.RemoveEventGroups(eventId);

            _calendarQueryService.DeleteEvent(eventId);

            return Ok();
        }
    }
}
