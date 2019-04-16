using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Views.ViewModel;

namespace Angelo.Connect.Calendar.Models
{
    public class CalendarResultViewModel
    {
        public int success { get; set; }
        public IList<EventViewModel> result {get; set;}
    }

    public class EventViewModel2
    {
        public string id { get; set; }
        public string title { get; set; }
        public string url { get; set; }
        //public int class { get; set; }
        public double start { get; set; }
        public double end { get; set; }
        public string description { get; set; }
        public string @class { get; set; }
        public string phone { get; set; }

        //"title": "This is warning class event with very long title to check how it fits to event in day view",
        //"url": "http://www.example.com/",
        //"class": "event-warning",
        //"start": "1362938400000",
        //"end":   "1363197686300"
    }

    
}
