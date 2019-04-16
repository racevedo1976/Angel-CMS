using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

using Angelo.Identity.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class UserProfileViewModel
    {
        public string DirectoryId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "User Name")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Confirmed")]
        public bool EmailConfirmed { get; set; }

        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Phone Number Confirmed")]
        public bool PhoneNumberConfirmed { get; set; }

        [Display(Name = "Wireless Provider")]
        public string WirelessProviderId { get; set; }

        [Display(Name = "Is Locked Out")]
        public bool IsLockedOut { get; set; }

        [Display(Name = "Membership Disabled")]
        public bool MembershipDisabled { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get {
                return (FirstName + " " + LastName).Trim();
            }
        }

        [Display(Name = "Site")]
        public IList<Role> SiteRoles { get; set; }

        [Display(Name = "Client")]
        public IList<Role> ClientRoles { get; set; }

        [Display(Name = "Corp")]
        public IList<Role> CorpRoles { get; set; }

        public List<UserClaim> Permissions { get; set; }
    }
}
