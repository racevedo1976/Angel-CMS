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
    public class BlogPostSettingsUpdateModel
    {
        public string BlogPostId { get; set; }

        public bool IsPrivate { get; set; }

        public string CategoryIds { get; set; }
    }
}
