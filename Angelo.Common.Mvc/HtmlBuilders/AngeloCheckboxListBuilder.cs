
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Text.Encodings.Web;

namespace Angelo.Common.Mvc
{
    public class AngeloCheckboxListBuilder<TModel> : IHtmlContent where TModel : class
    {
        protected IHtmlHelper htmlHelper;
        protected string controlName;
        protected TModel viewModel;
        protected IEnumerable<String> checkedValues;
        protected string checkedValuesPropertyName;
        protected IEnumerable<SelectListItem> selectListItems;
        protected string selectListItemsPropertyName;
        protected string fetchAction;
        protected string fetchUrl;
        protected bool autoHeight;
        

        public AngeloCheckboxListBuilder(IHtmlHelper helper, TModel model)
        {
            viewModel = model;
            htmlHelper = helper;
            checkedValues = null;
            selectListItems = null;
            autoHeight = true;
        }

        public AngeloCheckboxListBuilder<TModel> Name(string val)
        {
            controlName = val;
            return this;
        }

        public AngeloCheckboxListBuilder<TModel> Bind(IEnumerable<String> checkedItemValues, string modelPropertyName)
        {
            checkedValues = checkedItemValues;
            checkedValuesPropertyName = modelPropertyName;
            return this;
        }

        public AngeloCheckboxListBuilder<TModel> Bind(IEnumerable<SelectListItem> listItems, string modelPropertyName = null)
        {
            selectListItems = listItems;
            selectListItemsPropertyName = modelPropertyName;
            return this;
        }

        public AngeloCheckboxListBuilder<TModel> Bind(string modelPropertyName)
        {
            PropertyInfo pi = viewModel.GetType().GetProperty(modelPropertyName);
            if (pi != null)
            {
                if (typeof(IEnumerable<string>).IsAssignableFrom(pi.PropertyType))
                {
                    checkedValues = (IEnumerable<string>)pi.GetValue(viewModel, null);
                    checkedValuesPropertyName = modelPropertyName;
                }
                if (typeof(IEnumerable<SelectListItem>).IsAssignableFrom(pi.PropertyType))
                {
                    selectListItems = (IEnumerable<SelectListItem>)pi.GetValue(viewModel, null);
                    selectListItemsPropertyName = modelPropertyName;
                }
            }
            return this;
        }

        public AngeloCheckboxListBuilder<TModel> Bind(Expression<Func<TModel, object>> propertyLambda)
        {
            var me = propertyLambda.Body as MemberExpression;
            if (me == null)
                throw new ArgumentException("Invalid lambda expression defined in Bind method.");
            var modelPropertyName = me.Member.Name;
            return Bind(modelPropertyName);
        }

        public AngeloCheckboxListBuilder<TModel> FetchUrl(string url, string action = "POST")
        {
            fetchUrl = url;
            fetchAction = action;
            return this;
        }

        public AngeloCheckboxListBuilder<TModel> AutoHeight(bool val = true)
        {
            autoHeight = val;
            return this;
        }

        //public AngeloCheckboxListBuilder<TModel> Fetch<TController>(string controller, string action, object values) where TController : Controller
        //{

        //    var urlHelper = new UrlHelper(htmlHelper.ViewContext);
        //    var route = urlHelper.Action(new UrlActionContext(){ Action = action, Controller = controller, Values = values });

        //    //var client = new HttpClient();

        //    //client.PostAsync(route, values)



        //    //Type ct = ClientProductAppsDataController;
        //    var sp = htmlHelper.ViewContext.HttpContext.RequestServices;
        //    var ctr = sp.GetService(typeof(TController)) as TController;
        //    //var sp = Services.BuildServiceProvider();


        //    //var cb =  htmlHelper.ViewContext.

        //    //var s = ServiceProvider();
        //    var serviceCollection = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
        //    //var service = serviceCollection.


        //    return this;

        //}

