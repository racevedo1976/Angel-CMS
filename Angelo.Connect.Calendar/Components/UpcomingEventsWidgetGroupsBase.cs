using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Security;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Calendar.UI.ViewModels;

namespace Angelo.Connect.Calendar.Components
{
    public class UpcomingEventsWidgetGroupsBase : ViewComponent
    {
        private IContextAccessor<UserContext> _userContextAccessor;

        public UpcomingEventsWidgetGroupsBase(IContextAccessor<UserContext> userContextAccessor)
        {
            _userContextAccessor = userContextAccessor;
        }

        public async Task<IViewComponentResult> InvokeAsync(UpcomingEventsWidget model)
        {
            var userContext = _userContextAccessor.GetContext();

            var viewModel = new UpcomingEventsGroupFormViewModel
            {
                WidgetId = model.Id,
                UserId = userContext.UserId
            };

            return View(viewModel);
        }
    }
}
