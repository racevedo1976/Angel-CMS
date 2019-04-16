using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Angelo.Identity.Models
{
    public class UserLogin : IdentityUserLogin<string>
    {       
        public User User { get; set; }
    }
}
