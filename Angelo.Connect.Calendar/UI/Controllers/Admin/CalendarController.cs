using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Calendar.Data;
using Angelo.Connect.Calendar.Services;
using System.Text;
using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Calendar.UI.Controllers
{
    
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class CalendarController : Controller
    {
        private SiteContext _siteContext;
        private CalendarDbContext _calendarDbContext;
        private ConnectDbContext _connectDbContext;
        private CalendarWidgetService _calendarWidgetService;
        private CalendarQueryService _calendarQueryService;

        private string[] DayOfTheWeek = { "SU", "MO", "TU", "WE", "TH", "FR", "SA" };

        public CalendarController
        (
            SiteContext siteContext, 
            ConnectDbContext connectDbContext,
            CalendarDbContext calendarDbContext,
            CalendarWidgetService calendarWidgetService,
            CalendarQueryService calendarQueryService
        //BlogDocumentService blogDocumentService,
        //IFolderManager<BlogPost> blogFolderManager,
        //RawHtmlService rawHtmlService
        )
        {
            _siteContext = siteContext;
            _calendarDbContext = calendarDbContext;
            _connectDbContext = connectDbContext;
            _calendarWidgetService = calendarWidgetService;
            _calendarQueryService = calendarQueryService;
            //_blogDocumentService = blogDocumentService;
            //_blogFolderManager = blogFolderManager;
            //_rawHtmlService = rawHtmlService;
        }

        [Authorize]
        [HttpGet, Route("/sys/content/calendar/edit")]
        public IActionResult EditPost(string id)
        {
            var model = _calendarQueryService.GetEventById(id);
            if (model == null)
            {
                model = _calendarQueryService.GetDefaultModel();
            }

            ViewData["masterId"] = "123456";

            return View("~/Views/Admin/Calendar/EditEvent.cshtml", model);
        }

        
        [HttpGet, Route("/sys/content/calendar/createoutlookevent")]
        public IActionResult OutlookEvent(string id)
        {
            var model = _calendarQueryService.GetEventById(id);
            if (model == null)
            {
                return null;
            }

            var dateCreated = model.Posted.ToUniversalTime();
            var dateStart = model.EventStart.ToUniversalTime();
            var dateEnd = model.EventEnd.ToUniversalTime();

            var icsFile = new StringBuilder();

            icsFile.AppendLine("BEGIN:VCALENDAR");
            icsFile.AppendLine("VERSION:2.0");
            icsFile.AppendLine("METHOD:PUBLISH");
            icsFile.AppendLine("CALSCALE:GREGORIAN");

            // Define the event.
            icsFile.AppendLine("BEGIN:VEVENT");
            icsFile.AppendLine("UID:" + Guid.NewGuid());
            icsFile.AppendLine("SEQUENCE:0");
            icsFile.AppendLine("CLASS:PUBLIC");
            icsFile.AppendLine("PRIORITY:5");

            // Adding a time stamp
            icsFile.Append("DTSTAMP:");
            icsFile.AppendLine(dateCreated.ToString("yyyyMMddTHHmmssZ"));

            // Adding created date
            icsFile.Append("CREATED:");
            icsFile.AppendLine(dateCreated.ToString("yyyyMMddTHHmmssZ"));

            // Adding the start date time
            icsFile.Append("DTSTART;TZID=\"Central Standard Time\":");
            icsFile.AppendLine(dateStart.ToString("yyyyMMddTHHmmssZ"));

            // Adding the end date time
            icsFile.Append("DTEND;TZID=\"Central Standard Time\":");
            icsFile.AppendLine(dateEnd.ToString("yyyyMMddTHHmmssZ"));

            // Adding the summary and content
            icsFile.Append("SUMMARY:");
            icsFile.AppendLine(model.Title);

            //if (this.Appointment.OfficeInfo != null && this.Appointment.OfficeInfo.Address != null)
            //{
                icsFile.Append("LOCATION:");
                icsFile.AppendLine("");

                icsFile.Append("DESCRIPTION:");
                icsFile.AppendFormat(model.Description ?? "");
                icsFile.AppendLine();

            //icsFile.Append("X-ALT-DESC;FMTTYPE=text/html:");
            //icsFile.AppendFormat(IcsFileHtmlDesc, this.Address, this.Appointment.OfficeInfo.Phone, this._redirectLink);
            //icsFile.AppendLine();
            //}
            //ADD RECURRENCY
            //RRULE:FREQ=WEEKLY;COUNT=112;BYDAY=MO,TU,WE,TH,FR
            if (model.IsRecurrent)
            {
                var rRule = "RRULE:";
                switch (model.RecurrenceDetails.Frequency)
                {
                    case "Daily":
                        rRule += "FREQ=DAILY;";
                        break;
                    case "Weekly":
                        rRule += "FREQ=WEEKLY;";
                        if(model.RecurrenceDetails.DaysOfWeek != null)
                        {
                            var days = string.Empty;
                            foreach (var day in model.RecurrenceDetails.DaysOfWeek.Split(','))
                            {
                                days += DayOfTheWeek[int.Parse(day)] + ",";
                            }
                            rRule += $"BYDAY={days.Substring(0,days.Length - 1)};";
                        }
                        //BYDAY=MO,TU,WE
                        break;
                    case "Montly":
                        rRule += "FREQ=MONTHLY;";
                        rRule += $"BYMONTHDAY={model.RecurrenceDetails.DayOfMonth};";
                        break;
                    case "Yearly":
                        rRule += "FREQ=YEARLY;";
                        rRule += $"BYMONTHDAY={model.RecurrenceDetails.DayOfMonth};";
                        rRule += $"BYMONTH={model.RecurrenceDetails.Months.ToString()};";
                        break;
                    default:
                        break;
                }
                rRule += $"INTERVAL={model.RecurrenceDetails.Interval.ToString()};";
                if (model.RecurrenceDetails.Count != null)
                {
                    rRule += $"COUNT={model.RecurrenceDetails.Count.ToString()};";
                }
                else if (model.RecurrenceDetails.EndDate != null)
                {
                    rRule += $"UNTIL={model.RecurrenceDetails.EndDate.Value.ToString("yyyyMMddTHHmmssZ")};";
                }
                icsFile.AppendLine(rRule);
            }

            // ADD ALARM
            icsFile.AppendLine("BEGIN:VALARM");
            icsFile.AppendLine("TRIGGER:-PT30M");
            icsFile.AppendLine("ACTION:DISPLAY");
            icsFile.AppendLine("DESCRIPTION:Reminder");
            icsFile.AppendLine("END:VALARM");

            // END EVENT
            icsFile.AppendLine("END:VEVENT");
            icsFile.AppendLine("END:VCALENDAR");

            var bytes = Encoding.UTF8.GetBytes(icsFile.ToString());

            return this.File(bytes, "text/calendar", "apt.ics");
        }


        public IActionResult DisplayEvent(string eventId, string day, string hour)
        {

            ViewData["eventId"] = eventId;
            ViewData["day"] = day;
            ViewData["hour"] = hour;
            return PartialView("~/Views/Admin/Calendar/DisplayEvent.cshtml");
        }

        [Authorize]
        public IActionResult ShowEventEdit(string id, string day, string hour)
        {
            ViewData["id"] = id;
            ViewData["day"] = day;
            ViewData["hour"] = hour;
            return PartialView("~/Views/Admin/Calendar/EditEvent.cshtml");
        }

        [Authorize]
        public IActionResult EventEdit(string id, string day, string hour)
        {
            ViewData["id"] = id;
            ViewData["day"] = day;
            ViewData["hour"] = hour;

            return PartialView("~/Views/Admin/Calendar/EditEvent.cshtml");
        }



        [HttpGet, Route("/sys/calendar/event/{id}")]
        public IActionResult ViewEvent(string id, [FromQuery] string version = null, [FromQuery] bool preview = false)
        {
            var model = _calendarQueryService.GetEventById(id);

            // if (blogPost.IsPrivate && !_blogSecurity.AuthorizeForRead(blogPost))
            //     return Unauthorized();

            var settings = new ShellSettings(model.Title);

            // show toolbar if user is authorized (unless in preview mode)
            //preview = false;
            //if (_blogSecurity.AuthorizeForEdit(blogPost) && !preview)
            //{
            //    settings.Toolbar = new ToolbarSettings("~/UI/Views/Public/BlogPostToolbar.cshtml", blogPost);
            //}

            var bindings = new ContentBindings
            {
                ContentType = typeof(CalendarEvent).Name,
                ContentId = id,
                VersionCode = "",
                ViewPath = "~/Views/Admin/Calendar/ViewEventPublic.cshtml",
                ViewModel = model,
            };

            return this.MasterPageView(bindings, settings);
        }
    }
}
