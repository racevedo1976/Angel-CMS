using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;
using Angelo.Connect.Security.KnownClaims;
using Angelo.Connect.Calendar.Models;
using Angelo.Connect.Calendar.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Calendar.Security
{
    public class CalendarSecurityService
    {

        private IContextAccessor<SiteContext> _siteContextAccessor;
        private IContextAccessor<UserContext> _userContextAccessor;
        private CalendarDbContext _calendarDbContext;

        public CalendarSecurityService
        (
            IContextAccessor<SiteContext> siteContextAccessor,
            IContextAccessor<UserContext> userContextAccessor,
            CalendarDbContext calendarDbContext
        )
        {
            _siteContextAccessor = siteContextAccessor;
            _userContextAccessor = userContextAccessor;
            _calendarDbContext = calendarDbContext;
        }

        public bool AuthorizeForCreateEvents()
        {
            return AuthorizeForCreate();
        }

        public bool IsUserAuthenticated()
        {
            var userContext = _userContextAccessor.GetContext();

            //TODO find out how to implement security for calendar widget
            if (userContext.IsAuthenticated)
                return true;

            return false;
        }

        internal bool AuthorizeForCreate()
        {
            var userContext = _userContextAccessor.GetContext();

            // Has explicit permissions to create calendar events
            if (HasAnyClaim(CalendarClaimTypes.CalendarAuthor))
                return true;

            // Site Admins have implicit permissions to create calendars
            if (HasAnyClaim(SiteClaimTypes.SitePrimaryAdmin))
                return true;

            // Client Level Content Admin can create all content types
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Primary Admin can do everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;
            return false;
        }

        // NOTE: Implementation Example (MDJ)
        // TODO: Use this to secure editing CalendarEvents
        internal bool AuthorizeForEdit(Models.CalendarEvent calendarEvent)
        {
            var userContext = _userContextAccessor.GetContext();

            // Has Author permission and this is their event
            if (HasAnyClaim(CalendarClaimTypes.CalendarAuthor))
                return true;

            // Is a SiteAdmin and this is their event
            if (HasAnyClaim(SiteClaimTypes.SitePrimaryAdmin))
                return true;

            // Has been granted contribute permissions for this specific event 
            if (HasDelegateClaim(CalendarClaimTypes.CalendarEventContribute, calendarEvent))
                return true;

            // Client Level Content Admin can manage all user content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;
            return false;
        }

        // NOTE: Implementation Example (MDJ)
        // TODO: Use this to secure deleting CalendarEvents
        internal bool AuthorizeForDelete(Models.CalendarEvent calendarEvent)
        {
            var userContext = _userContextAccessor.GetContext();

            // Has Author permission and this is their event
            if (HasAnyClaim(CalendarClaimTypes.CalendarAuthor))
                return true;

            // Is a SiteAdmin and this is their event
            if (HasAnyClaim(SiteClaimTypes.SitePrimaryAdmin))
                return true;
     
            // Client Level Content Admin can manage all user content
            if (HasAdminClaim(ClientClaimTypes.UserContentManage))
                return true;

            // Client Level Primary Admin can manage everything
            if (HasAdminClaim(ClientClaimTypes.PrimaryAdmin))
                return true;
            return false;
        }


        // NOTE: We'll need to enforce security on CalendarEventGroups too (MDJ)
        // TODO: Implement the following methods for Calendar Event Groups
        /*
            internal bool AuthorizeForCreate(Models.CalendarEventGroup calendarEventGroup)
            internal bool AuthorizeForEdit(Models.CalendarEventGroup calendarEventGroup)
            internal bool AuthorizeForDelete(Models.CalendarEventGroup calendarEventGroup)
        */

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

        private bool HasAuthorClaim(string claimType, Models.CalendarEvent calendarEvent)
        {
            var userContext = _userContextAccessor.GetContext();

            if (userContext.UserId == calendarEvent.UserId)
            {
                // Not checking value because it doesn't matter which site issued this claim
                return userContext.SecurityClaims.Any(x => x.Type == claimType);
            }

            return false;
        }

        private bool HasDelegateClaim(string claimType, Models.CalendarEvent calendarEvent)
        {
            var userContext = _userContextAccessor.GetContext();

            //EG, has a claim against this specific event
            return userContext.SecurityClaims.Any(x => x.Type == claimType && x.Value == calendarEvent.EventId);
        }

        private bool HasDelegateClaim(string claimType, Models.CalendarEventGroup eventGroup)
        {
            var userContext = _userContextAccessor.GetContext();

            // Eg, has a claim against this specific event group
            return userContext.SecurityClaims.Any(x => x.Type == claimType && x.Value == eventGroup.EventGroupId);
        }

        public IList<string> GetEventGroupsSharedWithMe()
        {
            var userContext = _userContextAccessor.GetContext();

            var eventGroups = userContext.SecurityClaims.Where(x => x.Type == CalendarClaimTypes.CalendarEventGroupContribute).Select(x => x.Value).ToList();

            return eventGroups;
        }

        public List<CalendarEventGroup> GetEventGroupsSharedWithUser(UserContext userContext)
        {
            // TODO: Filter this list based on User's Claims
            var allowedIds = userContext.SecurityClaims
                .Where(x => x.Type == CalendarClaimTypes.CalendarEventGroupContribute)
                .Select(x => x.Value)
                .ToList();

            var categories = _calendarDbContext.CalendarEventGroups
                .AsNoTracking()
                .Where(x => allowedIds.Contains(x.EventGroupId)).ToList()
                .OrderBy(x => x.Title)
                .ToList();

            // Ensure navigation properties are null since not tracking 
            categories.ForEach(x => {
                x.WidgetGroups = null;
            });

            return categories;
        }
    }
}
