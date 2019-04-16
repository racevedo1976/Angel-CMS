using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Identity.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class PageSecurityUserViewModel
    {
        public string PageId { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string AccessLevel { get; set; }

        public string Id {
            get
            {
                return string.Concat("P:", PageId, "+U:", UserId);
            }
        }
    }
}
