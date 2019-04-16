using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UserConsole
{
    public class UserConsoleTreeProvider : IUserConsoleComponentProvider
    {

        private IEnumerable<IUserConsoleTreeComponent> _treeComponents;
        private HttpContext _httpContext;

        public string ProviderType { get; } = "tree";

        public UserConsoleTreeProvider(IEnumerable<IUserConsoleTreeComponent> treeComponents)
        {
            _treeComponents = treeComponents;
        }


        public IEnumerable<IUserConsoleComponent> ListComponents()
        {
            return _treeComponents;
        }

        public async Task<ComponentViewResult> GetExplorerView(string treeType)
        {
            var component = GetTreeComponent(treeType);

            var result = new GenericViewResult
            {
                Title = component.TreeTitle,
                ViewPath = "~/UI/UserConsole/Views/TreeComponent.cshtml",
                ViewModel = new ConsoleTreeRoot
                {
                    TreeType = component.ComponentType,
                    RootNodes = await component.GetRootNodes(),
                    TreeMenu = await component.GetTreeMenu()
                },
            };

            return new ComponentViewResult(component, result);
        }
    
        public async Task<string> GetDefaultRoute(string treeType)
        {
            var component = GetTreeComponent(treeType);
            var nodes = await component.GetRootNodes();

            if (nodes != null && nodes.Count() > 0)
            {
                return nodes.First().LinkUrl;
            }

            throw new ArgumentNullException($"Could not determine default route for tree: {treeType}. Null or empty node collection.");
        }

        public async Task<IEnumerable<GenericTreeNode>> GetBranchNodes(string treeType, string nodeId, string nodeType = null)
        {
            var component = GetTreeComponent(treeType);

            return await component.GetChildNodes(nodeId, nodeType);
        }


        private IUserConsoleTreeComponent GetTreeComponent(string treeType)
        {
            var component = _treeComponents.FirstOrDefault(x => x.ComponentType == treeType);

            if (component == null)
                throw new NullReferenceException($"Could not locate tree component: {treeType}");

            return component;
        }
    }


}
