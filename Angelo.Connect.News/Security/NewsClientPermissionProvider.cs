using System;
using System.Collections.Generic;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Identity.Models;

namespace Angelo.Connect.News.Security
{
    public class NewsClientPermissionProvider : ISecurityPermissionProvider
    {
        // using site admin context, not site context 
        private IContextAccessor<ClientAdminContext> _clientContextAccessor;

        public PoolType Level { get; } = PoolType.Client;
       

        public NewsClientPermissionProvider(IContextAccessor<ClientAdminContext> clientContextAccessor)
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
                        Title = "News Content",
                        Permissions = new[]
                        {
                            new Permission {
                                Title = "Author & Create Personal News",
                                Claims = {
                                    new SecurityClaim(NewsClaimTypes.PersonalNewsAuthor, currentClientId),
                                    new SecurityClaim(NewsClaimTypes.PersonalNewsPublish, currentClientId)
                                }
                            },


                            new Permission {
                                Title = "Manage / Administer All User News",
                                Claims = {
                                    new SecurityClaim(NewsClaimTypes.UserNewsBrowse, currentClientId),
                                    new SecurityClaim(NewsClaimTypes.UserNewsManage, currentClientId),
                                    new SecurityClaim(NewsClaimTypes.UserNewsPublish, currentClientId)
                                }
                            },
                        }
                    }
            };
        }
    }
}
