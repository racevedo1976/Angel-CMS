using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


using Angelo.Connect.Data;
using Angelo.Connect.CoreWidgets.Models;
using Angelo.Connect.Widgets.Services;

namespace Angelo.Connect.CoreWidgets.Services
{
    public class RawHtmlService : JsonWidgetService<RawHtml>
    {
        public RawHtmlService(ConnectDbContext db) : base(db)
        {
        }

        public override RawHtml GetDefaultModel()
        {
            // returning same options regardless of view (for now)
            return new RawHtml
            {
                Html = ""

                //Html = "<h3>Sample Content</h3>"
                //     + "<hr>"
                //     + "<p>Lorem ipsum dolor sit amet, ne malis aliquid praesent sea, eu mel oblique facilisis deseruisse.</p>"
                //     + "<p>Ne mei iriure definiebas signiferumque, tollit graeci facilis his ei. Quo ex enim praesent omittantur.</p>"
                //     + "<p>Quo cu erat rebum. Nec errem abhorreant voluptatibus et, inimicus definitionem ne sed.</p>"
                //     + "<hr>"
                //     + "<ul><li><a href=\"#link1\">Additional Link 1</a></li><li><a href=\"#link2\">Additional Link 2</a></li></ul>"
            };
        }
    }
}
