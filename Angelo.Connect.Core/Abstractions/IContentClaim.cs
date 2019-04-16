using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Security;

namespace Angelo.Connect.Abstractions
{
    public interface IContentClaim
    {
        OwnerLevel OwnerLevel { get; set; }
        string OwnerId { get; set; }
        string ClaimType { get; set; }
    }
}
