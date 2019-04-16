using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Assignments.UI.ViewModels
{
    public class AssignmentCategoryViewModel
    {
        public string Id { get; set; }
        public OwnerLevel OwnerLevel { get; set; }
        public string OwnerId { get; set; }

        [Display(Name = "Title.Name", ShortName = "Title.ShortName")]
        [Required(ErrorMessage = "Title.Error.Required")]
        public string Title { get; set; }
    }
}
