using Angelo.Identity.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Abstractions
{
    public interface ISecurityGroupProvider: IGroupProvider
    {
        Task<bool> IsActiveMember(User user, IGroup group);
    }
}
