using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UserConsole
{
    public class UserConsoleCustomProvider : IUserConsoleComponentProvider
    {

        private IEnumerable<IUserConsoleCustomComponent> _customComponents;
        private IHttpContextAccessor _httpContextAccessor;

        public string ProviderType { get; } = "custom";

        public UserConsoleCustomProvider
        (
            IEnumerable<IUserConsoleCustomComponent> customComponents, 
            IHttpContextAccessor httpContextAccessor
        )
        {
            _customComponents = customComponents;
            _httpContextAccessor = httpContextAccessor;
        }

        public IEnumerable<IUserConsoleComponent> ListComponents()
        {
            return _customComponents;
        }

        public async Task<ComponentViewResult> GetExplorerView(string componentType)
        {
            var component = GetComponent(componentType);
            var result = await component.ComposeExplorer();

            return new ComponentViewResult(component, result);
        }

        public async Task<string> GetDefaultRoute(string componentType)
        {
            var component = GetComponent(componentType);

            return await Task.FromResult(component.InitialRoute);
        }

        public async Task<ComponentViewResult> GetDefaultContentView(string componentType)
        {
            var component = GetComponent(componentType);
            var content = await RenderRoute(component.InitialRoute);

            var result = new GenericViewResult
            {
                ViewPath = "/UI/UserConsole/Views/Splat.cshtml",
                ViewModel = content
            };
     
            return new ComponentViewResult(component, result);
        }

        private IUserConsoleCustomComponent GetComponent(string componentType)
        {
            var component = _customComponents.FirstOrDefault(x => x.ComponentType == componentType);

            if (component == null)
                throw new NullReferenceException($"Could not locate custom component: {componentType}");

            return component;
        }

        private async Task<string> RenderRoute(string url)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            if (httpContext == null)
                throw new NullReferenceException($"Null Request Context. Cannot Render {url}");

            var request = httpContext.Request;
            var httpClient = new HttpClient();
            var query = "";

            httpClient.DefaultRequestHeaders.Add("cookie", request.Headers["cookie"].AsEnumerable());
            httpClient.BaseAddress = new Uri(request.Scheme + "://" + request.Host);


            var response = await httpClient.GetAsync(url + query);
            httpClient.Dispose();

            if (response.IsSuccessStatusCode)
                return await response.Content.ReadAsStringAsync();

            return String.Format(
                "ERROR {0}: {1} - {2}",
                response.StatusCode.ToString(),
                response.ReasonPhrase,
                url
            );
        }
    }
}
