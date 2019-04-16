using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Security.KnownClaims
{
    public class UserClaimTypes
    {
        public const string PersonalContentAuthor = "user-content-author";
        public const string PersonalContentPublish = "user-content-publish";
        public const string PersonalPageAuthor= "user-personal-page-author";
        public const string PersonalPagePublish = "user-personal-page-publish";

        public const string PersonalLibraryOwner = "user-library-owner";
        public const string PersonalGroupOwner = "user-group-owner";

        public const string NotificationSubscriber = "user-notification-subscriber";
    }
}
