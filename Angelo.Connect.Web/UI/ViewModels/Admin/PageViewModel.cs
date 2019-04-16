using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Models;
using Angelo.Connect.Web.UI.ViewModels.Validation;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class PageViewModel
    {

        [Display(Name = "Id", ShortName = "Id")]
        public string Id { get; set; }

        [Display(Name = "Page Title", ShortName = "Title")]
        public string Title { get; set; }

        [PagePath]
        [Display(Name = "Url Path", ShortName = "Path")]
        public string Path { get; set; }

        [Display(Name = "Page Type", ShortName = "Type")]
        public PageType Type { get; set; }

        [Display(Name = "Keywords", ShortName = "Keywords")]
        public string Keywords { get; set; }

        [Display(Name = "Summary", ShortName = "Summary")]
        public string Summary { get; set; }

        [Display(Name = "Private", ShortName = "Private")]
        public bool IsPrivate { get; set; }

        [Display(Name = "Page Layout", ShortName = "Layout")]
        public string Layout { get; set; }

        [Display(Name = "Master Page", ShortName = "Master")]
        public string PageMasterId { get; set; }

        [Display(Name = "Master Page", ShortName = "Master")]
        public string PageMasterTitle { get; set; }

        [Display(Name = "Parent Page", ShortName = "Parent")]
        public string ParentPageId { get; set; }

        [Display(Name = "Parent Page", ShortName = "Parent")]
        public string ParentPageTitle { get; set; }

        public string PublishedVersionCode { get; set; }

        public SelectList MasterPages { get; set; }

        public string SiteId { get; set; }
        public bool IsHomePage { get; set; }


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
    }
}
