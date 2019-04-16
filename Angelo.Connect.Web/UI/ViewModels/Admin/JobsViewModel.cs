using System;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class JobsViewModel
    {
        [Display(Name = "Id", ShortName = "Id")]
        public int Id { get; set; }

        [Display(Name = "Created", ShortName = "Created")]
        public DateTime Created { get; set; }

        [Display(Name = "Event Type", ShortName = "Event Type")]
        public string EventType { get; set; }

        [Display(Name = "Message", ShortName = "Message")]
        public string Message { get; set; }
    }
}
