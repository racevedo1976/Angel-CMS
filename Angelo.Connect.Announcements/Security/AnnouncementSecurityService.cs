using System;
using System.Linq;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Announcement.Security;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;


namespace Angelo.Connect.Announcement.Security
{
    public class AnnouncementSecurityService
    {

        private IContextAccessor<SiteContext> _siteContextAccessor;
        private IContextAccessor<UserContext> _userContextAccessor;

        public AnnouncementSecurityService
        (
            IContextAccessor<SiteContext> siteContextAccessor, 
            IContextAccessor<UserContext> userContextAccessor
        )
        {
            _siteContextAccessor = siteContextAccessor;
            _userContextAccessor = userContextAccessor;
        }
        

        public bool AuthorizeForCreate()
        {
            var userContext = _userContextAccessor.GetContext();

            // Has permission to create announcements
            if (HasAnyClaim(AnnouncementClaimTypes.PersonalAnnouncementAuthor))
                return true;

            // Site Admins implicitly have permissions to create announcements
            if (HasAnyClaim(SiteClaimTypes.SitePrimaryAdmin))
                return true;

            // Client Level Announcement Content Admins can create announcements
            if (HasAdminClaim(AnnouncementClaimTypes.UserAnnouncementsManage))
                return true;

            // Client Level Content Admins can create any content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Primary Admins can create anything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            return false;
        }

        public bool AuthorizeForRead(Models.AnnouncementPost announcementPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // The user can create announcements and this is their announcement
            if (HasAuthorClaim(AnnouncementClaimTypes.PersonalAnnouncementAuthor, announcementPost))
                return true;

            // The user is a site admin and this is their announcement
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, announcementPost))
                return true;

            // The user has been granted read access to this announcement by the author
            if (HasDelegateClaim(AnnouncementClaimTypes.AnnouncementPostRead, announcementPost))
                return true;

            // Client Level Announcement Content Admin can view all announcements
            if (HasAdminClaim(AnnouncementClaimTypes.UserAnnouncementsBrowse))
                return true;

            // Client Level User Content Admin can view all content
            if (HasAdminClaim(ClientClaimTypes.UserContentBrowse))
                return true;

            // Client Level Primary Admin can view everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            return false;
        }

        public bool AuthorizeForReadAdmin()
        {
            // Announcement Content Admin
            if (HasAdminClaim(AnnouncementClaimTypes.UserAnnouncementsBrowse))
                return true;

            // User Content Admin
            if (HasAdminClaim(ClientClaimTypes.UserContentBrowse))
                return true;

            // Primary Admin
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            return false;
        }

        public bool AuthorizeForEdit(Models.AnnouncementPost announcementPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // Can author announcements and this is their announcement
            if (HasAuthorClaim(AnnouncementClaimTypes.PersonalAnnouncementAuthor, announcementPost))
                return true;

            // Is a site admin and this is their announcement
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, announcementPost))
                return true;

            // Has been granted access to edit this announcement by the author
            if (HasDelegateClaim(AnnouncementClaimTypes.AnnouncementPostEdit, announcementPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can manage all content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Announcement Admin can manage all announcements
            if (HasAdminClaim(AnnouncementClaimTypes.UserAnnouncementsManage))
                return true;

            return false;
        }

        public bool AuthorizeForPublish(Models.AnnouncementPost announcementPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // Can author announcements and this is their announcement
            if (HasAuthorClaim(AnnouncementClaimTypes.PersonalAnnouncementPublish, announcementPost))
                return true;

            // Is a site admin and this is their announcement
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, announcementPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can publish all content
            if (HasAdminClaim(ClientClaimTypes.UserContentPublish))
                return true;

            // Client Level Announcement Admin can publish all announcements
            if (HasAdminClaim(AnnouncementClaimTypes.UserAnnouncementsPublish))
                return true;

            // no delegate claims to check since "publish" is not allowed to be delegated
            return false;
        }

        public bool AuthorizeForDelete(Models.AnnouncementPost announcementPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // Can author announcements and this is their announcement
            if (HasAuthorClaim(AnnouncementClaimTypes.PersonalAnnouncementAuthor, announcementPost))
                return true;

            // Is a site admin and this is their announcement
            if (HasAuthorClaim(AnnouncementClaimTypes.PersonalAnnouncementAuthor, announcementPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can manage all content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Announcement Admin can manage all announcements
            if (HasAdminClaim(AnnouncementClaimTypes.UserAnnouncementsManage))
                return true;

            return false;
        }
        
        private bool HasAdminClaim(string claimType)
        {
            var siteContext = _siteContextAccessor.GetContext();
            var userContext = _userContextAccessor.GetContext();

            // EG, ClientLevelOrAbove
            return userContext.SecurityClaims.FindAny(new SecurityClaim[]
            {
                new SecurityClaim(claimType, ConnectCoreConstants.CorporateId),
                new SecurityClaim(claimType, siteContext.Client.Id),
            });
        }

        private bool HasAnyClaim(string claimType)
        {
            var userContext = _userContextAccessor.GetContext();

            // Eg, Doesn't matter which site gave them the claim, so long as they have it 
            return userContext.SecurityClaims.Any(x => x.Type == claimType);
        }

        private bool HasAuthorClaim(string claimType, Models.AnnouncementPost announcementPost)
        {
            var userContext = _userContextAccessor.GetContext();

            if(userContext.UserId == announcementPost.UserId)
            {
                // not checking value because is irrevant since we're checking the announcementPost.author directly.
                return userContext.SecurityClaims.Any(x => x.Type == claimType);
            }


            return false;
        }

        private bool HasDelegateClaim(string claimType, Models.AnnouncementPost announcementPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // check for claimType with value equal this announcementPost's Id
            return userContext.SecurityClaims.Any(x => x.Type == claimType && x.Value == announcementPost.Id);
        }

    }
}
