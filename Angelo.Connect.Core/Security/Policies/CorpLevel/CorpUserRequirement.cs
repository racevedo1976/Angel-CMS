﻿using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Security.Policies.CorpLevel
{
    public class CorpUserRequirement : IAuthorizationRequirement
    {
    }

    public class CorpUserHandler : AbstractCorpLevelClaimHandler<CorpUserRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public CorpUserHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                CorpClaimTypes.CorpUser
            };
        }
        
    }
}
