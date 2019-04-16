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
    public class UserLibraryController : Controller
    {
        private IContextAccessor<UserContext> _userContextAccessor;

        public UserLibraryController(IContextAccessor<UserContext> userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        [Authorize]
        [HttpGet, Route("/sys/uc/library")]
        public ActionResult DefaultView()
        {
            var model = MockData.Files;

            ViewData["LibraryTitle"] = "File Count: All Folders";

            return PartialView("~/UI/UserConsole/Views/Messages/FolderDetails.cshtml", model);
        }

        [Authorize]
        [HttpGet, Route("/sys/uc/library/{folderId}")]
        public ActionResult FolderView(string folderId)
        {
            var model = MockData.Files.Where(x => x.FolderId == folderId);

            ViewData["LibraryTitle"] = "File Count" + folderId;

            return PartialView("~/UI/UserConsole/Views/Library/FolderDetails.cshtml", model);
        }

    }
}
