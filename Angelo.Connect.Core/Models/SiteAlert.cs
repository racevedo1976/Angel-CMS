using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit.Text;

namespace Angelo.Connect.Models
{
    public class SiteAlert
    {
        public string Id { get; set; }
        public string VersionCode { get; set; }
        public string SiteId { get; set; }
        public string UserId { get; set; }
        public string ContentTreeId { get; set; }
        public string Title { get; set; }
        //public string AlertBody { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime Posted { get; set; }
        public ContentStatus Status { get; set; }
        
    }
}
