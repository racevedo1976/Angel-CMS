using System;
using System.Text;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

using Angelo.Common.Mvc;

namespace Angelo.Common.Mvc.TagHelpers
{
    //[HtmlTargetElement("pager", TagStructure = TagStructure.WithoutEndTag)]
    [HtmlTargetElement("pager")]
    public class PagerTagHelper : TagHelper
    {
        [HtmlAttributeName("page-url")]
        public string Url { get; set; }

        [HtmlAttributeName("page-count")]
        public int PageCount { get; set; }

        [HtmlAttributeName("page-current")]
        public int PageCurrent { get; set; }

        [HtmlAttributeName("page-limit")]
        public int PageLimit { get; set; } = 10;      

        [HtmlAttributeName("ajax-method")]
        public AjaxMethod AjaxMethod { get; set; } = AjaxMethod.Get;

        [HtmlAttributeName("ajax-mode")]
        public AjaxMode AjaxMode { get; set; } = AjaxMode.Replace;

        [HtmlAttributeName("ajax-target")]
        public string AjaxTarget { get; set; }

        [ViewContextAttribute] 
        public ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            PageLimit = Math.Min(PageLimit, PageCount);

            var start = CalculateStart();
            var stop = Math.Min(PageCount, start + PageLimit - 1);
            
            var pages = BuildPageList(start, stop);
            var arrows = BuildArrowList(PageCurrent);

            var spacer = new HtmlString("<span>&nbsp;&nbsp;</span>");
            var label = new HtmlString($"<span><small>{PageCount} Pages</small></span>");

            output.TagName = "nav";
            output.Content.AppendHtml(pages);
            output.Content.AppendHtml(spacer);
            output.Content.AppendHtml(arrows);
            //output.Content.AppendHtml(spacer);
            //output.Content.AppendHtml(label);
        }


        private HtmlString BuildPageList(int start, int stop)
        {
            var ul = new TagMaker("ul", "pagination");

            for (var i = start; i <= stop; i++)
            {
                var li = new TagMaker("li", "page page-number");
                var a = new TagMaker("a");

                a.MergeAttribute("href", $"{Url}?page={i}");
                a.MergeAttribute("title", $"Click to go to page {i}");
                a.Text = i.ToString();

                ExtendForAjax(a);

                if (i == PageCurrent)
                    li.AddCssClass("active");
                
                li.Children.Add(a);
                ul.Children.Add(li);
            }

            return new HtmlString(ul.ToString());
        }

        private HtmlString BuildArrowList(int current)
        {
            var ul = new TagMaker("ul", "pagination");
            var left = new TagMaker("li", "page page-arrow");
            var right = new TagMaker("li", "page page-arrow");
            var leftArrow = new TagMaker("a") { Text = "&laquo;" };
            var rightArrow = new TagMaker("a") { Text = "&raquo;" };

            if (current > 1)
            {
                leftArrow.MergeAttribute("href", $"{Url}?page={current - 1}");
                leftArrow.MergeAttribute("title", "Go to the previous page");
            }
            else
            {
                left.AddCssClasses("disabled");
                leftArrow.Attributes.Add("disabled", "disabled");
            }

            if (current < PageCount)
            {
                rightArrow.Attributes.Add("href", $"{Url}?page={current + 1}");
                rightArrow.MergeAttribute("title", "Go to the next page");
            }
            else
            {
                right.AddCssClasses("disabled");
                rightArrow.Attributes.Add("disabled", "disabled");
            }

            ExtendForAjax(leftArrow);
            ExtendForAjax(rightArrow);

            left.Children.Add(leftArrow);
            right.Children.Add(rightArrow);
            ul.Children.Add(left);
            ul.Children.Add(right);

            return new HtmlString(ul.ToString());
        }

        private void ExtendForAjax(TagMaker tag)
        {
            if (!String.IsNullOrEmpty(AjaxTarget))
            {
                tag.MergeAttribute("data-ajax", "true");
                tag.MergeAttribute("data-ajax-update", AjaxTarget);
                tag.MergeAttribute("data-ajax-method", AjaxMethod.ToStringValue());
                tag.MergeAttribute("data-ajax-mode", AjaxMode.ToStringValue());
            }
        }

        private int CalculateStart()
        {
            var offsetRight = (int)Math.Floor((double)PageLimit / 2);
            var offsetLeft = (PageLimit % 2 == 1) ? offsetRight : offsetRight - 1;

            if (PageCount <= PageLimit || PageCurrent <= offsetLeft)
                return 1;

            else if (PageCurrent + offsetRight >= PageCount)
                return PageCount - PageLimit + 1;

            return PageCurrent - offsetLeft;
        }
       
    }
}
