using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Angelo.Plugins
{
    public class PluginConfig
    {
        public IConfigurationRoot ConfigurationRoot { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }
        public ILoggerFactory LoggerFactory { get; set; }

        public string AssemblyFolder { get; set; }
        public string ConnectionString { get; set; }
    }
}
