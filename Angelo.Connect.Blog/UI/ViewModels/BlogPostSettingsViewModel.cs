using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Angelo.Connect.Models;
using Angelo.Connect.Blog.Models;
using Angelo.Connect.Blog.UI.ViewModels.Validation;
using Angelo.Connect.Security;

namespace Angelo.Connect.Blog.UI.ViewModels
{
    public class BlogPostSettingsViewModel
    {
        public string BlogPostId { get; set; }

        public bool IsPrivate { get; set; }

        public IEnumerable<BlogCategory> Categories { get; set; }

        public IEnumerable<ContentVersion> Versions { get; set; }

        public IEnumerable<SecurityClaimConfig> PostPrivacyConfig { get; set; }
    }
}
