using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security.KnownClaims
{
    public class PageClaimTypes
    {

        // Used for private pages 
        public const string ViewPrivatePage = "page-view-private";

        // Used by page security
        public const string PageOwner = "page-owner";
        public const string DesignPage = "page-design";
        public const string PublishPage = "page-publish";
    }
}
