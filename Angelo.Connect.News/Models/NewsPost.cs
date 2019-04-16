using System;
using System.Collections.Generic;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;

namespace Angelo.Connect.News.Models
{
    public class NewsPost : IContent
    {
        public NewsPost()
        {
            Categories = new List<NewsPostCategory>();
            //ConnectionGroups = new List<NewsPostConnectionGroup>();
            Tags = new List<NewsPostTag>();
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

        public List<NewsPostCategory> Categories { get; set; }
        //public List<NewsPostConnectionGroup> ConnectionGroups { get; set; }
        public List<NewsPostTag> Tags { get; set; }
    }
}
