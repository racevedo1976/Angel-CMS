using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Angelo.Connect.Calendar.Models;

namespace Angelo.Connect.Calendar.Components
{
    public class CalendarWidgetGroupsBase : ViewComponent
    {

        public async Task<IViewComponentResult> InvokeAsync(CalendarWidgetSetting model)
        {

            return View(model);
        }
    }
}
