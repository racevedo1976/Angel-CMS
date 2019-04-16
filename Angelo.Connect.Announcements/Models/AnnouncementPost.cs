using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Widgets;
using Angelo.Identity.Models;
using Angelo.Connect.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Angelo.Connect.Announcement.Models
{
    public class AnnouncementPost : IContent
    {
        public AnnouncementPost()
        {
            Categories = new List<AnnouncementPostCategory>();
            //ConnectionGroups = new List<AnnouncementPostConnectionGroup>();
            Tags = new List<AnnouncementPostTag>();
            IsActive = true;
        }

        public string Id { get; set; }
        public string VersionCode { get; set; }
        public ContentStatus Status { get; set; }
        public string ContentTreeId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Caption { get; set; }
        public string Excerp { get; set; }
        public string Content { get; set; }
        public DateTime Posted { get; set; }
        public string UserId { get; set; }
        public bool IsPrivate { get; set; }
        public bool IsActive { get; set; }
        public bool Published { get; set; }

        public bool PrivateCommentsAllowed { get; set; }
        public bool PrivateCommentsModerated { get; set; }
        public bool PublicCommentsAllowed { get; set; }
        public bool PublicCommentsModerated { get; set; }

        public List<AnnouncementPostCategory> Categories { get; set; }
        //public List<AnnouncementPostConnectionGroup> ConnectionGroups { get; set; }
        public List<AnnouncementPostTag> Tags { get; set; }
    }
}
