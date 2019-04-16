using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

using Angelo.Identity.Models;

namespace Angelo.Identity
{
    public class RoleStore : RoleStore<Role,IdentityDbContext,string,UserRole, RoleClaim>
    {
        public RoleStore(IdentityDbContext dbContext, IdentityErrorDescriber describer) : base(dbContext, describer)
        {

        }

        protected override RoleClaim CreateRoleClaim(Role role, Claim claim)
        {
            return new RoleClaim { RoleId = role.Id, ClaimType = claim.Type, ClaimValue = claim.Value };
        }

    }
}
