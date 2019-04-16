using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Services;


namespace Angelo.Connect.Rendering.Components
{
    public class ContentNode : ViewComponent
    {
        private ContentManager _contentManager;

        public ContentNode(ContentManager contentManger)
        {
            _contentManager = contentManger;
        }

        public async Task<IViewComponentResult> InvokeAsync(Models.ContentNode node)
        {
            // display mode would have been set in the starting context
            var treeContext = ViewContext.ViewData.GetTreeContext();

            // TODO: Update all widgets so can be exported / imported properly 
            await _contentManager.EnsureContentNodeModel(node);
            
            // Update current node id for decendent nodes
            ViewContext.ViewData.UpdateTreeContext(node.Id);

            return View("/UI/Views/Rendering/ContentNode.cshtml", node);
        }
    }
}
