using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using IdentityServer4.Services;
using Angelo.Aegis.UI.ViewModels;

namespace Angelo.Aegis.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private CultureOptions _cultureOptions;

        public HomeController(IIdentityServerInteractionService interaction, IOptions<CultureOptions> cultureOptions)
        {
            _interaction = interaction;
            _cultureOptions = cultureOptions.Value;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost, Route("/culture")]
        public IActionResult Culture(string culture, string returnUrl)
        {
            _cultureOptions.SetCultureCookie(this.HttpContext, culture);

            return LocalRedirect(returnUrl);
        }

        /// <summary>
        /// Shows the error page
        /// </summary>
        public async Task<IActionResult> Error(string errorId)
        {
            var vm = new ErrorViewModel();

            // retrieve error details from identityserver
            var message = await _interaction.GetErrorContextAsync(errorId);
            if (message != null)
            {
                vm.Error = message;
            }

            return View("Error", vm);
        }
    }
}
