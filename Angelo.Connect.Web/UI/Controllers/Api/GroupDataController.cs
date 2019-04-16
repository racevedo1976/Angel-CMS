using Angelo.Connect.Web.UI.ViewModels.Admin;
using Angelo.Identity.Models;
using Angelo.Identity.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Angelo.Connect.Web.UI.Controllers.Api
{
    public class GroupDataController: BaseController
    {
        private GroupManager _groupManager;

        public GroupDataController(ILogger<GroupDataController> logger,
            GroupManager groupManager) : base(logger)
        {
            _groupManager = groupManager;
        }

        [Authorize]
        [HttpPost, Route("/api/group/save")]
        public async Task<ActionResult> SaveUserGroup(GroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (string.IsNullOrEmpty(model.Id))
                {
                    model.Id = Guid.NewGuid().ToString("N");
                    
                    await _groupManager.InserGroupAsync(new Group {
                                    Id = model.Id,
                                    UserId = model.OwnerId,
                                    Name = model.Name});
                }
                else
                {
                    await _groupManager.UpdateGroupAsync(new Group
                    {
                        Id = model.Id,
                        UserId = model.OwnerId,
                        Name = model.Name
                    });
                }
                return Ok(model);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost, Route("/api/group/membership/add")]
        public async Task<ActionResult> AddMembersToGroup (GroupMembership groupMembership)
        {
            
            if (ModelState.IsValid)
            {
               
                await _groupManager.AddUserToGroup(groupMembership);
                
                return Ok(groupMembership);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete, Route("/api/group/membership/remove")]
        public async Task<ActionResult> RemoveMembersFromGroup(GroupMembership groupMembership)
        {

            if (ModelState.IsValid)
            {

                await _groupManager.RemoveUserFromGroup(groupMembership);

                return Ok(groupMembership);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpDelete, Route("/api/group")]
        public async Task<ActionResult> RemoveGroup(string id)
        {

            if (ModelState.IsValid)
            {

                await _groupManager.RemoveGroup(id);

                return Ok();
            }
            return BadRequest(ModelState);
        }

    }
}
