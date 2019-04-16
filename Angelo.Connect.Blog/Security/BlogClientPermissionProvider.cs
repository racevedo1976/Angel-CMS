using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Identity.Models;

namespace Angelo.Connect.Blog.Security
{
    public class BlogClientPermissionProvider : ISecurityPermissionProvider
    {
        // using site admin context, not site context 
        private IContextAccessor<ClientAdminContext> _clientContextAccessor;

        public PoolType Level { get; } = PoolType.Client;
       

        public BlogClientPermissionProvider(IContextAccessor<ClientAdminContext> clientContextAccessor)
        {
            _clientContextAccessor = clientContextAccessor;
        }

        public IEnumerable<Permission> PermissionGroups()
        {
            var clientContext = _clientContextAccessor.GetContext();

            if (clientContext?.Client == null)
                throw new NullReferenceException("ClientAdminContext.Client is required");

            var currentClientId = clientContext.Client.Id;


            //build permissions groups
            return new List<Permission>(){
                    new Permission {
                        Title = "Blog Content",
                        Permissions = new Permission[]
                        {
                            new Permission {
                                Title = "Author & Create Personal Blogs",
                                Claims = {
                                    new SecurityClaim(BlogClaimTypes.PersonalBlogAuthor, currentClientId),
                                    new SecurityClaim(BlogClaimTypes.PersonalBlogPublish, currentClientId)
                                }
                            },


                            new Permission {
                                Title = "Manage / Administer All User Blogs",
                                Claims = {
                                    new SecurityClaim(BlogClaimTypes.UserBlogsBrowse, currentClientId),
                                    new SecurityClaim(BlogClaimTypes.UserBlogsManage, currentClientId),
                                    new SecurityClaim(BlogClaimTypes.UserBlogsPublish, currentClientId)
                                }
                            },
                        }
                    }
            };
        }
    }
}
