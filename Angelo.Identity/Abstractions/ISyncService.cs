using Angelo.Identity.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Identity.Abstractions
{
    public interface ISyncService<TUser, LdUser> where TUser: User, new()
                                                 where LdUser: ILdapUser
    {
        Task<TUser> CreateUserFromLdapAsync(LdUser ldapUser);

        Task<TUser> SyncUserFromLdapAsync(TUser user, LdUser ldapUser);
    }
}
