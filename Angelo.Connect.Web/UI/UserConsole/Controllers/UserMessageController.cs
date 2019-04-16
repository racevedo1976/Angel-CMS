using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AutoMapper.Extensions;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using Angelo.Connect.UserConsole;
using Angelo.Connect.UI.ViewModels;
using Angelo.Connect.Web.Data.Mock;

namespace Angelo.Connect.Web.UI.UserConsole.Controllers
{
    public class UserMessageController : Controller
    {
        private IContextAccessor<UserContext> _userContextAccessor;

        public UserMessageController(IContextAccessor<UserContext> userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        [Authorize]
        [HttpGet, Route("/sys/uc/messages")]
        public ActionResult GetMessageSummary()
        {
            var model = MockData.Messages;

            ViewData["SubTitle"] = "All Messages";

            return PartialView("~/UI/UserConsole/Views/Messages/Details.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/uc/messages/{cat}")]
        public ActionResult GetMessageSummary(string cat)
        {
            var model = MockData.Messages.Where(x => x.CategoryName == cat);

            ViewData["SubTitle"] = cat;

            return PartialView("~/UI/UserConsole/Views/Messages/Details.cshtml", model);
        }

    }
}
