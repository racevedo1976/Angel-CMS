using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Web.Config;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Web.UI.Controllers
{
    public class ClientControllerBase : AdminController
    {
       
        protected IAuthorizationService AuthorizationService { get; private set; }
        
        protected Client Client { get; private set; }
        protected ProductContext App { get; private set; }

        public ClientControllerBase(ClientAdminContextAccessor clientContextAccessor, IAuthorizationService authorizationService, ILogger logger) : base(logger)
        {
            var context = clientContextAccessor.GetContext();
            Client = context?.Client;
            App = context?.Product;

            // fail now since all subsequent actions & security depend on knowing which client we're dealing with 
            if (Client == null)
            {
                logger.LogError($"ClientAdminContext.Client was null during initialization of {nameof(ClientControllerBase)}");

                throw new ArgumentNullException("ClientAdminContext.Client cannot be null for Client Admin Controllers to function properly.");
            }

            AuthorizationService = authorizationService;
        }

        protected IActionResult ClientNotFound()
        {
            // TODO: Make this more robust
            return NotFound();
        }

    }
}
