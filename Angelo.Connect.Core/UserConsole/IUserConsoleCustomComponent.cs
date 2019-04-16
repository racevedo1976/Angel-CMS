using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UserConsole
{
    public interface IUserConsoleCustomComponent : IUserConsoleComponent
    {
        Task<GenericViewResult> ComposeExplorer();

        string InitialRoute { get; }
    }
}
