using System.Linq;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;


namespace Angelo.Connect.News.Security
{
    public class NewsSecurityService
    {

        private IContextAccessor<SiteContext> _siteContextAccessor;
        private IContextAccessor<UserContext> _userContextAccessor;

        public NewsSecurityService
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
            
            // Has permission to create News
            if (HasAnyClaim(NewsClaimTypes.PersonalNewsAuthor))
                return true;

            // Site Admins implicitly have permissions to create News
            if (HasAnyClaim(SiteClaimTypes.SitePrimaryAdmin))
                return true;

            // Client Level News Content Admins can create News
            if (HasAdminClaim(NewsClaimTypes.UserNewsManage))
                return true;

            // Client Level Content Admins can create any content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Primary Admins can create anything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            return false;
        }

        public bool AuthorizeForRead(Models.NewsPost newsPost)
        {
            
            // The user can create News and this is their News
            if (HasAuthorClaim(NewsClaimTypes.PersonalNewsAuthor, newsPost))
                return true;

            // The user is a site admin and this is their News
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, newsPost))
                return true;

            // The user has been granted read access to this News by the author
            if (HasDelegateClaim(NewsClaimTypes.NewsPostRead, newsPost))
                return true;

            // Client Level News Content Admin can view all News
            if (HasAdminClaim(NewsClaimTypes.UserNewsBrowse))
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
            // News Content Admin
            if (HasAdminClaim(NewsClaimTypes.UserNewsBrowse))
                return true;

            // User Content Admin
            if (HasAdminClaim(ClientClaimTypes.UserContentBrowse))
                return true;

            // Primary Admin
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            return false;
        }

        public bool AuthorizeForEdit(Models.NewsPost newsPost)
        {
            

            // Can author News and this is their News
            if (HasAuthorClaim(NewsClaimTypes.PersonalNewsAuthor, newsPost))
                return true;

            // Is a site admin and this is their News
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, newsPost))
                return true;

            // Has been granted access to edit this News by the author
            if (HasDelegateClaim(NewsClaimTypes.NewsPostEdit, newsPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can manage all content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level News Admin can manage all News
            if (HasAdminClaim(NewsClaimTypes.UserNewsManage))
                return true;

            return false;
        }

        public bool AuthorizeForPublish(Models.NewsPost newsPost)
        {
           

            // Can author News and this is their News
            if (HasAuthorClaim(NewsClaimTypes.PersonalNewsPublish, newsPost))
                return true;

            // Is a site admin and this is their News
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, newsPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can publish all content
            if (HasAdminClaim(ClientClaimTypes.UserContentPublish))
                return true;

            // Client Level News Admin can publish all News
            if (HasAdminClaim(NewsClaimTypes.UserNewsPublish))
                return true;

            // no delegate claims to check since "publish" is not allowed to be delegated
            return false;
        }

        public bool AuthorizeForDelete(Models.NewsPost newsPost)
        {
           
            // Can author News and this is their News
            if (HasAuthorClaim(NewsClaimTypes.PersonalNewsAuthor, newsPost))
                return true;

            // Is a site admin and this is their News
            if (HasAuthorClaim(NewsClaimTypes.PersonalNewsAuthor, newsPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can manage all content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level News Admin can manage all News
            if (HasAdminClaim(NewsClaimTypes.UserNewsManage))
                return true;

            return false;
        }
        
        private bool HasAdminClaim(string claimType)
        {
            var siteContext = _siteContextAccessor.GetContext();
            var userContext = _userContextAccessor.GetContext();

            // EG, ClientLevelOrAbove
            return userContext.SecurityClaims.FindAny(new[]
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

        private bool HasAuthorClaim(string claimType, Models.NewsPost newsPost)
        {
            var userContext = _userContextAccessor.GetContext();

            if(userContext.UserId == newsPost.UserId)
            {
                // not checking value because is irrevant since we're checking the NewsPost.author directly.
                return userContext.SecurityClaims.Any(x => x.Type == claimType);
            }


            return false;
        }

        private bool HasDelegateClaim(string claimType, Models.NewsPost newsPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // check for claimType with value equal this NewsPost's Id
            return userContext.SecurityClaims.Any(x => x.Type == claimType && x.Value == newsPost.Id);
        }

    }
}
