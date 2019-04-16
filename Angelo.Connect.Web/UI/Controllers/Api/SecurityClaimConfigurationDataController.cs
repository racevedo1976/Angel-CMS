using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Angelo.Connect.Security;
using Angelo.Identity;
using Angelo.Identity.Models;
using System.Security.Claims;
using Angelo.Identity.Services;

namespace Angelo.Connect.Web.UI.Controllers.Api.ClientLevel
{
    public class SecurityClaimConfigurationDataController : BaseController
    {
        private RoleManager _roleManager;
        private SecurityClaimManager _claimsManager;
        private SecurityPoolManager _poolManager;
        private UserManager _userManager;
        private GroupManager _groupManager;

        public SecurityClaimConfigurationDataController(
            RoleManager roleManager,
            SecurityClaimManager claimsManager,
            SecurityPoolManager poolManager,
            ClientAdminContextAccessor clientContextAccessor,
            IAuthorizationService authorizationService,
            ILogger<ClientRolesDataController> logger,
            UserManager userManager,
            GroupManager groupManager
        ) 
        : base(logger)
        {
            _roleManager = roleManager;
            _poolManager = poolManager;
            _claimsManager = claimsManager;
            _userManager = userManager;
            _groupManager = groupManager;
        }

        [Authorize]
        [HttpPost("api/role/configuration/claim/add")]
        public async Task<ActionResult> PermissionsAdd(string claimType, string claimValue, string roleId)
        {
            if (ModelState.IsValid)
            {
                
                // checking for null or locked roles
                var role = await _roleManager.GetByIdAsync(roleId);

                if (role == null)
                    return BadRequest("Invalid or missing role");

                //if (role.IsLocked)
                //    return BadRequest("Cannot edit the permissions of a locked role");

                IdentityResult result;

                //Get all claims under the role group
                var roleSelectedClaims = await _roleManager.GetClaimObjectsAsync(role);
                
                //check if claim is in role...                  
                if (roleSelectedClaims.Any())
                {
                    if (!roleSelectedClaims.Any(c => c.ClaimType == claimType
                                                && c.ClaimValue == claimValue))
                    {
                        var claim = new System.Security.Claims.Claim(claimType, claimValue);
                        result = await _roleManager.AddClaimAsync(new Role() { Id = role.Id }, claim);
                    }
                }
                    
                return Ok(roleId);
            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost("api/role/configuration/claim/remove")]
        public async Task<ActionResult> PermissionsRemove(string claimType, string claimValue, string roleId)
        {
            if (ModelState.IsValid)
            {
               
                // checking for null or locked roles
                var role = await _roleManager.GetByIdAsync(roleId);

                if (role == null)
                    return BadRequest("Invalid or missing role");

                //if (role.IsLocked)
                //    return BadRequest("Cannot edit the permissions of a locked role");

                IdentityResult result;

                result = await _roleManager.RemoveClaimAsync(claimType, claimValue, roleId);                  

                return Ok(roleId);

            }
            return BadRequest(ModelState);
        }

        [Authorize]
        [HttpPost("api/user/configuration/claim/add")]

        public async Task<ActionResult> UserSecurityClaimAdd(string claimType, string claimValue, string userId, string resourceType)
        {
           
            var identityResult = new IdentityResult();

            var userCurrentClaims = await _userManager.GetClaimObjectsAsync(userId);

            if (!userCurrentClaims.Any(x => x.ClaimType == claimType && x.ClaimValue == claimValue))
            {
                identityResult = await _userManager.AddClaimAsync(userId, new Claim(claimType, claimValue), "Permission");
            }

            return Ok(identityResult);
        }

        [Authorize]
        [HttpPost("api/user/configuration/claim/remove")]

        public async Task<ActionResult> UserSecurityClaimRemove(string claimType, string claimValue, string userId, string resourceType)
        {
            var identityResult = new IdentityResult();

            var userCurrentClaims = await _userManager.GetClaimObjectsAsync(userId);

            var claim = userCurrentClaims.FirstOrDefault(x => x.ClaimType == claimType && x.ClaimValue == claimValue);
            
            if (claim != null)
            {
                identityResult = await _userManager.RemoveClaimAsync(claim.Id);
            }

            return Ok(identityResult);
        }

        [Authorize]
        [HttpPost("api/group/configuration/claim/add")]

        public async Task<ActionResult> GroupSecurityClaimAdd(string claimType, string claimValue, string groupId)
        {

            var identityResult = new IdentityResult();

            var groupClaims = await _groupManager.GetGroupClaimsAsync(groupId, claimType);

            if (!groupClaims.Any(x => x.ClaimType == claimType && x.ClaimValue == claimValue))
            {
                identityResult = await _groupManager.AddClaimAsync(groupId, new Claim(claimType, claimValue), "Permission");
            }

            return Ok(identityResult);
        }

        [Authorize]
        [HttpPost("api/group/configuration/claim/remove")]

        public async Task<ActionResult> GroupSecurityClaimRemove(string claimType, string claimValue, string groupId)
        {
            var identityResult = new IdentityResult();

            var groupClaims = await _groupManager.GetGroupClaimsAsync(groupId, claimType);

            var claim = groupClaims.FirstOrDefault(x => x.ClaimType == claimType && x.ClaimValue == claimValue);

            if (claim != null)
            {
                identityResult = await _groupManager.RemoveClaimAsync(claim.Id);
            }

            return Ok(identityResult);
        }

    }
}
