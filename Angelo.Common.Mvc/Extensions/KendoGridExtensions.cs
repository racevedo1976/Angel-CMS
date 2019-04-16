using System;
using System.Collections.Generic;
using System.Linq;
using Kendo.Mvc.UI.Fluent;
using System.Text;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Reflection;

namespace Angelo.Connect.Kendo.Extensions
{
    public static class KendoGridExtensions
    {

        /// <summary>
        /// Creates a clickable column that will trigger a custom event to be handled by the view component.
        /// </summary>
        /// <param name="componentId">The id of the view component wrapper control.</param>
        /// <param name="eventName">The name of the custom event to be triggered (note: the full event name will be "componentId.eventName"</param>
        /// <param name="configureData">The strings or lamdas specifing the data model values to pass in the event.</param>
        /// <param name="template">The HTML to display for this column.</param>
        public static GridTemplateColumnBuilder<T> CustomEvent<T>(this GridColumnFactory<T> columnFactory,
            string componentId, string eventName, Action<DataBuilder<T>> configureData, string template) where T : class
        {
            var dataBuilder = new DataBuilder<T>();
            configureData(dataBuilder);

            var sb = new StringBuilder();
            sb.Append($"<div data-kendo-event-cid=\"{componentId}\" data-kendo-event-name=\"{eventName}\"");
            foreach(var item in dataBuilder.Items)
            {
                string name = Regex.Replace(item.Key, @"([A-Z])", "-$1").ToLower(); // convert camel case to hyphened
                string value = item.Value.Replace("\"", "&quot;").Replace("&", "&amp;");
                sb.Append($" data-kendo-param{name}=\"{value}\"");
            }
            sb.Append(" style = \"cursor: pointer\"");
            sb.Append(">");
            sb.Append(template);
            sb.Append("</div>");
            var templateStr = sb.ToString();

            return columnFactory.Template(templateStr);
        }

        /// <summary>
        /// Creates a clickable column that will trigger a custom event to be handled by the view component.
        /// </summary>
        /// <param name="componentId">The id of the view component wrapper control.</param>
        /// <param name="eventName">The name of the custom event to be triggered (note: the full event name will be "componentId.eventName"</param>
        /// <param name="configureData">The strings or lamdas specifing the data model values to pass in the event.</param>
        /// <param name="template">The HTML to display for this column.</param>
        public static GridTemplateColumnBuilder<T> CustomEvent<T>(this GridColumnFactory<T> columnFactory,
            string componentId, string eventName, Action<DataBuilder<T>> configureData, IHtmlContent template) where T : class
        {
            var writer = new System.IO.StringWriter();
            template.WriteTo(writer, HtmlEncoder.Default);
            string innerHtmlStr = writer.ToString();

            return CustomEvent(columnFactory, componentId, eventName, configureData, innerHtmlStr);
        }


        public class DataBuilder<TModel> where TModel : class
        {
            private Dictionary<string, string> _data;

            public IEnumerable<KeyValuePair<string, string>> Items
            {
                get { return _data.AsEnumerable<KeyValuePair<string, string>>(); }
            }

            public DataBuilder()
            {
                _data = new Dictionary<string, string>();
            }

            /// <summary>
            /// Adds or updates the specified event parameter to the specified value.
            /// note: The parameters will be passed in the "data" object of the event.
            /// </summary>
            /// <param name="eventParamName">The name of the parameter (ex: data.yourParamNameHere).</param>
            /// <param name="modelFieldName">The value of the parameter.</param>
            public DataBuilder<TModel> SetValue(string eventParamName, string fieldValue)
            {
                if (eventParamName.Contains('-'))
                    throw new Exception("Param names defined in KendoTemplateDataBuilder.CustomEvent can not contain \"-\" characters (eventParamName = \"" + eventParamName + "\").");

                if (_data.ContainsKey(eventParamName))
                {
                    _data[eventParamName] = fieldValue;
                }
                else
                {
                    _data.Add(eventParamName, fieldValue);
                }
                return this;
            }

            /// <summary>
            /// Maps the specified event parameter to the specified model field.
            /// note: The parameters will be passed in the "data" object of the event.
            /// </summary>
            /// <param name="eventParamName">The name of the parameter (ex: data.yourParamNameHere)</param>
            /// <param name="modelFieldName">The name of the model field to be mapped.</param>
            public DataBuilder<TModel> MapField(string eventParamName, string modelFieldName)
            {
                string fieldName = "#:" + modelFieldName + "#";
                SetValue(eventParamName, fieldName);
                return this;
            }

            /// <summary>
            /// Maps the specified event parameter to the specified model field.
            /// note: The parameters will be passed in the "data" object of the event.  Also, 
            /// the event and model field names will be the same.
            /// </summary>
            /// <param name="modelFieldName">The lamda specifing the model field to be mapped.</param>
            protected string GetPropertyName<TValue>(Expression<Func<TModel, TValue>> propertyLambda)
            {
                Type type = typeof(TModel);

                MemberExpression member = propertyLambda.Body as MemberExpression;
                if (member == null)
                    throw new ArgumentException(string.Format("Expression '{0}' refers to a method, not a property.", propertyLambda.ToString()));

                PropertyInfo propInfo = member.Member as PropertyInfo;
                if (propInfo == null)
                    throw new ArgumentException(string.Format("Expression '{0}' refers to a field, not a property.", propertyLambda.ToString()));

                //if (type != propInfo.ReflectedType && !type.IsSubclassOf(propInfo.ReflectedType))
                //    throw new ArgumentException(string.Format("Expresion '{0}' refers to a property that is not from type {1}.", propertyLambda.ToString(), type));

                return propInfo.Name;
            }

            /// <summary>
            /// Maps the specified event parameter to the specified model field.
            /// note: The parameters will be passed in the "data" object of the event.
            /// Use this function if you wish to have a different field name in your event data than that of the model field.
            /// </summary>
            /// <param name="eventParamName">The name of the parameter (ex: data.yourParamNameHere).</param>
            /// <param name="modelFieldName">The lamda specifing the model field to be mapped.</param>
            public DataBuilder<TModel> MapField<TValue>(string eventParamName, Expression<Func<TModel, TValue>> expression)
            {
                return MapField(eventParamName, GetPropertyName(expression));
            }

            public DataBuilder<TModel> MapField<TValue>(Expression<Func<TModel, TValue>> expression)
            {
                var propertyName = GetPropertyName(expression);
                return MapField(propertyName, propertyName);
            }

        }

    }
}

