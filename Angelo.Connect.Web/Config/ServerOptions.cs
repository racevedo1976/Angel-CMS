using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;


namespace Angelo.Connect.Web
{
    public class ServerOptions
    {
        private int _cultureCookieDaysToKeep = 60;

        public bool UseHttpsRoutes { get; set; }

        public bool UseHttpsRewriter { get; set; }

        public int SessionMinutes { get; set; } = 30;

        public string ResourcePath { get; set; }

        public string DefaultCulture { get; set; }

        public List<string> SupportedCultures { get; set; }

     
        /// <summary>
        /// Returns the CultureInfo object for the system's default culture
        /// </summary>
        public CultureInfo GetDefaultCulture()
        {
            return new CultureInfo(DefaultCulture);
        }

        /// <summary>
        /// Returns the CultureInfo collection for the system's supported cultures
        /// </summary>
        public IEnumerable<CultureInfo> GetSupportedCultures()
        {
            return SupportedCultures.Select(key => new CultureInfo(key)).ToList();
        }

        /// <summary>
        /// Custom culture provider that set the culture based on either a QueryString or Cookie entry named "culture".
        /// </summary>
        public async Task<ProviderCultureResult> GetRequestCulture(HttpContext context)
        {
            string cultureKey = DefaultCulture;

            if (context.Request.Query.ContainsKey("culture"))
            {
                cultureKey = context.Request.Query["culture"];
                SetRequestCulture(context, cultureKey);
            }

            if (context.Request.Cookies.ContainsKey("culture"))
                cultureKey = context.Request.Cookies["culture"];

            return await Task.FromResult(new ProviderCultureResult(cultureKey));
        }

        public void SetRequestCulture(HttpContext context, string cultureKey)
        {
            context.Response.Cookies.Append("culture", cultureKey, new CookieOptions
            {
                Expires = DateTime.Now.AddDays(_cultureCookieDaysToKeep)
            });
        }

        public void SetRequestLocatizationOptions(RequestLocalizationOptions options)
        {
            var supportedCultures = GetSupportedCultures().ToList();

            options.DefaultRequestCulture = new RequestCulture(DefaultCulture);
            options.SupportedCultures = supportedCultures;
            options.SupportedUICultures = supportedCultures;
            options.RequestCultureProviders = new List<IRequestCultureProvider>()
            {
                new CustomRequestCultureProvider(GetRequestCulture)
            };
        }

    }
}
