using Angelo.Connect.Abstractions;
using Angelo.Connect.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Angelo.Connect.Web.UI.ViewModels.Extensions;

namespace Angelo.Connect.Web.UI.Components
{

    // CONTENT SELECTOR USAGE:
    //
    // To user the content selector, add this component somewhere in you web page:
    //
    // ex:
    // <component name="yourComponent" type="ConentSelector"></component>
    //
    // Javascript Usage:
    // To call the content selector, execute the ShowContentSelector(options) function:
    //
    // options = {
    //    idTarget: The id of the element to store the ContentId of the selected conent item.
    //    nameTarget: The id of the element to store the Name of the selected content item.
    //    fileUrlTarget: The id of the element to store the file URL of the selected content item (this element can be either an input or image element).
    //    thumbUrlTarget: The id of the element to store the thumbnail URL of the selected content item (this element can be either an input or image element).
    //
    //    onSelect: The function or function definition to call when an item is selected.
    //          
    //                       onSelect: function (items) { ... do something ... }
    //                       Where items is an array of selected item objects:
    //                          items[0].contentId
    //                          items[0].name
    //                          items[0].contentType
    //                          items[0].fileUrl
    //                          items[0].thumbUrl
    // 
    //    onCancel: The function or function definition to call when the content selector is canceled.
    //
    //
    // example:
    //    ShowContentSelector({
    //          fileUrlTarget: "IdOfFileInputElement",
    //          onSelect: function(items) { alert("Name of selected item = " + items[0].name); }
    //    });
    //

    public class ContentSelector : ViewComponent
    {
        private IEnumerable<IContentBrowser> _browserTypes;

        public ContentSelector(IEnumerable<IContentBrowser> browserTypes)
        {
            _browserTypes = browserTypes;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = HttpContext.User.GetUserId();

            ViewData["userId"] = userId;
            
            return View("ContentSelector");
        }
      
    }
}
