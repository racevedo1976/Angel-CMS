using Angelo.Connect.Abstractions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Models
{
    public class CalendarEvent : IDocument
    {
        public CalendarEvent()
        {
            
        }
        public string EventId { get; set; }
        public string DocumentId { get; set; }
        [Required]
        public string Title{ get; set; }
        public string Style { get; set; }
        public DateTime Posted { get; set; }
        public string Description { get; set; }
        public DateTime EventStart { get; set; }
        public DateTime EventEnd { get; set; }
        public string SiteId { get; set; }
        public string UserId { get; set; }
        public string Phone { get; set; }
        public bool AllDayEvent { get; set; }
        public string Url { get; set; }
        public string LinkTarget { get; set; }
        public string BackgroundColor { get; set; }
        public bool IsRecurrent { get; set; }
        public string Location { get; set; }
        public bool ShowOrganizerName { get; set; }
        public bool ShowPhoneNumber { get; set; }
        
        public CalendarEventRecurrence RecurrenceDetails { get; set; }

        public virtual ICollection<CalendarEventGroupEvent> EventGroupEvents { get; } = new List<CalendarEventGroupEvent>();

        public string Summary {
            get
            {
                var eventDetails = string.Empty;
                if (IsRecurrent)
                {
                    switch (RecurrenceDetails.Frequency)
                    {
                        case "Daily":
                            eventDetails = $"{RecurrenceDetails.Frequency} every {RecurrenceDetails.Interval} day";
                            break;
                        case "Weekly":
                            var selDayDesc = string.Empty;
                            string[] days = { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
                            if (RecurrenceDetails.DaysOfWeek == null)
                            {
                                break;
                            }
                                
                            string[] selectedDays = RecurrenceDetails.DaysOfWeek?.Split(',');
                            foreach (var selDay in selectedDays)
                            {
                                if (!string.IsNullOrEmpty(selDay))
                                    selDayDesc += days[int.Parse(selDay)] + ",";
                            }
                            eventDetails = $"{RecurrenceDetails.Frequency} every {RecurrenceDetails.Interval} week on {selDayDesc.Substring(0,selDayDesc.Length -1)}";
                            break;
                        case "Monthly":
                            eventDetails = $"{RecurrenceDetails.Frequency} every {RecurrenceDetails.Interval} month on the {RecurrenceDetails.DayOfMonth} of each month";
                            break;
                        case "Yearly":
                            eventDetails = $"{RecurrenceDetails.Frequency} every {RecurrenceDetails.Interval} year on {DateTime.Parse(RecurrenceDetails.Months + "/1/2017").ToString("MMM", CultureInfo.InvariantCulture)}  {RecurrenceDetails.DayOfMonth}";
                            break;
                       
                    }
                    // resolve event end date.
                    if (RecurrenceDetails.Count == null && RecurrenceDetails.EndDate == null)
                    {
                        eventDetails += $" with no end date.";
                    }else if (RecurrenceDetails.Count != null && RecurrenceDetails.EndDate == null)
                    {
                        eventDetails += $" for {RecurrenceDetails.Count} occurrences.";
                    }
                    else if (RecurrenceDetails.Count == null && RecurrenceDetails.EndDate != null)
                    {
                        eventDetails += $" ending {RecurrenceDetails.EndDate.Value.ToString("MMMM dd, yyyy")} .";
                    }

                }


                return eventDetails;


            }
        }
    }
}
