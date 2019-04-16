using System.Linq;
using System.Text;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

using IdentityServer4.Services;

using Angelo.Identity;
using Angelo.Identity.Models;
using Angelo.Aegis.Configuration;
using Angelo.Aegis.Internal;
using Angelo.Aegis.Messaging;
using Angelo.Aegis.UI.ViewModels.Account;
using Angelo.Aegis.UI.ViewModels;
using Angelo.Identity.Services;

namespace Angelo.Aegis.UI.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        private const string IgnorePassword = "~Ignore1";
        private const string CustomTokenSalt = "m2o0y3x5t5p3v4rre7v1";  //TODO: REMOVE after built-in token methods work correctly
        private IHostingEnvironment _environment;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly UserManager _userManager;
        private readonly SignInManager _signInManager;
        private readonly MessagingService _messaging;
        private readonly ILogger _logger;
        private readonly ICypher _cypher;
        private DataProtectorTokenProvider<User> _protect;
        private IDataProtectionProvider _dataProtectionProvider;
        private IHttpContextAccessor _httpContextAccessor;
        private AegisTenantResolver _aegisTenantResolver;
        private DirectoryManager _directoryManager;
       

        public AccountController(
            IHostingEnvironment environment,
            IIdentityServerInteractionService interaction,
            UserManager userManager,
            SignInManager signInManager,
            MessagingService messagingService,
            ILoggerFactory loggerFactory,
            ICypher cypher,
            IHttpContextAccessor httpContextAccessor,
            AegisTenantResolver aegisTenantResolver,
            DirectoryManager directoryManager
        )
        {
            _logger = loggerFactory.CreateLogger<AccountController>();

            _environment = environment;
            _interaction = interaction;
            _userManager = userManager;
            _signInManager = signInManager;
            _messaging = messagingService;
            _cypher = cypher;
            _httpContextAccessor = httpContextAccessor;
            _aegisTenantResolver = aegisTenantResolver;
            _directoryManager = directoryManager;
            
        }

        //
        // GET: /Account/Login
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null, string origin = null)
        {

            if (returnUrl != null)
            {
                origin = _cypher.Cypher(returnUrl);
            }

            string loginHint = null;
            var authContext = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (authContext != null)
            {
                // See if this is a register new user request.
                if (authContext.AcrValues.Contains("register"))
                {
                    return Register(origin);
                }

                loginHint = authContext.LoginHint;
            }
            
            // auto login when a hint is supplied in development mode
            if (_environment.IsDevelopment() && loginHint != null)
            {              
                var user = await _userManager.FindByNameAsync(loginHint);
                if (user != null)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    Redirect(returnUrl);
                }
            }

            // Otherwise continue with normal login process
            if (origin != null)
            {
                ViewData["Origin"] = origin;
            }

            var model = new LoginViewModel() { Username = loginHint };

            return View(model);
        }


        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string origin = null)
        {            
            ViewData["Origin"] = origin;
          
            if (ModelState.IsValid)
            {
                var tenant = await GetCurrentTenantAsync();
                var result = await _signInManager.PasswordSignInAsync(tenant.TenantKey, model.Username, model.Password, model.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation(1, "User logged in.");

                    return RedirectToOrigin(origin);
                }

                if (result.MustChangePassword)
                {
                    //TODO: PasswordResetToken isn't being consumed successfully. Workaround is to use custom token.
                    //var passwordResetToken = _userManager.GeneratePasswordResetTokenAsync(user);
                    var user = result.User;
                    var passwordResetToken = sha256_hash(user.Id + CustomTokenSalt);

                    return RedirectToAction("ForceResetPassword", "Account", new { userId = user.Id, code = passwordResetToken, forced = true, origin = GetWebLoginLinkFromOrigin(origin) });
                }

                if (result.RequiresTwoFactor)
                {
                    return RedirectToAction(nameof(SendCode), new { Origin = origin, RememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    _logger.LogWarning(2, "User account locked out.");
                    return View("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, result.ToString());
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string origin = null)
        {
            if (origin != null)
            {
                ViewData["Origin"] = origin;
            }

            ViewData["wirelessProviderSelectList"] = GetWirelessProviderListAsync().Result;

            var model = new RegisterViewModel();
            model.DirectoryId = GetDirectoryIdFromRequestAsync().Result;
            return View("Register", model);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string origin = null)
        {
            return await RegisterInternal(model, origin, isExternal: false);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LogOff(string post_logout_redirect_uri)
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation(4, "User logged out.");
            return Redirect(post_logout_redirect_uri);
        }

        //
        // POST: /Account/ExternalLogin
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ExternalLogin(string provider, string origin = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { Origin = origin });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            return Challenge(properties, provider);
        }

        //
        // GET: /Account/ExternalLoginCallback
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string origin = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Error from external provider: {remoteError}");
                return View(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login), new { Origin = origin });
            }

            // Sign in the user with this external login provider if the user already has a login.
            var tenant = await GetCurrentTenantAsync();
            var result = await _signInManager.ExternalLoginSignInAsync(tenant.TenantKey, info.LoginProvider, info.ProviderKey, isPersistent: false);           

            if (result.Succeeded)
            {
                _logger.LogInformation(5, "User logged in with {Name} provider.", info.LoginProvider);

                return RedirectToOrigin(origin);
            }

            if (result.RequiresTwoFactor)
            {
                return RedirectToAction(nameof(SendCode), new { Origin = origin });
            }

            if (result.IsLockedOut)
            {
                return View("Lockout");
            }
            else
            {
                // If the user does not have an account, then prompt the user to regiser
                ViewData["Origin"] = origin;
                ViewData["LoginProvider"] = info.LoginProvider;
                ViewData["wirelessProviderSelectList"] = await GetWirelessProviderListAsync();

                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("Register", new RegisterViewModel
                {
                    Email = email,
                    Password = IgnorePassword,
                    ConfirmPassword = IgnorePassword,
                    IsExternal = true
                });
            }
        }

        //
        // POST: /Account/ExternalLoginDone
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginDone(RegisterViewModel model, string origin = null)
        {
            return await RegisterInternal(model, origin, isExternal: true);
        }

        private async Task<IActionResult> RegisterInternal(RegisterViewModel model, string origin, bool isExternal)
        {
            if (model != null) model.IsExternal = isExternal;   // Prevent requests from being able to set the model property (should be publicly readonly anyway)

            if (ModelState.IsValid)
            {
                var info = default(ExternalLoginInfo);
                if (isExternal)
                {
                    // Get the information about the user from the external login provider
                    info = await _signInManager.GetExternalLoginInfoAsync();
                    
                    if (info == null)
                    {
                        return View("ExternalLoginFailure");
                    }
                }

                var user = new User
                {
                    UserName = model.Username ?? model.Email,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    WirelessProviderId = model.WirelessProviderId,
                    DirectoryId = model.DirectoryId,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    DisplayName = model.DisplayName
                };

                // For now, confirm the phone numbers until we implement the verify sms flow.
                if ((user.PhoneNumber != null) && (user.PhoneNumber.Where(Char.IsDigit).Count() == 10))
                    user.PhoneNumberConfirmed = true;

                var result = await (isExternal ? _userManager.CreateAsync(user) : _userManager.CreateAsync(user, model.Password));

                if (result.Succeeded)
                {
                    if (isExternal)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                    }

                    if (result.Succeeded)
                    {
                        await SendWelcomeEmailAsync(user, origin);

                        if (isExternal)
                        {
                            _logger.LogInformation(6, "User created an account using {Name} provider.", info.LoginProvider);
                        }
                        else
                        {
                            _logger.LogInformation(3, "User created a new account with password.");
                        }

                        ViewData["Origin"] = GetWebLoginLinkFromOrigin(origin);

                        return View("RegisterDone");
                    }
                }
                AddErrors(result);
            }
            ViewData["Origin"] = origin;
            ViewData["wirelessProviderSelectList"] = await GetWirelessProviderListAsync();
            return View("Register", model);
        }

        // GET: /Account/ConfirmEmail
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code, string webLogin)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                ViewData["webLoginLink"] = webLogin;
                return View("ConfirmEmail");
            }
            else
                return View("Error");
        }

        // GET: /Account/NewUserSetPassword
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> NewUserSetPassword(string userId, string code, string ru)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            
            var user = await _userManager.FindByIdAsync(userId);
           
            if (user == null)
            {
                return View("Error");
            }

            var result = await _userManager.VerifyUserTokenAsync(user, nameof(ConnectCustomTokenProvider<User>), "NewUserPassword", code);
            if (!result)
            {
                return View("Error", new ErrorViewModel()
                {
                    Error = new IdentityServer4.Models.ErrorMessage
                    {
                        ErrorDescription = "Invalid Token."
                    }
                });
            }


            //Since user is trying to get to this password reset page, meaning email is verified as well.
            var emailVerificationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var emailConfirmResult = await _userManager.ConfirmEmailAsync(user, emailVerificationToken);
            if (!emailConfirmResult.Succeeded)
            {
                AddErrors(emailConfirmResult);
                return View("Error", new ErrorViewModel()
                {
                    Error = new IdentityServer4.Models.ErrorMessage
                    {
                        ErrorDescription = emailConfirmResult.Errors.FirstOrDefault()?.Description
                    }
                });
            }

            //if all checks out for this user....
            //Generate token for updating the password.


            var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);

            SetNewPasswordViewModel model = new SetNewPasswordViewModel
            {
                UserId = user.Id,
                Token = passwordResetToken,
                UserName = user.DisplayName,
                ReturnUrl = ru
            };
            
            return View(model);
        }

        //Post: /Account/NewUserSetPassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> NewUserSetPassword(SetNewPasswordViewModel model)
        {

            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(model.UserId);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return RedirectToAction(nameof(AccountController.NewUserSetPassword));
                }

                var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
                
                if (!result.Succeeded)
                {
                    AddErrors(result);
                    return View(model);

                }

                //ViewData["ReturnUrl"] = model.ReturnUrl;

                return RedirectToAction(nameof(AccountController.ResetPasswordDone), new { returnUrl = model.ReturnUrl});

            }
            return View(model);
        }



        //
        // GET: /Account/ForgotPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword(string origin = null)
        {
            ViewData["Origin"] = GetWebLoginLinkFromOrigin(origin);

            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, string origin = null)
        {
            ViewData["Origin"] = origin;

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null)
                {
                    // Don't reveal that the user does not exist
                    return View("ForgotPasswordDone");
                }

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=532713
                // Send an email with this link
                var passwordResetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = passwordResetToken, origin = origin }, protocol: HttpContext.Request.Scheme);
                var resetPasswordEmail = new Messaging.Models.ResetPasswordEmail()
                {
                    Username = user.UserName,
                    EmailLink = callbackUrl
                };

                await _messaging.SendEmailAsync(model.Email, resetPasswordEmail);

                return View("ForgotPasswordDone");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ForgotPasswordConfirmation
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordDone()
        {
            return View();
        }

        //
        // GET: /Account/ForceResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForceResetPassword(string userId, string code = null, string origin = null)
        {
            ViewData["Origin"] = origin;
            var model = new ForceResetPasswordViewModel
            {
                UserId = userId,
                Code = code
            };

            return code == null ? View("Error") : View(model);
        }

        //
        // GET: /Account/ResetPassword
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string userId, string code = null, string origin = null)
        {
            ViewData["Origin"] = origin;
            var model = new ResetPasswordViewModel
            {
                UserId = userId,
                Code = code
            };

            return code == null ? View("Error") : View(model);
        }

        //
        // POST: /Account/ForceResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForceResetPassword(ForceResetPasswordViewModel model, string origin)
        {
            ViewData["Origin"] = origin;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return View("ResetPasswordDone");
            }

            IdentityResult result = new IdentityResult();
            var checkResult = await _signInManager.CheckPasswordSignInAsync(user, model.Password, lockoutOnFailure: false);

            if (checkResult.Succeeded)
            {
                //check hashed token then change password
                var passwordResetToken = sha256_hash(user.Id + CustomTokenSalt);
                if (passwordResetToken == model.Code)
                {
                    result = await _userManager.ChangePasswordAsync(user, model.Password, model.NewPassword);

                    if (result.Succeeded)
                    {
                        user.MustChangePassword = false;
                        await _userManager.UpdateAsync(user);
                    }
                }
            }

            if (result.Succeeded)
            {
                return View("ResetPasswordDone");
            }

            if (!checkResult.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Current Password is not correct.");
            }

            AddErrors(result);
            return View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, string origin)
        {
            ViewData["Origin"] = origin;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return View("ResetPasswordDone");
            }

            IdentityResult result = new IdentityResult();

            result = await _userManager.ResetPasswordAsync(user, model.Code, model.NewPassword);

            if (result.Succeeded)
            {
                user.MustChangePassword = false;
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);

                return View("ResetPasswordDone");
            }

            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordDone
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordDone(string returnUrl)
        {
            ViewData["returnUrl"] = returnUrl;
            return View();
        }

        //
        // GET: /Account/SendCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string origin = null, bool rememberMe = false)
        {
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                return View("Error");
            }

            var userFactors = await _userManager.GetValidTwoFactorProvidersAsync(user);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();

            ViewData["Origin"] = origin;
            return View(new SendCodeViewModel { Providers = factorOptions, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendCode(SendCodeViewModel model, string origin = null)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return View("Error");
            }

            // Generate the token and send it
            var code = await _userManager.GenerateTwoFactorTokenAsync(user, model.SelectedProvider);

            if (string.IsNullOrWhiteSpace(code))
            {
                return View("Error");
            }

            var message = "Your security code is: " + code;

            if (model.SelectedProvider == "Email")
            {
                await _messaging.SendEmailAsync(await _userManager.GetEmailAsync(user), "Security Code", message);
            }
            else if (model.SelectedProvider == "Phone")
            {
                await _messaging.SendSmsAsync(await _userManager.GetPhoneNumberAsync(user), message);
            }

            return RedirectToAction(nameof(VerifyCode), new { Origin = origin, Provider = model.SelectedProvider, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/VerifyCode
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode(string provider, bool rememberMe, string origin = null)
        {
            // Require that the user has already logged in via username/password or external login
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
                return View("Error");

            ViewData["Origin"] = origin;
            return View(new VerifyCodeViewModel { Provider = provider, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model, string origin = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            // The following code protects for brute force attacks against the two factor codes.
            // If a user enters incorrect codes for a specified amount of time then the user account
            // will be locked out for a specified amount of time.
            var result = await _signInManager.TwoFactorSignInAsync(model.Provider, model.Code, model.RememberMe, model.RememberBrowser);

            if (result.Succeeded)
                return RedirectToOrigin(origin);

            if (result.IsLockedOut)
            {
                _logger.LogWarning(7, "User account locked out.");
                return View("Lockout");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid code.");
                return View(model);
            }
        }

        /// <summary>
        /// Show logout page
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string logoutId)
        {
            var vm = new LogoutViewModel
            {
                LogoutId = logoutId
            };

            return View(vm);
        }

        /// <summary>
        /// Handle logout page postback
        /// </summary>
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout(LogoutViewModel model)
        {
            // delete authentication cookie
            await _signInManager.SignOutAsync();

            // get context information (client name, post logout redirect URI and iframe for federated signout)
            var logout = await _interaction.GetLogoutContextAsync(model.LogoutId);
            if (!string.IsNullOrEmpty(logout?.PostLogoutRedirectUri))
                Redirect(logout.PostLogoutRedirectUri);

            var vm = new LoggedOutViewModel
            {
                PostLogoutRedirectUri = logout?.PostLogoutRedirectUri,
                ClientName = logout?.ClientId,
                SignOutIframeUrl = logout?.SignOutIFrameUrl
            };

            return View("LoggedOut", vm);
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private Task<User> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        private IActionResult RedirectToOrigin(string origin, bool allowExternalRedirect = false)
        {
            var returnUrl = _cypher.Decypher(origin);

            if (Url.IsLocalUrl(returnUrl) || allowExternalRedirect)
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }


        private static string GetFirstName(ExternalLoginInfo info)
        {
            var name = info?.Principal?.Identity?.Name;

            return (name?.Contains(" ") ?? false) ? name.Substring(0, name.IndexOf(" ")).Trim() : null;
        }

        private static string GetLastName(ExternalLoginInfo info)
        {
            var name = info?.Principal?.Identity?.Name;

            return (name?.Contains(" ") ?? false) ? name.Substring(name.LastIndexOf(" ")).Trim() : name;
        }

        private async Task SendWelcomeEmailAsync(User user, string origin)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            string webLoginLink = GetWebLoginLinkFromOrigin(origin);
            var link = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = token, webLogin = webLoginLink }, Request.Scheme, Request.Host.Value);

            var welcomeEmail = new Messaging.Models.WelcomeEmail()
            {
                Username = user.UserName,
                ConfirmEmailLink = link
            };

            await _messaging.SendEmailAsync(user.Email, welcomeEmail);
        }


        private async Task<AegisTenant> GetCurrentTenantAsync()
        {
            var tenant = HttpContext.GetTenant<AegisTenant>();

            return await Task.FromResult(tenant);
        }

        private async Task<List<SelectListItem>> GetWirelessProviderListAsync()
        {
            var providers = await _userManager.GetWirelessProvidersAsync();
            var providerSelectList = providers.Select(p => new SelectListItem() { Value = p.Id, Text = p.Name }).ToList();
            providerSelectList.Insert(0, new SelectListItem() { Value = "", Text = "----- Select -----" });
            return providerSelectList;
        }

        private async Task<string> GetDirectoryIdFromRequestAsync()
        {
            var aegisContext = await _aegisTenantResolver.ResolveAsync(HttpContext);
            var directories = await _directoryManager.GetDirectoriesAsync(aegisContext?.Tenant?.TenantKey);
            if (directories.Count > 0)
                return directories.First().Id;
            else
                return null;
        }

        private string GetWebLoginLinkFromOrigin(string origin)
        {
            const string relativeLoginPath = "/sys/account/login";
            const string redirect_uri = "redirect_uri=";

            string webLoginLink = string.Empty;
            try
            {
                // Get the original redirect_uri value from the origin
                var redirect1 = _cypher.Decypher(origin);
                var pos1 = redirect1.IndexOf(redirect_uri) + redirect_uri.Length;
                var pos2 = redirect1.IndexOf('&', pos1);
                if ((pos1 > -1) && (pos2 > pos1))
                {
                    var encodedRedirectUri = redirect1.Substring(pos1, pos2 - pos1);
                    var redirectUri = System.Net.WebUtility.UrlDecode(encodedRedirectUri);
                    // Build the web login link using the authority found in the redirect_uri. 
                    var originalUri = new Uri(redirectUri, UriKind.Absolute);
                    webLoginLink = string.Format("{0}://{1}{2}", originalUri.Scheme, originalUri.Authority, relativeLoginPath);
                }
            }
            catch (Exception)
            {
                // ignore errors
            }
            return webLoginLink;
        }
        #endregion

        public static String sha256_hash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                Byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (Byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
