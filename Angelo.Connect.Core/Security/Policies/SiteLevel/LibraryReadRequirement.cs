﻿using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Abstractions;


namespace Angelo.Connect.Security.Policies.SiteLevel
{
    public class LibraryReadRequirement : IAuthorizationRequirement
    {
    }

    public class LibraryReadHandler : AbstractSiteLevelOrAboveClaimHandler<LibraryReadRequirement>
    {
        public override IEnumerable<string> ValidClaimTypes { get; set; }

        public LibraryReadHandler(IContextAccessor<AdminContext> adminContextAccessor) : base(adminContextAccessor)
        {
            ValidClaimTypes = new string[]
            {
                SiteClaimTypes.SiteLibraryReader,
                SiteClaimTypes.SitePrimaryAdmin,
                ClientClaimTypes.PrimaryAdmin
            };
        }

    }
}
