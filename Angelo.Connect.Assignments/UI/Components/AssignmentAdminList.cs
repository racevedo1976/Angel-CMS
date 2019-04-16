using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Angelo.Connect.Assignments.UI.Components
{
    public class AssignmentAdminList : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(string ownerLevel, string ownerId, string categoryId)
        {
            ViewData["ownerLevel"] = ownerLevel;
            ViewData["ownerId"] = ownerId ?? string.Empty;
            ViewData["categoryId"] = categoryId ?? string.Empty;

            return await Task.Run(() => {
                return View("AssignmentList");
            });
        }
    }
}
