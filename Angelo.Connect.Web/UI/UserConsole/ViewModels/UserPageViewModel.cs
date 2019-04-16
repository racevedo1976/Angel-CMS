using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Models;
using Angelo.Connect.Security;

namespace Angelo.Connect.Web.UI.UserConsole.ViewModels
{
    public class UserPageViewModel
    {

        [Display(Name = "Id", ShortName = "Id")]
        public string Id { get; set; }

        [Display(Name = "Page Title", ShortName = "Title")]
        public string Title { get; set; }

        [Display(Name = "Url Path", ShortName = "Path")]
        public string Path { get; set; }

        [Display(Name = "Keywords", ShortName = "Keywords")]
        public string Keywords { get; set; }

        [Display(Name = "Summary", ShortName = "Summary")]
        public string Summary { get; set; }

        [Display(Name = "Page Type", ShortName = "Type")]
        public PageType Type { get; set; }

        [Display(Name = "Private", ShortName = "Private")]
        public bool IsPrivate { get; set; }

        [Display(Name = "Page Layout", ShortName = "Layout")]
        public string Layout { get; set; }

        [Display(Name = "Master Page", ShortName = "Master Page")]
        public string PageMasterId { get; set; }

        [Display(Name = "Master Page", ShortName = "Master Page")]
        public string PageMasterTitle { get; set; }

        public string PublishedVersionCode { get; set; }

        public SelectList MasterPages { get; set; }

        public string SiteId { get; set; }

        public string ParentPageId { get; set; }

        public IEnumerable<ContentVersion> Versions { get; set; }

        public string DefaultDomain { get; set; }

        public bool HasBeenPublished
        {
            get {
                if (Versions != null)
                    return Versions.Any(x => x.Status == ContentStatus.Published);

                return !string.IsNullOrEmpty(PublishedVersionCode);
            }
        }

        public bool HasVersions
        {
            get {
                return Versions != null && Versions.Count() > 0;
            }
        }

        public IEnumerable<SecurityClaimConfig> PageSecurityConfig { get; set; }

        public IEnumerable<SecurityClaimConfig> PagePrivacyConfig { get; set; }
    }
}
