using Angelo.Connect.Calendar.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Models;

namespace Angelo.Connect.Calendar.Services
{
    public class YearlyEventRecurrenceProvider : IRecurringEvent
    {
        public CalendarEvent EventDetail { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public DateTime CalculateEndDate()
        {
            //Calc End Date
            DateTime endDate;
            if (EventDetail.RecurrenceDetails.EndDate == null && EventDetail.RecurrenceDetails.Count == null)
            {
                //this means there is no End Date so it should be the date range end date of searched values
                endDate = To;
            }
            else if (EventDetail.RecurrenceDetails.EndDate != null)
            {
                endDate = EventDetail.RecurrenceDetails.EndDate.Value;
            }
            else if (EventDetail.RecurrenceDetails.Count != null)
            {
                //get the start date which is the day the year the event will occur
                
                //temp end date, adjustement needed
                endDate = EventDetail.EventStart.AddYears(EventDetail.RecurrenceDetails.Count.Value * EventDetail.RecurrenceDetails.Interval);

              
                //reset the time to match endevent time
                endDate = DateTime.Parse(endDate.ToString("MM/dd/yyyy") + " " + EventDetail.EventEnd.ToString("h:mm tt"));

            }
            else
            {
                endDate = EventDetail.EventEnd;
            }

            //after figuring out the actual event end date, compare with the searched date range and
            //adjust to just the range if neeed.
            if (endDate > To)
            {
                endDate = DateTime.Parse(To.ToString("MM/dd/yyyy") + " " + endDate.ToString("h:mm tt"));
            }

            return endDate;
        }

        public string FrequencyType()
        {
            return "Yearly";
        }

        public IEnumerable<CalendarEvent> ResolveRecurringEvents()
        {
            Ensure.NotNull(EventDetail);
            Ensure.NotEqual(From, DateTime.MinValue, "The From date property is required.");
            Ensure.NotEqual(To, DateTime.MinValue, "The To date property is required.");


            DateTime? resolvedRecurringStartDate;
            DateTime? resolvedRecurringEndDate;

            resolvedRecurringEndDate = CalculateEndDate();

            //generate all recurring events until the event really ends or the end of what was searched
            var generatedRecurringEvents = GenerateRecurringEvents(EventDetail.EventStart, resolvedRecurringEndDate);

            //resolve the start of the recurring events that we only care to show based on 
            // searched range
            if (EventDetail.EventStart > From)
            {
                resolvedRecurringStartDate = EventDetail.EventStart;
            }
            else
            {
                resolvedRecurringStartDate = From;
            }


            return generatedRecurringEvents.Where(x => (x.EventStart >= resolvedRecurringStartDate && x.EventStart <= resolvedRecurringEndDate)
                                                       || (x.EventEnd >= resolvedRecurringStartDate && x.EventEnd <= resolvedRecurringEndDate)).ToList();
        }

        private IEnumerable<CalendarEvent> GenerateRecurringEvents(DateTime resolvedRecurringStartDate, DateTime? resolvedRecurringEndDate)
        {

            var newEventDate = resolvedRecurringStartDate;

            List<CalendarEvent> newEvents = new List<CalendarEvent>();
            while (newEventDate <= resolvedRecurringEndDate)
            {

                newEvents.Add(new CalendarEvent
                {
                    Title = EventDetail.Title,
                    AllDayEvent = EventDetail.AllDayEvent,
                    Description = EventDetail.Description,
                    EventStart = newEventDate,
                    EventEnd = DateTime.Parse(newEventDate.ToString("MM/dd/yyyy") + " " + resolvedRecurringEndDate.Value.ToString("h:mm tt")),
                    Posted = EventDetail.Posted,
                    Phone = EventDetail.Phone,
                    EventId = EventDetail.EventId,
                    Style = EventDetail.Style,
                    BackgroundColor = EventDetail.BackgroundColor,
                    UserId = EventDetail.UserId,
                    IsRecurrent = EventDetail.IsRecurrent,
                    Location = EventDetail.Location,
                    ShowOrganizerName = EventDetail.ShowOrganizerName,
                    ShowPhoneNumber = EventDetail.ShowPhoneNumber
                });
                
                newEventDate = newEventDate.AddYears(EventDetail.RecurrenceDetails.Interval);
            }
            return newEvents;
        }
    }
}
