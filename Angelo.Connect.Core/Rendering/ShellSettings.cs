using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Rendering
{
    public class ShellSettings
    {
        public string WindowTitle { get; set; }

        public Dictionary<string, string> MetaTags { get; set; }

        public ToolbarSettings Toolbar { get; set; }

        public ShellSettings()
        {
            /* empty constructor */
        }

        public ShellSettings(string title)
        {
            WindowTitle = title;
        }

        public ShellSettings(string title, ToolbarSettings toolbar)
        {
            WindowTitle = title;
            Toolbar = toolbar;
        }
    }
}
