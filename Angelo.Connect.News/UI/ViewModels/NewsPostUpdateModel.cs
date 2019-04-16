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
    public class NewsPostUpdateModel
    {
        public string Id { get; set; }
        public string VersionCode { get; set; }
        public string ContentTreeId { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }

        [RequiredCaption]
        public string Caption { get; set; }
        public string Excerp { get; set; }

        public DateTime Posted { get; set; }
        public string UserId { get; set; }

        public bool IsPrivate { get; set; }

        public string NewVersionLabel { get; set; }

        public bool ShouldPublish { get; set; }

        public string CategoryIds { get; set; }
    }
}
