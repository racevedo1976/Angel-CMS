using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Identity.Models;

namespace Angelo.Connect.Announcement.Security
{
    public class AnnouncementClientPermissionProvider : ISecurityPermissionProvider
    {
        // using site admin context, not site context 
        private IContextAccessor<ClientAdminContext> _clientContextAccessor;

        public PoolType Level { get; } = PoolType.Client;
       

        public AnnouncementClientPermissionProvider(IContextAccessor<ClientAdminContext> clientContextAccessor)
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
                        Title = "Announcement Content",
                        Permissions = new Permission[]
                        {
                            new Permission {
                                Title = "Author & Create Personal Announcements",
                                Claims = {
                                    new SecurityClaim(AnnouncementClaimTypes.PersonalAnnouncementAuthor, currentClientId),
                                    new SecurityClaim(AnnouncementClaimTypes.PersonalAnnouncementPublish, currentClientId)
                                }
                            },


                            new Permission {
                                Title = "Manage / Administer All User Announcements",
                                Claims = {
                                    new SecurityClaim(AnnouncementClaimTypes.UserAnnouncementsBrowse, currentClientId),
                                    new SecurityClaim(AnnouncementClaimTypes.UserAnnouncementsManage, currentClientId),
                                    new SecurityClaim(AnnouncementClaimTypes.UserAnnouncementsPublish, currentClientId)
                                }
                            },
                        }
                    }
            };
        }
    }
}
