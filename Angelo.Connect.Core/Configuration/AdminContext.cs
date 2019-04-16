using System;
using System.Collections.Generic;

using Angelo.Connect.Security;
using Angelo.Connect.Abstractions;

namespace Angelo.Connect.Configuration
{
    public class AdminContext
    {
        private IContextAccessor<UserContext> _userContextAccessor;
        private IContextAccessor<ClientAdminContext> _clientContextAccessor;
        private IContextAccessor<SiteAdminContext> _siteContextAccessor;
        public AdminContext
        (
            IContextAccessor<UserContext> userContextAccessor,
            IContextAccessor<ClientAdminContext> clientContextAccessor,
            IContextAccessor<SiteAdminContext> siteContextAccessor
        )
        {
            _userContextAccessor = userContextAccessor;
            _clientContextAccessor = clientContextAccessor;
            _siteContextAccessor = siteContextAccessor;
        }

        public string CorpId { get; } = ConnectCoreConstants.CorporateId;

        public string ClientId
        {
            get {
                return _clientContextAccessor.GetContext().Client?.Id;
            }
        }
        public string SiteId
        {
            get {
                return _siteContextAccessor.GetContext().Site?.Id;
            }
        }

        public SiteAdminContext SiteContext
        {
            get {
                return _siteContextAccessor.GetContext();
            }
        }

        public ClientAdminContext ClientContext
        {
            get {
                return _clientContextAccessor.GetContext();
            }
        }

        public UserContext UserContext
        {
            get {
                return _userContextAccessor.GetContext();
            }
        }

    }
}
