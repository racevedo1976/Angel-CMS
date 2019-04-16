using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Angelo.Common.Mvc.TagHelpers
{
    [HtmlTargetElement("input")]
    public class AutocompleteTagHelper : TagHelper
    {

        [HtmlAttributeName("autocomplete-url")]
        public string SourceUrl { get; set; }

        [HtmlAttributeName("autocomplete-action")]
        public string Action { get; set; }

        [HtmlAttributeName("autocomplete-lookuplimit")]
        public int ListLookupLimit { get; set; } = 10;

        [HtmlAttributeName("autocomplete-minchars")]
        public int MinChars { get; set; } = 3;

        [HtmlAttributeName("autocomplete-maxheight")]
        public int MaxHeight { get; set; } = 300;

        [HtmlAttributeName("autocomplete-width")]
        public string Width { get; set; } = "auto";

        [HtmlAttributeName("autocomplete-defer-request-by")]
        public int DeferRequestBy { get; set; } = 500; //milliseconds

        [HtmlAttributeName("autocomplete-show-no-suggestion-notice")]
        public bool ShowNoSuggestionNotice { get; set; } = true;

        [HtmlAttributeName("autocomplete-no-suggestion-notice")]
        public string NoSuggestionNotice { get; set; } = "No results";

        [HtmlAttributeName("autocomplete-group-by")]
        public string GroupBy { get; set; } = "";

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            if (!string.IsNullOrEmpty(SourceUrl))
            {
                output.Attributes.Add("data-autocomplete-url", SourceUrl);
                output.Attributes.Add("data-autocomplete-action", Action);
                output.Attributes.Add("data-autocomplete-lookuplimit", ListLookupLimit);
                output.Attributes.Add("data-autocomplete-minchars", MinChars);
                output.Attributes.Add("data-autocomplete-maxheight", MaxHeight);
                output.Attributes.Add("data-autocomplete-width", Width);
                output.Attributes.Add("data-autocomplete-defer-request-by", DeferRequestBy);
                output.Attributes.Add("data-autocomplete-show-no-suggestion-notice", ShowNoSuggestionNotice);
                output.Attributes.Add("data-autocomplete-no-suggestion-notice", NoSuggestionNotice);
                output.Attributes.Add("data-autocomplete-group-by", GroupBy);
            }
            await Task.FromResult(0);
        }
    }
}