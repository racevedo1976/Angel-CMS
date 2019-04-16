using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Interfaces;

namespace Angelo.Connect.Calendar.Models
{
    /// <summary>
    /// This class represents the manytomany relationship between 
    /// an Event and a Group
    /// </summary>
    public class CalendarEventGroupEvent
    {

        public string EventId { get; set; }
        public CalendarEvent Event { get; set; }


        public string EventGroupId { get; set; }
       // public CalendarEventGroup EventGroup { get; set; }

    }
}