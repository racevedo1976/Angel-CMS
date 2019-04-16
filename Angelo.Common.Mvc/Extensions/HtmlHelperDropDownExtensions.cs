using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Angelo.Common.Mvc
{
    /* This code is based off of the Unobtrusive Ajax dropdown list
    ** see Responsible Coder article:
    ** http://responsiblecoder.com/2011/06/asp-net-mvc3-app-part-2-ajax-cascading-dropdown/
    */

    public static class HtmlHelperDropDownExtensions
    {
        public static IHtmlContent AjaxDropDownlistFor<TModel, TProperty, TMasterProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            string optionLabel,
            IDictionary<String, Object> htmlAttributes,
            Expression<Func<TModel, TMasterProperty>> masterExpression,
            string serviceUrl)
        {
            htmlAttributes.Add("angelo-dropdown-dependson", htmlHelper.IdFor(masterExpression));
            htmlAttributes.Add("angelo-dropdown-loadfrom", serviceUrl);
            if (string.IsNullOrEmpty(optionLabel))
                return htmlHelper.DropDownListFor(expression, selectList, htmlAttributes);
            else
            {
                htmlAttributes.Add("angelo-dropdown-option-label", optionLabel);
                return htmlHelper.DropDownListFor(expression, selectList, optionLabel, htmlAttributes);
            }
        }

        public static IHtmlContent AjaxDropDownlistFor<TModel, TProperty, TMasterProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            Expression<Func<TModel, TMasterProperty>> masterExpression,
            string serviceUrl)
        {
            IDictionary<string, object> attr = new RouteValueDictionary();
            return AjaxDropDownlistFor(htmlHelper, expression, selectList, string.Empty, attr, masterExpression, serviceUrl);
        }

        public static IHtmlContent AjaxDropDownlistFor<TModel, TProperty, TMasterProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            IDictionary<String, Object> htmlAttributes,
            Expression<Func<TModel, TMasterProperty>> masterExpression,
            string serviceUrl)
        {
            return AjaxDropDownlistFor(htmlHelper, expression, selectList, string.Empty, htmlAttributes, masterExpression, serviceUrl);
        }

        public static IHtmlContent AjaxDropDownlistFor<TModel, TProperty, TMasterProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            object htmlAttributes,
            Expression<Func<TModel, TMasterProperty>> masterExpression,
            string serviceUrl)
        {
            IDictionary<string, object> attr = new RouteValueDictionary(htmlAttributes);
            return AjaxDropDownlistFor(htmlHelper, expression, selectList, string.Empty, attr, masterExpression, serviceUrl);
        }

        public static IHtmlContent AjaxDropDownlistFor<TModel, TProperty, TMasterProperty>(this IHtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            IEnumerable<SelectListItem> selectList,
            string optionLabel,
            object htmlAttributes,
            Expression<Func<TModel, TMasterProperty>> masterExpression,
            string serviceUrl)
        {
            IDictionary<string, object> attr = new RouteValueDictionary(htmlAttributes);
            return AjaxDropDownlistFor(htmlHelper, expression, selectList, optionLabel, attr, masterExpression, serviceUrl);
        }

    }
}
