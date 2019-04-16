using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using Angelo.Connect.UserConsole;
using Angelo.Connect.Services;
using Angelo.Connect.Web.UI.UserConsole.ViewModels;

namespace Angelo.Connect.Web.UI.UserConsole
{
    public class UserConsoleController : Controller
    {
        private UserConsoleComponentFactory _providerFactory;
        private ContentManager _contentManager;

        private IEnumerable<IUserConsoleComponentProvider> _componentProviders;

        public UserConsoleController(UserConsoleComponentFactory providerFactory, ContentManager contentManager)
        {
            _providerFactory = providerFactory;
            _contentManager = contentManager;
        }

        [Authorize]
        [HttpGet("/sys/uc/dialog")]
        [HttpGet("/sys/console/dialog")]
        public async Task<IActionResult> Dialog([FromQuery] string route = null)
        {
            var model = new UserConsoleViewModel();
            var componentExplorers = await _providerFactory.GetExplorerViews();

            model.ExplorerViews = componentExplorers;

            if (componentExplorers != null && componentExplorers.Count() > 0)
            {
                var componentType = componentExplorers.First().Component.ComponentType;

                if (string.IsNullOrEmpty(route))
                    route = await _providerFactory.GetDefaultRoute(componentType);

                model.DefaultRoute = route;
                model.DefaultContent = await _providerFactory.RenderRoute(route);
            }

            return PartialView("~/UI/UserConsole/Views/Dialog.cshtml", model);
        }

        [Authorize]
        [HttpGet("/sys/uc/dialog/{component}")]
        [HttpGet("/sys/console/dialog/{component}")]
        public async Task<IActionResult> ComponentDialog(string component, [FromQuery] string route = null)
        {
            var explorerResult = await _providerFactory.GetExplorerView(component);

            if(string.IsNullOrEmpty(route))
                route = await _providerFactory.GetDefaultRoute(component);

            var model = new UserConsoleViewModel
            {
                DefaultRoute = route,
                ExplorerViews = new List<FactoryViewResult>() { explorerResult },
                DefaultContent = await _providerFactory.RenderRoute(route)
            };

            return PartialView("~/UI/UserConsole/Views/Dialog.cshtml", model);
        }

        [Authorize]
        [HttpGet("/sys/console/nav/{component}")]
        public async Task<IActionResult> ComponentExplorer(string component)
        {
            var result = await _providerFactory.GetExplorerView(component);

            return PartialView("~/UI/UserConsole/Views/Explorer.cshtml", result);
        }

        [Authorize]
        [HttpGet("/sys/console/nav/{tree}/branch/{id}")]
        public async Task<IActionResult> ComponentExplorerTreeBranch(string tree, string id, [FromQuery] string type = null)
        {
            try
            {
                var treeProvider = _providerFactory.GetProvider<UserConsoleTreeProvider>();

                var model = new ConsoleTreeBranch
                {
                    TreeType = tree,
                    Nodes = await treeProvider.GetBranchNodes(tree, id, type)
                };

                return PartialView("~/UI/UserConsole/Views/TreeBranch.cshtml", model);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        /* SHARED UTILITIES */
        [Authorize]
        [HttpGet("/sys/console/versions/{contentType}/{id}")]
        public async Task<IActionResult> VersionModal(string contentType, string id)
        {
            var versions = await _contentManager.GetVersionHistory(contentType, id);

            return PartialView("/UI/UserConsole/Views/VersionSelector.cshtml", versions);
        }

    }

}
