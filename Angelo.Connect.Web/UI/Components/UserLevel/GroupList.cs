using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Angelo.Identity.Services;
using Angelo.Connect.Abstractions;
using Angelo.Connect.Security;
using System.Linq;
using Angelo.Connect.Web.UI.ViewModels.Admin;

namespace Angelo.Connect.Web.UI.Components
{
    public class GroupList : ViewComponent
    {
        private GroupManager _groupManager;
        private IContextAccessor<UserContext> _userContextAccessor;

        public GroupList(GroupManager groupManager, IContextAccessor<UserContext> userContextAccessor)
        {
            _groupManager = groupManager;
            _userContextAccessor = userContextAccessor; 
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userContext = _userContextAccessor.GetContext();

            var groups = (await _groupManager.GetGroupsOwnedByUser(userContext.UserId));

            var groupsViewModel = groups.Select(x => new GroupViewModel
            {
                Id = x.Id,
                Name = x.Name,
                OwnerId = x.UserId
            }).ToList();

           
            return View("GroupList", groupsViewModel);
           
        }
    }
}
