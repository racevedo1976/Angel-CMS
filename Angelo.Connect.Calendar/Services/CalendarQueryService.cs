using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Data;
using Angelo.Connect.Calendar.Models;
using Microsoft.EntityFrameworkCore;
using Angelo.Connect.Calendar.Interface;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Calendar.Services
{
    public class CalendarQueryService
    {
        private CalendarDbContext _calendarDbContext;
        private IEnumerable<IRecurringEvent> _recurringEventProviders;
        private IEnumerable<ISecurityGroupProvider> _groupProviders;

        public CalendarQueryService(CalendarDbContext calendarDbContext, 
            IEnumerable<IRecurringEvent> recurringEventProviders,
            IEnumerable<ISecurityGroupProvider> groupProviders)
        {
            _calendarDbContext = calendarDbContext;
            _recurringEventProviders = recurringEventProviders;
            _groupProviders = groupProviders;
        }

        public IEnumerable<CalendarEvent> GetAll()
        {
            return _calendarDbContext.CalendarEvents.ToList();
        }

        public CalendarEvent GetEventById(string id)
        {
            return _calendarDbContext
                .CalendarEvents
                .Include(e =>e.EventGroupEvents)
                .Include(e => e.RecurrenceDetails)
                .FirstOrDefault(x => x.EventId == id);
        }


        public IEnumerable<CalendarEvent> GetEventsByDate(DateTime eventsFrom, DateTime eventsTo)
        {
            return _calendarDbContext.CalendarEvents
                .Include(x => x.RecurrenceDetails)
                .Where(x => !(x.EventStart >= eventsTo) && !(x.EventEnd <= eventsFrom));
        }

        public List<CalendarEvent> GetEventsByDate(DateTime eventsFrom,DateTime eventsTo, List<string> groups)
        {
            return _calendarDbContext
                  .CalendarEvents
                  .Include(e => e.EventGroupEvents)
                  .Include(e => e.RecurrenceDetails)
                  .Where(x => x.EventGroupEvents.Any(p => groups.Contains(p.EventGroupId)) &&
                                (   (!(x.EventStart >= eventsTo) && !(x.EventEnd <= eventsFrom)) ||                                  //date range crosses event dates
                                    ((x.IsRecurrent) && (x.RecurrenceDetails.Count == null && x.RecurrenceDetails.EndDate == null))  //if recurrent, has no end end date
                                )
                        )
                  .ToList();
        }

        public IEnumerable<CalendarEvent> GetEventsByDate(string widgetId, DateTime eventsFrom, DateTime eventsTo)
        {
            //first figure out which groups are we pulling events from based on the widget
            var groupsForWidget = _calendarDbContext.CalendarWidgetEventGroups.Where(x => x.WidgetId == widgetId)
                .Select(x => x.EventGroupId)
                .ToList();
            var events = new List<CalendarEvent>();

            //This will figure out all groups where the user is a member
            List<string> groupsForUser = new List<string>();
            //groupsForUser = GetEventGroupsByUserId().Select(x => x.Id).ToList();
            
            //Merge both, widget assigned groups and user groups. 
            var mergedGroups = groupsForWidget.Union(groupsForUser).ToList();

            //Merge groups pulled from the users and widget groups from .provider groups by user
            if (mergedGroups.Any())
            {
                events = GetEventsByDate(eventsFrom, eventsTo, mergedGroups);
            }

            //**** this is for get events not in groups ****
            //So far, rule is that events not in a group will not be shown in the instance.
            //Mostly only instance based groups - events only.
            //I will keep this just in case, if we need to add more events that are not in specific group
            //var eventList = GetEventsByDate(eventsFrom, eventsTo).Where(x => !events.Any(x2 => x2.EventId == x.EventId)).ToList();
            //return ResolveRecurrence(events.Union(eventList).Distinct(), eventsFrom, eventsTo);

            return ResolveRecurrence(events, eventsFrom, eventsTo);
        }

        public IEnumerable<CalendarEvent> GetUpcomingEventsByDate(string widgetId, DateTime eventsFrom, DateTime eventsTo)
        {
            //first figure out which groups are we pulling events from based on the widget
            var groupsForWidget = _calendarDbContext.UpcomingEventsWidgetEventGroups.Where(x => x.WidgetId == widgetId)
                .Select(x => x.EventGroupId)
                .ToList();
            var events = new List<CalendarEvent>();

            //This will figure out all groups where the user is a member
            List<string> groupsForUser = new List<string>();
            //groupsForUser = GetEventGroupsByUserId().Select(x => x.Id).ToList();

            //Merge both, widget assigned groups and user groups. 
            var mergedGroups = groupsForWidget.Union(groupsForUser).ToList();

            //Merge groups pulled from the users and widget groups from .provider groups by user
            if (mergedGroups.Any())
            {
                events = GetEventsByDate(eventsFrom, eventsTo, mergedGroups);
            }

            return ResolveRecurrence(events, eventsFrom, eventsTo);
        }

        /*
        public IQueryable<CalendarEvent> GetEventsByWidget(string widgetId)
        {
            var widgetCats = _calendarDbContext.UpcomingEventsWidgetEventGroups.Where(x => x.WidgetId == widgetId);

            var calendarEvents = _calendarDbContext.CalendarEvents
                .Where(x => x.EventGroupEvents.Any(y => widgetCats.Any(z => z.EventGroupId == y.EventGroupId))
                );

            return calendarEvents;
        }
        */

        public CalendarEvent SaveEvent(CalendarEvent newCalendarEvent)
        {
            _calendarDbContext.CalendarEvents.Add(newCalendarEvent);
            _calendarDbContext.SaveChanges();

            return newCalendarEvent;
        }

        public void UpdateEvent(CalendarEvent calendarEvent)
        {
            _calendarDbContext.Attach(calendarEvent);
            _calendarDbContext.Entry(calendarEvent).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            
            _calendarDbContext.SaveChanges();
            
        }

        public void DeleteEvent(string eventId)
        {
            var ev = GetEventById(eventId);
            if (ev != null)
            {
                _calendarDbContext.Remove(ev);
                _calendarDbContext.SaveChanges();
            }
        }

        public CalendarEvent GetDefaultModel()
        {
            var newId = Guid.NewGuid().ToString();
            var newStartDate = RoundUp(DateTime.Now, TimeSpan.FromMinutes(30));
            return new CalendarEvent
            {
                EventId = newId,
                EventStart = newStartDate,
                EventEnd = newStartDate.AddHours(1),
                Title = "",
                DocumentId = Guid.NewGuid().ToString("N"),
                Posted = DateTime.Now,
                IsRecurrent= false,
                AllDayEvent =false,
                LinkTarget = "_self",
                RecurrenceDetails = new CalendarEventRecurrence()
                {
                    Frequency ="Daily",
                    Interval = 1,
                    EventId = newId,
                    Id = Guid.NewGuid().ToString()
        }
            };
            //throw new NotImplementedException();
        }

        private DateTime RoundUp(DateTime dt, TimeSpan d)
        {
            return new DateTime((dt.Ticks + d.Ticks - 1) / d.Ticks * d.Ticks, dt.Kind);
        }
        //public IList<CalendarEventGroup> GetEventCategories(string eventId)
        //{
        //    return _calendarDbContext.CalendarEventCategories.Where(x => x.EventId == eventId).ToList();

        //}

        //internal void RemoveEventCategories(string eventId)
        //{
        //    _calendarDbContext.CalendarEventCategories.RemoveRange(_calendarDbContext.CalendarEventCategories.Where(x => x.EventId == eventId));
        //    _calendarDbContext.SaveChanges();
        //}

        //internal void SaveEventCategory(CalendarEventGroup eventCategory)
        //{
        //    _calendarDbContext.CalendarEventCategories.Add(eventCategory);
        //    _calendarDbContext.SaveChanges();
        //}


        // Event Groups services
        public IList<CalendarEventGroup> GetEventGroupsByUserId(string userId)
        {
            return _calendarDbContext.CalendarEventGroups.Where(x => x.UserId == userId).ToList();
        }

        public CalendarEventGroup CreateEventGroup(CalendarEventGroup eventGroup)
        {
            var existingGroup = _calendarDbContext.CalendarEventGroups.FirstOrDefault(x => x.Title == eventGroup.Title && x.UserId == eventGroup.UserId);
            if (existingGroup == null)
            {
                _calendarDbContext.CalendarEventGroups.Add(eventGroup);
                _calendarDbContext.SaveChanges();
            }else
            {
                eventGroup = existingGroup;
            }
            return eventGroup;
           
        }

        // Event Groups services
        public IList<CalendarEventGroup> GetSharedEventGroups(IEnumerable<string> eventGroupIds, string userId)
        {
            return _calendarDbContext.CalendarEventGroups.Where(x => eventGroupIds.Contains(x.EventGroupId) && x.UserId != userId).ToList();

        }



        //public void SaveEventGroups(CalendarEventGroup eventGroup)
        //{
        //    _calendarDbContext.Add(eventGroup);
        //    _calendarDbContext.SaveChanges();
        //}

        public CalendarEventGroup GetEventGroup(string eventGroupId)
        {
            return _calendarDbContext.CalendarEventGroups.FirstOrDefault(x => x.EventGroupId == eventGroupId);

        }



        internal IList<CalendarEventTag> GetEventTags(string id)
        {
            return _calendarDbContext.CalendarEventTags.Where(x => x.EventId == id).ToList();
        }

        internal void RemoveEventTags(string eventId)
        {
            _calendarDbContext.CalendarEventTags.RemoveRange(_calendarDbContext.CalendarEventTags.Where(x => x.EventId == eventId));
            _calendarDbContext.SaveChanges();
        }

        internal void SaveEventTags(CalendarEventTag calendarEventTag)
        {
            _calendarDbContext.CalendarEventTags.Add(calendarEventTag);
            _calendarDbContext.SaveChanges();
        }

        internal void RemoveEventGroups(string eventId)
        {
            var thisEvent = GetEventById(eventId);
            foreach (var item in thisEvent.EventGroupEvents.ToList())
            {
                thisEvent.EventGroupEvents.Remove(item);
            }
           
            _calendarDbContext.SaveChanges();
        }

        internal void SaveEventGroup(string eventId, string eventGroupId)
        {
            var thisEvent = GetEventById(eventId);
            //var thisGroup = GetEventGroup(eventGroupId);

            thisEvent.EventGroupEvents.Add(new CalendarEventGroupEvent
            {
                Event = thisEvent,
                //EventGroup = thisGroup,
                EventGroupId = eventGroupId
            });

            _calendarDbContext.SaveChanges();
        }


        //widget Groups
        
        internal List<CalendarWidgetEventGroup> GetWidgetGroups(string id)
        {
            return _calendarDbContext.CalendarWidgetEventGroups.Where(x => x.WidgetId == id).ToList();
        }

        internal void RemoveWidgetGroups(string widgetId)
        {
            _calendarDbContext.CalendarWidgetEventGroups.RemoveRange(_calendarDbContext.CalendarWidgetEventGroups.Where(x => x.WidgetId == widgetId));
            _calendarDbContext.SaveChanges();
        }

        internal void SaveWidgetGroup(CalendarWidgetEventGroup groupItem)
        {
            _calendarDbContext.CalendarWidgetEventGroups.Add(groupItem);
            _calendarDbContext.SaveChanges();
        }

        internal List<UpcomingEventsWidgetEventGroup> GetUpcomingEventWidgetGroups(string id)
        {
            return _calendarDbContext.UpcomingEventsWidgetEventGroups.Where(x => x.WidgetId == id).ToList();
        }


        public void ClearUpcomingEventsCategories(string widgetId)
        {
            // Remove old mappings
            _calendarDbContext.UpcomingEventsWidgetEventGroups.RemoveRange(
                _calendarDbContext.UpcomingEventsWidgetEventGroups.Where(x => x.WidgetId == widgetId));

            _calendarDbContext.SaveChanges();
        }


        public void SetUpcomingEventsGroups(string widgetId, IEnumerable<string> groupIds)
        {
            // Remove old mappings
            _calendarDbContext.UpcomingEventsWidgetEventGroups.RemoveRange(
                _calendarDbContext.UpcomingEventsWidgetEventGroups.Where(x => x.WidgetId == widgetId));

            _calendarDbContext.SaveChanges();

            // Add new mappings
            foreach (var groupId in groupIds)
            {
                if (!string.IsNullOrEmpty(groupId))
                {
                    _calendarDbContext.UpcomingEventsWidgetEventGroups.Add(new UpcomingEventsWidgetEventGroup
                    {
                        Id = KeyGen.NewGuid(),
                        WidgetId = widgetId,
                        EventGroupId = groupId,
                    });
                }
            }

            _calendarDbContext.SaveChanges();
        }

        //recurrence resolvers
        private IEnumerable<CalendarEvent> ResolveRecurrence(IEnumerable<CalendarEvent> currentEvents, DateTime from, DateTime to)
        {
            List<CalendarEvent> eventList = new List<CalendarEvent>();

            foreach (var thisEvent in currentEvents)
            {
                eventList.AddRange(ResolveRecurrence(thisEvent, from, to));
            }

            return eventList;
        }
        private IEnumerable<CalendarEvent> ResolveRecurrence(CalendarEvent currentEvent, DateTime from, DateTime to)
        {
            List<CalendarEvent> events = new List<CalendarEvent>();
            if (!currentEvent.IsRecurrent)
            {
                events.Add(currentEvent);
            }else
            {
                //selecte provider and resolve
                var freqProvider = _recurringEventProviders.FirstOrDefault(x => x.FrequencyType() == currentEvent.RecurrenceDetails.Frequency);
                freqProvider.EventDetail = currentEvent;
                freqProvider.From = from;
                freqProvider.To = to;
                events.AddRange(freqProvider.ResolveRecurringEvents());

            }

            return events;
        }
    }
}
