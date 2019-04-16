using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.UserConsole
{
    public class FactoryViewResult : ComponentViewResult
    {
        public IUserConsoleComponentProvider Provider { get; set; }

        public FactoryViewResult(IUserConsoleComponentProvider provider, ComponentViewResult result)
        {
            Provider = provider;

            Component = result.Component;
            Title = result.Title;
            ViewPath = result.ViewPath;
            ViewModel = result.ViewModel;
            ViewData = result.ViewData;
        }

    }
}
