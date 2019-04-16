using System;

namespace Angelo.Connect.Assignments.UI.ViewModels
{
    public class AssignmentListItemViewModel
    {
        public string Id { get; set; }
        public DateTime CreatedDT { get; set; }
        public DateTime DueDT { get; set; }
        public string TimeZoneId { get; set; }
        public string Status { get; set; }
        public string Title { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
    }
}
