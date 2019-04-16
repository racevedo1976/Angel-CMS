using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.UserConsole
{
    public class UserConsoleComponentFactory
    {
        private IEnumerable<IUserConsoleComponentProvider> _componentProviders;
        private IHttpContextAccessor _httpContextAccessor;

        public UserConsoleComponentFactory(IEnumerable<IUserConsoleComponentProvider> componentProviders, IHttpContextAccessor httpContextAccessor)
        {
            _componentProviders = componentProviders;
            _httpContextAccessor = httpContextAccessor;
        }

        public TComponentProvider GetProvider<TComponentProvider>() where TComponentProvider : class, IUserConsoleComponentProvider
        {
            var provider = _componentProviders.FirstOrDefault(x => x.GetType() == typeof(TComponentProvider));

            if (provider == null)
                throw new NullReferenceException($"Invalid or missing provider: {nameof(TComponentProvider)}");

            return provider as TComponentProvider;
        }

        public IUserConsoleComponentProvider GetProvider(string componentType)
        {
            foreach(var provider in _componentProviders)
            {
                var components = provider.ListComponents();

                if (components.Any(x => x.ComponentType == componentType))
                {
                    return provider;
                }
            }

            throw new NullReferenceException($"Missing provider for component: {componentType}");
        }

        public IUserConsoleComponent GetComponent(string componentType)
        {
            foreach (var provider in _componentProviders)
            {
                var component = provider.ListComponents()?.FirstOrDefault(x => x.ComponentType == componentType);

                if (component != null)
                    return component;
            }

            throw new NullReferenceException($"Missing provider for component: {componentType}");
        }

        public async Task<IEnumerable<FactoryViewResult>> GetExplorerViews()
        {
            var results = new List<FactoryViewResult>();
            var components = new List<IUserConsoleComponent>();
            
            foreach (var provider in _componentProviders)
            {
                components.AddRange(provider.ListComponents());
            }

            foreach (var component in components.OrderBy(x => x.ComponentOrder))
            {
                var result = await GetExplorerView(component.ComponentType);

                results.Add(result);
            }

            return results;
        }

        public async Task<FactoryViewResult> GetExplorerView(string componentType)
        {
            var provider = GetProvider(componentType);
            var result = await provider.GetExplorerView(componentType);

            return new FactoryViewResult(provider, result);
        }

        public async Task<string> GetDefaultRoute(string componentType)
        {
            var provider = GetProvider(componentType);

            return await provider.GetDefaultRoute(componentType);
        }

        public FactoryViewResult ToFactoryResult(IUserConsoleComponent component, GenericViewResult genericResult)
        {
            var provider = GetProvider(component.ComponentType);
            var componentResult = new ComponentViewResult(component, genericResult);

            return new FactoryViewResult(provider, componentResult);
        }

        public async Task<string> RenderRoute(string url)
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
