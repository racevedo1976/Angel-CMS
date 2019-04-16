using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Angelo.Connect.UserConsole
{
    public interface IUserConsoleComponentProvider
    {
        string ProviderType { get; }

        IEnumerable<IUserConsoleComponent> ListComponents();

        Task<ComponentViewResult> GetExplorerView(string componentType);

        Task<string> GetDefaultRoute(string componentType);
    }
}
