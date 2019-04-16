using System;

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Angelo.Identity.Models
{
    public class UserClaim : IdentityUserClaim<string>
    {
        /// <summary>
        /// The OpenId scope of the claim - eg, profile, etc
        /// </summary>
        public string ClaimScope { get; set; }
        /// <summary>
        /// Specific resource to secure
        /// </summary>
       // public string ResourceType { get; set; }
        public User User { get; set; }
    }
}
