using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UserConsole
{
    public class ComponentViewResult : GenericViewResult
    {
        public IUserConsoleComponent Component { get; set; }

        public ComponentViewResult() : base()
        {

        }

        public ComponentViewResult(IUserConsoleComponent component, GenericViewResult result) : base()
        {
            Component = component;

            Title = result.Title;
            ViewPath = result.ViewPath;
            ViewModel = result.ViewModel;
            ViewData = result.ViewData;
        }

    }
}
