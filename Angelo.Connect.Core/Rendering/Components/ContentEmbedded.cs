using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Data;
using Angelo.Connect.Widgets;
using Angelo.Connect.Services;

namespace Angelo.Connect.Rendering.Components
{
    public class ContentEmbedded : ViewComponent
    {
        private ConnectDbContext _dbContext;
        private WidgetProvider _widgetProvider;
        private ContentManager _contentManager;

        public ContentEmbedded(ConnectDbContext dbContext, WidgetProvider widgetProvider, ContentManager contentManager)
        {
            _dbContext = dbContext;
            _widgetProvider = widgetProvider;
            _contentManager = contentManager;
        }

        public async Task<IViewComponentResult> InvokeAsync(string tree, string container, string widgetType, string modelName, string viewId = null)
        {
            var treeContext = ViewContext.ViewData.GetTreeContext();

            // Get the nodes for this branch of the tree
            var node = await _dbContext.ContentNodes
                .FirstOrDefaultAsync(x =>
                    x.ContentTree.Id == tree
                    && x.Zone == container
                );

            if(node == null)
            {
                node = await CreateNodeFromDefaultModel(tree, container, widgetType, modelName, viewId);
            }


            // Update tree context for rendering child components
            treeContext.NodeId = node.Id;
            treeContext.Zone = new ZoneContext
            {
                Name = container,
                Embedded = true,
                AllowContainers = false,
                AllowPadding = false
            };
            ViewContext.ViewData.SetTreeContext(treeContext);

            return View("/UI/Views/Rendering/ContentEmbedded.cshtml", node);
        }

        private async Task<Models.ContentNode> CreateNodeFromDefaultModel(string treeId, string containerName, string widgetType, string modelName, string viewId)
        {
            
            var widgetConfig = _widgetProvider.GetWidgetConfig(widgetType);
            IWidgetModel widgetModel;

            // Create an instance of the widget using based on default or named model
            if (string.IsNullOrEmpty(modelName))
                widgetModel = _widgetProvider.Create(widgetType);
            else
                widgetModel = _widgetProvider.Create(widgetType, modelName);

            

            // Map the widget to the ContentTree
            var node = new Models.ContentNode()
            {
                ContentTreeId = treeId,
                WidgetType = widgetType,
                WidgetId = widgetModel?.Id,
                Zone = containerName
            };

            // Verify ViewId
            if (viewId == null || !widgetConfig.Views.Any(x => x.Id == viewId))
            {
                node.ViewId = widgetConfig.GetDefaultViewId();
            }
            else
            {
                node.ViewId = viewId;
            }

            await _contentManager.CreateContentNode(node);

            return node;
        }
    }
}
