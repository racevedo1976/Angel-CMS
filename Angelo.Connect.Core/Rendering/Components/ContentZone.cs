using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Data;

namespace Angelo.Connect.Rendering.Components
{
    public class ContentZone : ViewComponent
    {
        private ConnectDbContext _dbContext;

        public ContentZone(ConnectDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IViewComponentResult> InvokeAsync(string zone, string tree, string node)
        {
            // Get the nodes for this branch of the tree
            var nodes = await _dbContext.ContentNodes
                .Where(x =>
                    x.ContentTree.Id == tree
                    && x.ParentId == node
                    && x.Zone == zone 
                )               
                .ToListAsync();

            // manual sorting
            nodes.Sort((x, y) => {
                return x.Index > y.Index ? 1 : -1;
            });
           
            return View("/UI/Views/Rendering/ContentZone.cshtml", nodes);
        }
    }
}
