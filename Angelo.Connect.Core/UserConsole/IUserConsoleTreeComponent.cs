using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UserConsole
{
    public interface IUserConsoleTreeComponent : IUserConsoleComponent
    {
        string TreeTitle { get; }

        Task<IEnumerable<GenericMenuItem>> GetTreeMenu();

        Task<IEnumerable<GenericTreeNode>> GetRootNodes();

        Task<IEnumerable<GenericTreeNode>> GetChildNodes(string nodeId, string nodeType);
    }
}
