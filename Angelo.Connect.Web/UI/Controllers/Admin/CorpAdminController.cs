using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

using Angelo.Connect.Configuration;
using Angelo.Connect.Security;


namespace Angelo.Connect.Web.UI.Controllers.Admin
{
    public class CorpAdminController : AdminController
    {
        public CorpAdminController
        (
            ILogger<CorpAdminController> logger
        )
        :base(logger)
        {

        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        public IActionResult Dashboard()
        {
            return View();
        }

        [Authorize(policy: PolicyNames.StubPolicy)]
        public IActionResult Jobs()
        {
            return View();
        }

        [Authorize(policy: PolicyNames.CorpClientsRead)]
        public IActionResult Clients()
        {
            return View();
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        public IActionResult Sites()
        {
            return View();
        }

        [Authorize(policy: PolicyNames.CorpSiteCanManage)]
        public IActionResult LdapSettings()
        {
            return View();
        }

        /*
        [Authorize(policy: PolicyNames.StubPolicy)]
        public async Task<IActionResult> Aegis([FromServices] IOptions<AegisOptions> aegisOptions)
        {
            var client = new HttpClient();
            var options = aegisOptions.Value;

            client.SetBearerToken(User.GetAccessToken());

            ViewBag.OpenIdConfig = JObject.Parse(
                await client.GetStringAsync(options.EndPoints.Meta)
            );

            ViewBag.UserInfo = JObject.Parse(
                await client.GetStringAsync(options.EndPoints.UserInfo)
            );

            return View();
        }
        */
    }
}
