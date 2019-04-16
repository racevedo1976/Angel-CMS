using System;
using System.Dynamic;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Angelo.Connect.Web.Controllers
{
    public class ComponentsController : Controller
    {
        private IViewComponentSelector _selector;

        public ComponentsController(IViewComponentSelector selector)
        {
            _selector = selector;
        }

        [ResponseCache(Duration = 0)]
        public IActionResult Render(string type, string cid = null)
        {
            var arguments = ToExpando(HttpContext.Request.Query, type);

            ViewData["cid"] = cid ?? type;

            return ViewComponent(type, arguments);
        }

        private ExpandoObject ToExpando(IQueryCollection query, string viewComponentType)
        {
            ViewComponentDescriptor vcDescriptor = _selector.SelectComponent(viewComponentType);
            if (vcDescriptor == null)
                throw new Exception($"Unable to find definition of ViewComponent ({viewComponentType})");
            MethodBase invokeMethod = vcDescriptor.TypeInfo.GetMethod("InvokeAsync");
            if (invokeMethod == null)
                throw new Exception($"InvokeAsync method not found in ViewComponent ({viewComponentType})");
            ParameterInfo[] paramInfo = invokeMethod.GetParameters();

            var expando = new ExpandoObject() as IDictionary<string, object>;
            foreach (var param in paramInfo)
            {
                if (query.ContainsKey(param.Name))
                {
                    if (param.ParameterType == typeof(string))
                        expando.Add(param.Name, query[param.Name].ToString());

                    else if (param.ParameterType == typeof(bool))
                        expando.Add(param.Name, Convert.ToBoolean(query[param.Name]));

                    else if (param.ParameterType == typeof(int))
                        expando.Add(param.Name, Convert.ToInt32(query[param.Name]));

                    else if (param.ParameterType == typeof(double))
                        expando.Add(param.Name, Convert.ToDouble(query[param.Name]));

                    else if (param.ParameterType == typeof(DateTime))
                        expando.Add(param.Name, Convert.ToDateTime(query[param.Name]));

                    // TO DO: Add conversions for arrays, collections, and classes
                }
            }
            return (ExpandoObject)expando;
        }

    }

}