        private TagBuilder BuildCheckboxRow(SelectListItem item, int index)
        {
            var idPrefix = controlName + "__" + index.ToString() + "_";

            var selTag = new TagBuilder("input");
            selTag.MergeAttribute("id", idPrefix + "Selected");
            selTag.MergeAttribute("hidden", "hidden");
            selTag.MergeAttribute("value", item.Selected ? "true" : "false");

            var textTag = new TagBuilder("input");
            textTag.MergeAttribute("id", idPrefix + "Text");
            textTag.MergeAttribute("hidden", "hidden");
            textTag.MergeAttribute("value", item.Text);

            var valueTag = new TagBuilder("input");
            valueTag.MergeAttribute("id", idPrefix + "Value");
            valueTag.MergeAttribute("hidden", "hidden");
            valueTag.MergeAttribute("value", item.Value);

            if (!string.IsNullOrEmpty(selectListItemsPropertyName))
            {
                var namePrefix =  selectListItemsPropertyName + "[" + index.ToString() + "].";
                selTag.MergeAttribute("name", namePrefix + "Selected");
                textTag.MergeAttribute("name", namePrefix + "Text");
                valueTag.MergeAttribute("name", namePrefix + "Value");
            }

            var checkboxTag = new TagBuilder("input");
            checkboxTag.MergeAttribute("id", idPrefix + "checkbox");
            if (!string.IsNullOrEmpty(checkedValuesPropertyName))
                checkboxTag.MergeAttribute("name", checkedValuesPropertyName);
            checkboxTag.MergeAttribute("type", "checkbox");
            checkboxTag.MergeAttribute("value", item.Value);
            checkboxTag.MergeAttribute("data-angelo-ctr-parent-id", controlName);
            checkboxTag.MergeAttribute("data-angelo-ctr-index", index.ToString());
            checkboxTag.MergeAttribute("data-angelo-ctr-target-id", selTag.Attributes["id"]);
            checkboxTag.MergeAttribute("onchange", "AngeloCheckboxListOnChange(this)");
            if (item.Selected)
                checkboxTag.MergeAttribute("checked", "checkted");

            var labelTag = new TagBuilder("label");
            labelTag.MergeAttribute("id", idPrefix + "label");
            labelTag.MergeAttribute("for", checkboxTag.Attributes["id"]);
            labelTag.InnerHtml.Append(item.Text);

            var colTag = new TagBuilder("div");
            colTag.AddCssClass("col-sm-12");
            colTag.InnerHtml.AppendHtml(selTag);
            colTag.InnerHtml.AppendHtml(textTag);
            colTag.InnerHtml.AppendHtml(valueTag);
            colTag.InnerHtml.AppendHtml(checkboxTag);
            colTag.InnerHtml.AppendHtml(labelTag);

            var rowTag = new TagBuilder("div");
            rowTag.AddCssClass("row");
            rowTag.MergeAttribute("id", idPrefix);
            rowTag.InnerHtml.AppendHtml(colTag);

            return rowTag;
        }

        private TagBuilder BuildContainerTag(List<SelectListItem> selectList)
        {
            var tag = new TagBuilder("div");
            tag.MergeAttribute("id", controlName + "_container");
            var index = 0;
            foreach (var item in selectList)
            {
                tag.InnerHtml.AppendHtml(BuildCheckboxRow(item, index));
                index++;
            }
            return tag;
        }

        private TagBuilder BuildControlTag(List<SelectListItem> selectList)
        {
            var tag = new TagBuilder("div");
            tag.AddCssClass("form-control");
            if (autoHeight)
                tag.MergeAttribute("style", "height:auto;");
            tag.MergeAttribute("id", controlName);
            tag.MergeAttribute("data-angelo-ctr-url", fetchUrl);
            tag.MergeAttribute("data-angelo-ctr-action", fetchAction);
            if (!string.IsNullOrEmpty(checkedValuesPropertyName))
                tag.MergeAttribute("data-angelo-ctr-value-list-name", checkedValuesPropertyName);
            if (!string.IsNullOrEmpty(selectListItemsPropertyName))
                tag.MergeAttribute("data-angelo-ctr-item-list-name", selectListItemsPropertyName);
            tag.MergeAttribute("data-angelo-ctr-count", selectList.Count.ToString());
            tag.InnerHtml.AppendHtml(BuildContainerTag(selectList));
            return tag;
        }

        private List<SelectListItem> BuildSelectList()
        {
            List<String> values;
            if (checkedValues == null)
                values = new List<String>();
            else
                values = checkedValues.ToList();

            var list = new List<SelectListItem>();
            if (selectListItems != null)
            {
                foreach(var item in selectListItems)
                {
                    list.Add(new SelectListItem()
                    {
                        Text = item.Text,
                        Value = item.Value,
                        Disabled = item.Disabled,
                        Selected = item.Selected || values.Contains(item.Value)
                    });
                }
            }
            return list;
        }

        public void WriteTo(TextWriter writer, HtmlEncoder encoder)
        {
            if (string.IsNullOrEmpty(controlName))
                controlName = Guid.NewGuid().ToString("N");
            var selectList = BuildSelectList();
            var tag = BuildControlTag(selectList);
            tag.WriteTo(writer, encoder);
        }
    }





}
