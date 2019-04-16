using Angelo.Connect.Calendar.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Models;

namespace Angelo.Connect.Calendar.Services
{
    public class WeeklyEventRecurrenceProvider : IRecurringEvent
    {
        public CalendarEvent EventDetail { get; set; }

        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string FrequencyType()
        {
            return "Weekly";
        }
      

        private static DateTime[] WeekDaysRange(int Year, int WeekNumber)
        {
            DateTime start = new DateTime(Year, 1, 1).AddDays(7 * WeekNumber);
            start = start.AddDays(-((int)start.DayOfWeek));
            return Enumerable.Range(0, 7).Select(num => start.AddDays(num)).ToArray();
        }
        private DateTime[] WeekDaysListForDate(DateTime thisDate)
        {
            var dayOfWeek = (int)thisDate.DayOfWeek;
            var begginingOfWeek = thisDate.AddDays(-dayOfWeek);
            return Enumerable.Range(0, 7).Select(num => begginingOfWeek.AddDays(num)).ToArray();
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
            var daysOfWeekSelected = EventDetail.RecurrenceDetails.DaysOfWeek.Split(',');
            if(daysOfWeekSelected[daysOfWeekSelected.Count() - 1] == "")
            {
                daysOfWeekSelected = daysOfWeekSelected.Take(daysOfWeekSelected.Count() - 1).ToArray();
            }
           
            DateTime? newEventDate = resolvedRecurringStartDate.AddDays(-(int)resolvedRecurringStartDate.DayOfWeek); //start from the Sunday
            List<CalendarEvent> newEvents = new List<CalendarEvent>();
            while (newEventDate <= resolvedRecurringEndDate)
            {
                
                foreach (var dayOfWeek in daysOfWeekSelected)
                {
                    var actualSelectedDay = newEventDate.Value.AddDays(int.Parse(dayOfWeek));
                    if (actualSelectedDay <= resolvedRecurringEndDate)
                    {
                        newEvents.Add(new CalendarEvent
                        {
                            
                            Title = EventDetail.Title,
                            AllDayEvent = EventDetail.AllDayEvent,
                            Description = EventDetail.Description,
                            EventStart = DateTime.Parse(actualSelectedDay.ToString("MM/dd/yyyy") + " " + actualSelectedDay.ToString("h:mm tt")),
                            EventEnd = DateTime.Parse(actualSelectedDay.ToString("MM/dd/yyyy") + " " + resolvedRecurringEndDate.Value.ToString("h:mm tt")),
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
                    }
                }
                

                newEventDate = newEventDate.Value.AddDays(7 * EventDetail.RecurrenceDetails.Interval);
            }
            return newEvents;
        }

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
                var daysOfWeekSelected = EventDetail.RecurrenceDetails.DaysOfWeek.Split(',');
                if (daysOfWeekSelected[daysOfWeekSelected.Count() - 1] == "")
                {
                    daysOfWeekSelected = daysOfWeekSelected.Take(daysOfWeekSelected.Count() - 1).ToArray();
                }
               

                //figure out amount of weeks we need to add. Round up.
                var numOfWeeks =(int)Math.Ceiling((decimal)(EventDetail.RecurrenceDetails.Count.Value / daysOfWeekSelected.Count()));

                //days to add
                var daysToAdd = (numOfWeeks * 7) * EventDetail.RecurrenceDetails.Interval;

                //figure first day of the start week (Sunday)
                //var weekStartDateSun = EventDetail.EventStart.AddDays(-(int)EventDetail.EventStart.DayOfWeek);

                //temp end date, adjustement needed
                endDate = EventDetail.EventStart.AddDays(daysToAdd);
                bool dayMatchFound = false;
                while (!dayMatchFound)
                {
                    endDate =endDate.AddDays(-1) ;
                    var newEndDateDayofTheWeek = (int)endDate.DayOfWeek;
                    if (daysOfWeekSelected.Any(x => int.Parse(x) == newEndDateDayofTheWeek))
                    {
                        dayMatchFound = true;
                    }
                }

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
    }
}
