using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;

namespace Angelo.Connect.Web.UI.ViewModels
{
    public class PortalsViewModel
    {
        public string Title { get; set; }

        public ICollection<Site> Sites { get; set; }
    }
}
