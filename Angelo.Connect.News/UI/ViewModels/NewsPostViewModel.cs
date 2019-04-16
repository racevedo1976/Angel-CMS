using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

using Angelo.Connect.Models;
using Angelo.Connect.News.Models;
using Angelo.Connect.News.UI.ViewModels.Validation;
using Angelo.Connect.Security;

namespace Angelo.Connect.News.UI.ViewModels
{
    public class NewsPostViewModel
    {
        public string Id { get; set; }
        public string ContentTreeId { get; set; }
        public string VersionCode { get; set; }
        public string VersionLabel { get; set; }

        public string Title { get; set; }
        public string Image { get; set; }
        [RequiredCaption]
        public string Caption { get; set; }
        public string Excerp { get; set; }
        public string Content { get; set; }
        public DateTime Posted { get; set; }
        public string PostedStr { get; set; }
        public bool IsPrivate { get; set; }

        public IEnumerable<NewsCategory> Categories { get; set; }

        public IEnumerable<SecurityClaimConfig> PostPrivacyConfig { get; set; }
    }
}
