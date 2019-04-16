using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Threading.Tasks;

namespace Angelo.Common.Mvc.TagHelpers
{
    public class ModalContext
    {
        public IHtmlContent Body { get; set; }
        public IHtmlContent Footer { get; set; }
    }

    /// <summary>
    /// A Bootstrap modal dialog
    /// </summary>
    [RestrictChildren("modal-body", "modal-footer")]
    public class ModalTagHelper : TagHelper
    {
        /// <summary>
        /// The title of the modal
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// The Id of the modal
        /// </summary>
        public string Id { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var modalContext = new ModalContext();
            var classNames = "modal fade";

            context.Items.Add(typeof(ModalTagHelper), modalContext);

            await output.GetChildContentAsync();

            output.TagName = "div";
            output.Attributes.SetAttribute("role", "dialog");
            output.Attributes.SetAttribute("id", Id);
            output.Attributes.SetAttribute("aria-labelledby", $"{context.UniqueId}Label");
            output.Attributes.SetAttribute("tabindex", "-1");

           
            if (output.Attributes.ContainsName("class"))
            {
                classNames = string.Format("{0} {1}", output.Attributes["class"].Value, classNames);
            }
            output.Attributes.SetAttribute("class", classNames);

            output.Content.AppendHtml("<div class='modal-dialog' role='document'>");
            output.Content.AppendHtml(" <div class='modal-content'>");
            output.Content.AppendHtml(" <div class='modal-header'>");
            output.Content.AppendHtml("     <button class='close' data-dismiss='modal'><span>&times;</span></button>");
            output.Content.AppendHtml($"    <h4 class='modal-title' id='{context.UniqueId}Label'>{Title}</h4>");
            output.Content.AppendHtml(" </div>");

            if (modalContext.Body != null)
            {
                output.Content.AppendHtml("<div class='modal-body'>");
                output.Content.AppendHtml(modalContext.Body);
                output.Content.AppendHtml("</div>");
            }

            if (modalContext.Footer != null)
            {
                output.Content.AppendHtml("<div class='modal-footer'>");
                output.Content.AppendHtml(modalContext.Footer);
                output.Content.AppendHtml("</div>");
            }
            
            output.Content.AppendHtml("</div></div>");
        }
    }
}
