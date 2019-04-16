using System;
using System.Linq;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Blog.Security;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;


namespace Angelo.Connect.Blog.Security
{
    public class BlogSecurityService
    {

        private IContextAccessor<SiteContext> _siteContextAccessor;
        private IContextAccessor<UserContext> _userContextAccessor;

        public BlogSecurityService
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

            // Has permission to create blogs
            if (HasAnyClaim(BlogClaimTypes.PersonalBlogAuthor))
                return true;

            // Site Admins implicitly have permissions to create blogs
            if (HasAnyClaim(SiteClaimTypes.SitePrimaryAdmin))
                return true;

            // Client Level Blog Content Admins can create blogs
            if (HasAdminClaim(BlogClaimTypes.UserBlogsManage))
                return true;

            // Client Level Content Admins can create any content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Primary Admins can create anything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            return false;
        }

        public bool AuthorizeForRead(Models.BlogPost blogPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // The user can create blogs and this is their blog
            if (HasAuthorClaim(BlogClaimTypes.PersonalBlogAuthor, blogPost))
                return true;

            // The user is a site admin and this is their blog
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, blogPost))
                return true;

            // The user has been granted read access to this blog by the author
            if (HasDelegateClaim(BlogClaimTypes.BlogPostRead, blogPost))
                return true;

            // Client Level Blog Content Admin can view all blogs
            if (HasAdminClaim(BlogClaimTypes.UserBlogsBrowse))
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
            // Blog Content Admin
            if (HasAdminClaim(BlogClaimTypes.UserBlogsBrowse))
                return true;

            // User Content Admin
            if (HasAdminClaim(ClientClaimTypes.UserContentBrowse))
                return true;

            // Primary Admin
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            return false;
        }

        public bool AuthorizeForEdit(Models.BlogPost blogPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // Can author blogs and this is their blog
            if (HasAuthorClaim(BlogClaimTypes.PersonalBlogAuthor, blogPost))
                return true;

            // Is a site admin and this is their blog
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, blogPost))
                return true;

            // Has been granted access to edit this blog by the author
            if (HasDelegateClaim(BlogClaimTypes.BlogPostEdit, blogPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can manage all content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Blog Admin can manage all blogs
            if (HasAdminClaim(BlogClaimTypes.UserBlogsManage))
                return true;

            return false;
        }

        public bool AuthorizeForPublish(Models.BlogPost blogPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // Can author blogs and this is their blog
            if (HasAuthorClaim(BlogClaimTypes.PersonalBlogPublish, blogPost))
                return true;

            // Is a site admin and this is their blog
            if (HasAuthorClaim(SiteClaimTypes.SitePrimaryAdmin, blogPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can publish all content
            if (HasAdminClaim(ClientClaimTypes.UserContentPublish))
                return true;

            // Client Level Blog Admin can publish all blogs
            if (HasAdminClaim(BlogClaimTypes.UserBlogsPublish))
                return true;

            // no delegate claims to check since "publish" is not allowed to be delegated
            return false;
        }

        public bool AuthorizeForDelete(Models.BlogPost blogPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // Can author blogs and this is their blog
            if (HasAuthorClaim(BlogClaimTypes.PersonalBlogAuthor, blogPost))
                return true;

            // Is a site admin and this is their blog
            if (HasAuthorClaim(BlogClaimTypes.PersonalBlogAuthor, blogPost))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;

            // Client Level Content Admin can manage all content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Blog Admin can manage all blogs
            if (HasAdminClaim(BlogClaimTypes.UserBlogsManage))
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

        private bool HasAuthorClaim(string claimType, Models.BlogPost blogPost)
        {
            var userContext = _userContextAccessor.GetContext();

            if(userContext.UserId == blogPost.UserId)
            {
                // not checking value because is irrevant since we're checking the blogPost.author directly.
                return userContext.SecurityClaims.Any(x => x.Type == claimType);
            }


            return false;
        }

        private bool HasDelegateClaim(string claimType, Models.BlogPost blogPost)
        {
            var userContext = _userContextAccessor.GetContext();

            // check for claimType with value equal this blogPost's Id
            return userContext.SecurityClaims.Any(x => x.Type == claimType && x.Value == blogPost.Id);
        }

    }
}
