using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Common.Models;
using Angelo.Connect.Models;
using System.ComponentModel.DataAnnotations;

namespace Angelo.Connect.Web.UI.ViewModels.Admin
{
    public class ClientSharedFolderViewModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string ParentId { get; set; }

    }
}
