using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Filters;


namespace Angelo.Connect.Rendering.Filters
{
    public class ReturnUrlActionFilter : IActionFilter
    {

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.Controller is Controller)
            {
                var query = context.HttpContext.Request.Query;
                var cookies = context.HttpContext.Request.Cookies;
                var viewData = (context.Controller as Controller).ViewData;

                string returnUrl = null;

                if (query.ContainsKey("ru"))
                {
                    returnUrl = GetDecodedQueryValue(query, "ru");
                }
                else if (cookies.ContainsKey("ru"))
                {
                    returnUrl = cookies["ru"];
                }

                if(returnUrl != null)
                {
                    viewData["ReturnUrl"] = returnUrl;
                }
            }              
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Controller is Controller)
            {
                var cookies = context.HttpContext.Response.Cookies;
                var viewData = (context.Controller as Controller).ViewData;

                if(viewData["ReturnUrl"] != null)
                {
                    cookies.Append("ru", (string)viewData["ReturnUrl"]);
                }
            }
        }

        private string GetDecodedQueryValue(IQueryCollection query, string key)
        {
            string value = null;
            if (query.ContainsKey(key))
            {
                value = query[key].ToString();
                value = WebUtility.UrlDecode(value).Trim();
            }

            return value;
        }
    }
}
