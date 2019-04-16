using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Angelo.Connect.Configuration;
using Angelo.Connect.Logging;

namespace Angelo.Connect.Web
{
    //
    // TODO: Change to a class instance and accept configuration in the constructor
    //
    public class OpenIdInterceptors : IOpenIdConnectEvents
    {
        private ILogger _logger;
        private ServerOptions _serverOptions;

        public OpenIdInterceptors(DbLoggerProvider logProvider, IOptions<ServerOptions> serverOptions)
        {
            _logger = logProvider.CreateLogger("OIDC Events");
            _serverOptions = serverOptions.Value;
        }

        public Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            _logger.LogError("Authentication Failed", context.Exception);
            return Task.FromResult(0);
        }

        public Task AuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
        {
            _logger.LogInformation("Authorization Code Received", context.ProtocolMessage);
            return Task.FromResult(0);
        }

        public Task MessageReceived(MessageReceivedContext context)
        {
            return Task.FromResult(0);
        }

        public Task RedirectToIdentityProvider(RedirectContext context)
        {
            var services = context.HttpContext.RequestServices;
            var site = services.GetService<SiteContext>() as SiteContext;

            // add tenant key if present
            if (site != null)
            {
                context.ProtocolMessage.AcrValues = $"tenant:{site.SecurityPoolId}";
            }

            // alter returnUrl to use HTTPS (fix for ARR proxying HTTPS as HTTP)
            if(_serverOptions.UseHttpsRoutes)
            {
                context.ProtocolMessage.RedirectUri = context.ProtocolMessage.RedirectUri.Replace("http://", "https://");
            }

            // add registeration acr token if present
            var requestUrl = context.HttpContext.Request.Path.ToString().ToLower();
            if (requestUrl.Contains("/register"))
            {
                context.ProtocolMessage.AcrValues += " register";
            }

            // add loginhint if present 
            if (context.Request.Query.ContainsKey("login"))
            {
                context.ProtocolMessage.LoginHint = context.Request.Query["login"].ToString();
            }

            _logger.LogInformation("Redirecting to Identity Provider, " + context.Options.Authority, context.ProtocolMessage);

            return Task.FromResult(0);
        }

        public Task RedirectToIdentityProviderForSignOut(RedirectContext context)
        {
            if (_serverOptions.UseHttpsRoutes)
            {
                context.ProtocolMessage.PostLogoutRedirectUri = context.ProtocolMessage.PostLogoutRedirectUri.Replace("http://", "https://");
            }

            return Task.FromResult(0);
        }

        public Task RemoteSignOut(RemoteSignOutContext context)
        {
            _logger.LogInformation("Remote Sign Out", context.ProtocolMessage);

            return Task.FromResult(0);
        }

        public Task TokenResponseReceived(TokenResponseReceivedContext context)
        {
            _logger.LogInformation("Token Response", context.TokenEndpointResponse);

            return Task.FromResult(0);
        }

        public Task TokenValidated(TokenValidatedContext context)
        {
            _logger.LogInformation("Token Validated", context.SecurityToken);

            return Task.FromResult(0);
        }

        public Task UserInformationReceived(UserInformationReceivedContext context)
        {
            _logger.LogInformation("User Info Received", context.ProtocolMessage);

            return Task.FromResult(0);
        }

        public Task RemoteFailure(FailureContext context)
        {
            _logger.LogError("Remote Failure", context.Failure);

            return Task.FromResult(0);
        }

        public Task TicketReceived(TicketReceivedContext context)
        {
            /*
           var userId = context.HttpContext.User.GetUserId();
           var token = context.ProtocolMessage.AccessToken;

           context.HttpContext.Response.Cookies.Append("token", token);
           */

            _logger.LogInformation("Ticket Received", context.Ticket);

            return Task.FromResult(0);
        }
    }
}
