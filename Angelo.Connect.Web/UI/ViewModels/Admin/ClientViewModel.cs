using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;
using Angelo.Connect.Models;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientViewModel : IMappableViewModel
    {
        [Display(Name = "Client Id", ShortName = "Id")]
        public string Id { get; set; }

        [Display(Name = "Tenant Key", ShortName = "Tenant Key")]
        [Required(ErrorMessage = "Tennant Key is required.")]
        public string TenantKey { get; set; }

        [Display(Name = "Client Name", ShortName = "Name")]
        [StringLength(maximumLength: 50, ErrorMessage = "Name has exceeded the maximum length")]
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }

        [Display(Name = "Client's Preferred Name", ShortName = "Preferred Name")]
        [StringLength(maximumLength: 50, ErrorMessage = "Preferred Name has exceeded the maximum length")]
        [Required(ErrorMessage = "Preferred Name is required.")]
        public string PreferredName { get; set; }

        [Display(Name = "Client's Short Name", ShortName = "Short Name")]
        [StringLength(maximumLength: 50, ErrorMessage = "Short Name has exceeded the maximum length")]
        [Required(ErrorMessage = "Short Name is required")]
        public string ShortName { get; set; }

        [Display(Name = "Address - First Line", ShortName = "Address")]
        [StringLength(maximumLength: 100, ErrorMessage = "Address has exceeded the maximum length")]
        public string Address1 { get; set; }

        [Display(Name = "Address - Second Line", ShortName = "Address")]
        [StringLength(maximumLength: 100, ErrorMessage = "Address has exceeded the maximum length")]
        public string Address2 { get; set; }

        [Display(Name = "Client's City - Physical Location", ShortName = "City")]
        [StringLength(maximumLength: 50, ErrorMessage = "City has exceeded the maximum length")]
        //[Required(ErrorMessage = "City.Error.Required")]
        public string City { get; set; }

        [Display(Name = "Client's State - Physical Location", ShortName = "State")]
        [StringLength(maximumLength: 50, ErrorMessage = "State has exceeded the maximum length")]
        //[Required(ErrorMessage = "State is required")]
        public string State { get; set; }

        [Display(Name = "Postal Code", ShortName = "Postal Code")]
        [StringLength(maximumLength: 20, ErrorMessage = "Postal Code has exceeded the maximum length")]
        public string PostalCode { get; set; }

        [Display(Name = "Client's Country", ShortName = "Country")]
        [StringLength(maximumLength: 50, ErrorMessage = "Country has exceeded the maximum length")]
        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Display(Name = "Client Status", ShortName = "Status")]
        public bool Active { get; set; }

        [Display(Name = "Notes", ShortName = "Notes")]
        [StringLength(maximumLength: 4000, ErrorMessage = "Notes has exceeded the maximum length")]
        public string Notes { get; set; }

        [Display(Name = "Client's Anniversary Date", ShortName = "Anniversary Date")]
        public DateTime AnniversaryDate { get; set; }

        [Display(Name = "Security Pool Id", ShortName = "Pool Id")]
        public string SecurityPoolId { get; set; }

        // Mapping method
        public void CopyFrom(object data)
        {
            if (data is Client)
            {
                var client = data as Client;
                Id = client.Id;
                Name = client.Name;
                PreferredName = client.PreferredName;
                ShortName = client.ShortName;
                Address1 = client.Address1;
                Address2 = client.Address2;
                City = client.City;
                State = client.State;
                Country = client.Country;
                PostalCode = client.PostalCode;
                Notes = client.Notes;
                Active = client.Active;
                AnniversaryDate = client.AnniversaryDate;
            }
            else
                throw new NotImplementedException();
        }

        // Mapping method
        public void CopyTo(object data)
        {
            throw new NotImplementedException();
        }
    }
}
