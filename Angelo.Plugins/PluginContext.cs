using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using Microsoft.EntityFrameworkCore;


namespace Angelo.Plugins
{
    public class PluginContext
    {
        public PluginConfig Config { get; internal set; }

        public IEnumerable<Assembly> Assemblies { get; internal set; }

        public IEnumerable<IPlugin> Plugins { get; internal set; }

        internal ICollection<Action<IServiceCollection>> ServiceConfigurations { get; private set; }
        internal ICollection<Action<IRouteBuilder>> RouteConfigurations { get; private set; }
        internal ICollection<Action<IApplicationBuilder>> AppConfigurations { get; private set; }
        internal IDictionary<string, IList<IPluginData>> AppData { get; private set; }
        
        public PluginContext(PluginConfig config)
        {
            Config = config;

            ServiceConfigurations = new List<Action<IServiceCollection>>();
            RouteConfigurations = new List<Action<IRouteBuilder>>();
            AppConfigurations = new List<Action<IApplicationBuilder>>();
            AppData = new Dictionary<string, IList<IPluginData>>();
        }

        public void AddData<TDataItem>(TDataItem item)
        where TDataItem : IPluginData
        {
            var dataStore = GetOrCreateDataStore(typeof(TDataItem));

            dataStore.Add(item);
        }

        public IList<TDataItem> GetData<TDataItem>()
        where TDataItem : IPluginData
        {
            var dataStore = GetOrCreateDataStore(typeof(TDataItem));

            return dataStore.Select(x => (TDataItem)x).ToList();
        }

        public TDataItem GetData<TDataItem>(string id)
        where TDataItem : IPluginData
        {
            var dataStore = GetOrCreateDataStore(typeof(TDataItem));
            
            if (dataStore.Any(x => x.Id == id))
                return (TDataItem)dataStore.First(x => x.Id == id);

            return default(TDataItem);
        }

        private IList<IPluginData> GetOrCreateDataStore(Type dataType)
        {
            var dataKey = dataType.GetType().FullName;

            if (!AppData.ContainsKey(dataKey))
            {
                AppData[dataKey] = new List<IPluginData>();
            }

            return AppData[dataKey];
        }
       
    }
}
