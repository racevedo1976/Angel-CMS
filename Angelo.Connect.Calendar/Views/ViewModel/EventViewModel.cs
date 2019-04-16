using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Calendar.Views.ViewModel
{
    /// <summary>
    ///     Event object use by FullCalendar from fullCalendar.io
    /// </summary>
    public class EventViewModel
    {

        public string id { get; set; }      //String/Integer.Optional Uniquely identifies the given event. Different instances of repeating events should all have the same id.  
        [Required]
        public string title { get; set; }   //String. Required. The text on an event's element  

        public bool allDay { get; set; }  //true or false. Optional. Whether an event occurs at a specific time-of-day.This property affects whether an event's time is shown. Also, in the agenda views, determines if it is displayed in the "all-day" section. If this value is not explicitly specified, allDayDefault will be used if it is defined. If all else fails, FullCalendar will try to guess. If either the start or end value has a "T" as part of the ISO8601 date string, allDay will become false. Otherwise, it will be true. Don't include quotes around your true/false. This value is a boolean, not a string!  

        public string start { get; set; }   //The date/time an event begins.Required.A Moment-ish input, like an ISO8601 string. Throughout the API this will become a real Moment object.  
        
        public string end { get; set; }     //The exclusive date/time an event ends.Optional.A Moment-ish input, like an ISO8601 string. Throughout the API this will become a real Moment object. It is the moment immediately after the event has ended. For example, if the last full day of an event is Thursday, the exclusive end of the event will be 00:00:00 on Friday!  
        
        public string url { get; set; }     //String.Optional.A URL that will be visited when this event is clicked by the user. For more information on controlling this behavior, see the eventClick callback.  

        public string className { get; set; } //String/Array. Optional. A CSS class (or array of classes) that will be attached to this event's element.  

        public bool editable { get; set; } //true or false. Optional. Overrides the master editable option for this single event.  

        public bool startEditable { get; set; } //true or false. Optional. Overrides the master eventStartEditable option for this single event.  

        public bool durationEditable { get; set; } //true or false. Optional. Overrides the master eventDurationEditable option for this single event.  

        public bool resourceEditable { get; set; } //true or false. Optional. Overrides the master eventResourceEditable option for this single event.  

        public string rendering { get; set; } //Allows alternate rendering of the event, like background events. Can be empty, "background", or "inverse-background"  

        public bool overlap { get; set; } //true or false. Optional. Overrides the master eventOverlap option for this single event. If false, prevents this event from being dragged/resized over other events.Also prevents other events from being dragged/resized over this event.  

        public string constraint { get; set; } //an event ID, "businessHours", object. Optional.Overrides the master eventConstraint option for this single event.  

        public string source { get; set; } //Event Source Object. Automatically populated. A reference to the event source that this event came from.
        
        public string color { get; set; } //Sets an event's background and border color just like the calendar-wide eventColor option.  

        public string backgroundColor { get; set; } //Sets an event's background color just like the calendar-wide eventBackgroundColor option.  

        public string borderColor { get; set; } //Sets an event's border color just like the the calendar-wide eventBorderColor option.  

        public string textColor { get; set; } //Sets an event's text color just like the calendar-wide eventTextColor option

        public string phone { get; set; }
        public string description { get; set; }
        public bool isRecurrent { get; set; }
        public string location { get; set; }
        public string userId { get; set; }
        public string userName { get; set; }
        public bool showOrganizerName { get; set; }
        public bool showPhoneNumber { get; set; }
    }
}
