using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using Angelo.Connect.Models;
using Angelo.Connect.Services;

namespace Angelo.Connect.Web.UI.Controllers
{ 
    public class AdminController : BaseController
    { 
      
        public AdminController(ILogger logger) : base(logger)
        {

        }
    }
}
