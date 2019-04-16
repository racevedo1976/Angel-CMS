using System;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class UserViewModel
    {
        public string DirectoryId { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        [Display(Name = "Id")]
        public string Id { get; set; }

        [Display(Name = "User Name")]
        [StringLength(maximumLength: 50, ErrorMessage = "User Name must be less than {0} characters.")]
        [Required(ErrorMessage = "User Name is required.")]
        public string UserName { get; set; }

        [Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "First Name")]
        [StringLength(maximumLength: 50, ErrorMessage = "First Name must be less than {0} characters.")]
        [Required(ErrorMessage = "First Name is required.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [StringLength(maximumLength: 50, ErrorMessage = "Last Name must be less than {0} characters.")]
        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; }

        [Display(Name = "Suffix")]
        public string Suffix { get; set; }

        [Display(Name = "Display Name")]
        [StringLength(maximumLength: 50, ErrorMessage = "Display Name must be less than {0} characters.")]
        [Required(ErrorMessage = "Display Name is equired")]
        public string DisplayName { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "Gender")]
        public string Gender { get; set; }

        [Display(Name = "Email")]
        [StringLength(maximumLength: 100, ErrorMessage = "Email address must be less than {0} characters.")]
        [Required(ErrorMessage = "Email address is required")]
        public string Email { get; set; }

        [Display(Name = "Confirmed")]
        public bool EmailConfirmed { get; set; }

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

        public string LdapGuid { get; set; }
    }
}
