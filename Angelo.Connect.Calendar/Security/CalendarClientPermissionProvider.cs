using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Identity.Models;

namespace Angelo.Connect.Calendar.Security
{
    public class CalendarClientPermissionProvider : ISecurityPermissionProvider
    {
        // using site admin context, not site context 
        private IContextAccessor<ClientAdminContext> _clientContextAccessor;

        public PoolType Level { get; } = PoolType.Client;
       

        public CalendarClientPermissionProvider(IContextAccessor<ClientAdminContext> clientContextAccessor)
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
                        Title = "Calendar Content",
                        Permissions = new Permission[]
                        {
                            new Permission {
                                Title = "Create Calendars Events",
                                Claims = {
                                    new SecurityClaim(CalendarClaimTypes.CalendarEventGroupContribute, currentClientId)
                                }
                            }
                        }
                    }
            };
        }
    }
}
