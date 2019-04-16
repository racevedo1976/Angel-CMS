using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using Angelo.Connect.Widgets;
using Angelo.Connect.Services;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Web.UI.Controllers
{
    public class ContentController : Controller
    {
        private ContentManager _contentManager;
        private WidgetProvider _widgetProvider;
        private IServiceProvider _serviceProvider;
        private IHttpContextAccessor _httpContextAccessor;

        public ContentController(ContentManager contentManager, WidgetProvider widgetProvider, IServiceProvider serviceProvider, IHttpContextAccessor httpContextAccessor) 
        {
            _contentManager = contentManager;
            _widgetProvider = widgetProvider;
            _serviceProvider = serviceProvider;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet, Route("/sys/content/{type}/menu2/{id}")]
        public IActionResult GetMenu(string type, string id, [FromQuery] string ru)
        {
            return ViewComponent("ContentMenu", new { type = type, id = id, ru =  ru});
        }

        [Route("/sys/content/nodes")]
        public IActionResult CreateNode([FromQuery]string tree, [FromQuery]string type, [FromQuery]string view, [FromQuery]string parent, [FromQuery]int index)
        {
            var zone = GetZoneContextFromQuery(Request.Query);

            // TODO: Move validation logic to service
            if (tree == null || type == null || zone == null)
                return BadRequest("Invalid node operation");

            var widget = _widgetProvider.Create(type);

            if(view == null)
                view = _widgetProvider.GetWidgetConfig(type).Views.First().Id;

            var node = new ContentNode()
            {
                ContentTreeId = tree,
                ParentId = parent,
                WidgetType = type,
                WidgetId = widget?.Id,
                ViewId = view,
                Zone = zone.Name,
                Index = index,
            };
           
            _contentManager.CreateContentNode(node).Wait();

            InitTreeContext(node, zone);

            return ViewComponent("ContentNode", new { node = node });
        }

        [HttpGet, Route("/sys/content/nodes/{id}")]
        public IActionResult GetNode(string id)
        {
            var node = _contentManager.GetContentNode(id).Result;
            var zone = GetZoneContextFromQuery(Request.Query);

            if (node == null || zone == null)
                return BadRequest("Invalid node operation");

            InitTreeContext(node, zone);

            return ViewComponent("ContentNode", new { node = node });
        }

        [HttpGet, Route("/sys/content/{type}/menu/{id}")]
        public IActionResult GetCustomMenu(string type, string id, [FromQuery] string ru)
        {
            var menuItems = _widgetProvider.GetCustomMenuItems(type, id, ru);

            return new JsonResult(menuItems.Select(x => new
            {
                Url = x.Url,
                Title = x.Title,
                Icon = x.Icon.ToString()
            }));
        }

        [HttpDelete, Route("/sys/content/nodes/{id}")]
        public IActionResult DeleteNode(string id)
        {
            _contentManager.DeleteContentNode(id).Wait();
                      
            return Ok();
        }

        [HttpPost, Route("/sys/content/nodes/{id}")]
        public IActionResult MoveNode(string id, ContentNodeUpdateModel model)
        {
            _contentManager.UpdateNodePosition(id, model.Parent, model.Zone, int.Parse(model.Index)).Wait();

            return Ok();
        }

        [HttpPost, Route("/sys/content/nodes/{id}/css")]
        public async Task<IActionResult> SetNodeClasses(string id, [FromForm] string value)
        {
            await _contentManager.UpdateNodeClasses(id, value);

            return Ok();
        }


        [HttpPost, Route("/sys/content/nodes/{id}/bg")]
        public async Task<IActionResult> SetNodeBackground(string id, [FromForm] string value)
        {
            await _contentManager.UpdateNodeBackground(id, value);

            return Ok();
        }

        [HttpPost, Route("/sys/content/nodes/{id}/width")]
        public async Task<IActionResult> SetNodeFullWidth(string id, [FromForm] bool fullWidth)
        {
            await _contentManager.UpdateNodeFullWidth(id, fullWidth);

            return Ok();
        }

        [HttpPost, Route("/sys/content/nodes/{id}/padding")]
        public async Task<IActionResult> SetNodePadding(string id, [FromForm] string top, [FromForm] string bottom)
        {
            await _contentManager.UpdateNodePadding(id, top, bottom);

            return Ok();
        }

        [HttpPost, Route("/sys/content/nodes/{id}/maxheight")]
        public async Task<IActionResult> SetNodeMaxHeight(string id, [FromForm] string value)
        {
            await _contentManager.UpdateNodeMaxHeight(id, value);

            return Ok();
        }

        [HttpPost, Route("/sys/content/nodes/{id}/alignment")]
        public async Task<IActionResult> SetNodeAlignment(string id, [FromForm] string value)
        {
            await _contentManager.UpdateNodeAlignment(id, value);

            return Ok();
        }

        public class ContentNodeUpdateModel
        {
            public string Parent { get; set; }
            public string Index { get; set; }
            public string Zone { get; set; }
        }

        private void InitTreeContext(ContentNode node, ZoneContext zone)
        {

            // Tree Context
            // design mode = true, otherwise we wouldn't be here
            // set allowcontainers at the root to be same as zone's current settings
            ViewData.SetTreeContext(new TreeContext
            {
                TreeId = node.ContentTreeId,
                NodeId = node.Id,
                Editable = true,
                AllowContainers = zone.AllowContainers,
                Zone = zone
            });
        }

        private ZoneContext GetZoneContextFromQuery(IQueryCollection query)
        {
            if (query.ContainsKey("zone[name]"))
            {
                var context = new ZoneContext
                {
                    Name = query["zone[name]"].ToString()
                };

                if (query.ContainsKey("zone[embedded]"))
                    context.Embedded = bool.Parse(query["zone[embedded]"]);

                if (query.ContainsKey("zone[allowContainers]"))
                    context.AllowContainers = bool.Parse(query["zone[allowContainers]"]);

                if (query.ContainsKey("zone[allowPadding]"))
                    context.AllowContainers = bool.Parse(query["zone[allowPadding]"]);

                return context;
            }

            return null;
        }
    }
}
