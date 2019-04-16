using System;
using System.Collections;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Rendering;

/*
namespace Angelo.Connect.Web
{
    public static class HtmlHelperViewExtensions
    {

        /// <summary>
        /// Renders the HTML returned from an action
        /// </summary>
        public static IHtmlContent RenderAction<TModel>(this IHtmlHelper<TModel> helper, string action, string controller, object parameters)
        {
            var url = GetActionUrl(helper, action, controller);

            var content = RenderActionUrl(helper, url, parameters);
            return new HtmlString(content);
        }

        /// <summary>
        /// Renders the HTML returned from an action
        /// </summary>
        public static IHtmlContent RenderAction<TModel>(this IHtmlHelper<TModel> helper, string action, object parameters)
        {
            var url = action.StartsWith("/") || action.StartsWith("~/")
                ? action.Replace("~", "")
                : GetActionUrl(helper, action);

            var content = RenderActionUrl(helper, url, parameters);
            return new HtmlString(content);
        }

        #region private helpers

        private static string RenderActionUrl<TModel>(IHtmlHelper<TModel> helper, string url, object parameters)
        {
            var request = helper.ViewContext?.HttpContext?.Request;
            var user = helper.ViewContext?.HttpContext?.User;

            if (request == null)
                throw new NullReferenceException($"Null Request Context. Cannot Render {url}");

            var httpClient = new HttpClient();
            var query = "";

            httpClient.DefaultRequestHeaders.Add("cookie", request.Headers["cookie"].AsEnumerable());
            httpClient.BaseAddress = new Uri(request.Scheme + "://" + request.Host);

            if(user != null)
            {
                //var token = user.GetAccessToken();
                var token = request.Cookies["token"];
                httpClient.SetBearerToken(token);
            }
            
            if (parameters != null)
                query = GetQueryString(helper, parameters);

            var response = httpClient.GetAsync(url + query).Result;
            httpClient.Dispose();

            if (response.IsSuccessStatusCode)
                return response.Content.ReadAsStringAsync().Result;

            return String.Format(
                "ERROR {0}: {1} - {2}",
                response.StatusCode.ToString(),
                response.ReasonPhrase,
                url
            );
        }

        private static string GetActionUrl<TModel>(IHtmlHelper<TModel> helper, string action, string controller = null)
        {
            var context = helper.ViewContext.ActionDescriptor as ControllerActionDescriptor;
            var currentPath = helper.ViewContext?.HttpContext?.Request?.Path.ToString().ToLower();
            var basePath = Regex.Split(currentPath, context.ControllerName.ToLower())[0];

            return basePath + (controller ?? context.ControllerName) + "/" + action;
        }

        private static string GetQueryString(IHtmlHelper helper, object parameters)
        {
            var sb = new StringBuilder();

            foreach (PropertyInfo prop in parameters.GetType().GetProperties())
            {
                var value = Convert.ToString(prop.GetValue(parameters));

                sb.Append(helper.UrlEncoder.Encode(prop.Name) + "=");
                sb.Append(helper.UrlEncoder.Encode(value) + "&");
            }

            var result = sb.ToString();

            if (result.Length > 0)
                result = "?" + result.Substring(0, result.Length - 1);

            return result;
        }

        #endregion
    }
}

*/