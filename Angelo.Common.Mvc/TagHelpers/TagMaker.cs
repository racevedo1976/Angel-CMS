using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Angelo.Common.Mvc.TagHelpers
{
    public class TagMaker : TagBuilder
    {
        public List<TagMaker> Children { get; set; } = new List<TagMaker>();
        public string Text { get; set; }

        public TagMaker(string tagName) : base(tagName)
        {
        }

        public TagMaker(string tagName, string css) : base(tagName)
        {
            AddCssClass(css);
        }

        public new void AddCssClass(string className)
        {
            AddCssClasses(className.Split(' '));
        }

        public void AddCssClasses(params string[] classNames)
        {
            var css = new List<string>();

            if (Attributes.ContainsKey("class")) {
                css.AddRange(Attributes["class"].Split(' '));
                Attributes.Remove("class");
            }

            css.AddRange(classNames);;

            Attributes.Add("class", String.Join(" ", classNames.Distinct()));
        }

        public HtmlString ToHtmlString()
        {
            return new HtmlString(this.ToString());
        }

        public override string ToString()
        {
            var sb = new StringBuilder("<" + TagName + AttributeString());

            if (IsSelfClosing())
            {
                sb.Append("/>");
            }
            else
            {
                sb.Append($">" + (Text ?? ""));
                foreach (var child in Children)
                {
                    sb.Append("\n" + child.ToString() + "\n");
                }
                sb.Append($"</{TagName}>");
            }

            return sb.ToString();
        }


        private string AttributeString()
        {
            var sb = new StringBuilder();

            foreach (var prop in Attributes)
            {
                sb.Append(" " + prop.Key + "=" + '"' + prop.Value.Replace("\"", "\\\"") + '"');
            }

            if (Attributes.Count == 0)
                sb.Append(" ");

            return sb.ToString();
        }

        private bool IsSelfClosing()
        {
            // Ref: http://www.w3.org/tr/html5/syntax.html
            // Section 8.1.2 (Void Elements)
            
            var targets = new string[] {
                "area", "base", "br", "col",  "embed", "hr", "img", "input", "keygen", "link",
                "meta", "param", "source", "track", "wbr"
            };

            return targets.Contains(TagName.ToLower());
        }

    }
}
