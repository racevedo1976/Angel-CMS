using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Angelo.Identity.Models
{
    public class RoleClaim : IdentityRoleClaim<string>
    {  
        public Role Role { get; set; }
    }
}
