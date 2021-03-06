﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class SiteCollectionSiteViewModel
    {
        public string SiteCollectionId { get; set; }

        public string ClientId { get; set; }

        public string SiteId { get; set; }

        public string Title { get; set; }
    }
}
