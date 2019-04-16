using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;

namespace Angelo.Connect.Models
{
    public class Client
    {
        public Client()
        {
            ClientProductApps = new List<ClientProductApp>();
            SiteCollections = new List<SiteCollection>();
            Sites = new List<Site>();
        }

        public string Id { get; set; }
        public string TenantKey { get; set; }
        public string Name { get; set; }
        public string PreferredName { get; set; }
        public string ShortName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Notes { get; set; }
        public bool Active { get; set; } = true;
        [DataType(DataType.Date, ErrorMessage = "Date Only - yyyy/mm/dd")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AnniversaryDate { get; set; } = DateTime.UtcNow;
       
        public string SecurityPoolId { get; set; }

        public ICollection<ClientProductApp> ClientProductApps { get; set; }
        public ICollection<SiteCollection> SiteCollections { get; set; }
        public ICollection<Site> Sites { get; set; }

        /*
         Address Model (type: billing, physical)
         Time Zone
         Contact Model (type: billing, primary, legal, IT)
         Sales Information/Portal for future versions?
         Client-wide settings
         */
    }
}
