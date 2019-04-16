using Angelo.Connect.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Identity.Models;

namespace Angelo.Connect.Abstractions
{
    public interface ISecurityPermissionProvider
    {
        PoolType Level { get; }

        IEnumerable<Permission> PermissionGroups();
    }
}
