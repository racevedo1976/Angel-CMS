using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Connect.Models;
using AutoMapper.Extensions;
using Angelo.Common.Mvc.ActionResults;

namespace Angelo.Connect.Web.UI.Components
{
    public class SharedFolderCreate : ViewComponent
    {
        public SharedFolderCreate()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(string documentType = "",  string parentFolderId = "", string clientId = "", string siteId = "")
        {
            var model = new SharedFolderViewModel()
            {
                FolderId = string.Empty,
                Title = string.Empty,
                DocumentType = documentType,
                ParentFolderId = parentFolderId,
            };
            if (string.IsNullOrEmpty(siteId) == false)
            {
                ViewData["siteId"] = siteId;
                return View("SharedFolderCreateForSite", model);
            }
            else if (string.IsNullOrEmpty(clientId) == false)
            {
                ViewData["clientId"] = clientId;
                return View("SharedFolderCreateForClient", model);
            }
            return await Task.FromResult(new ViewComponentPlaceholder());
        }

    }
}
