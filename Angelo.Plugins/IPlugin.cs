using System;
using System.Collections.Generic;

namespace Angelo.Plugins
{
    /// <summary>
    /// Provides an interface for registering plugins with the <see cref="PluginProvider">PluginProvider</see>
    /// </summary>
    public interface IPlugin
  {
        /// <summary>
        /// The common name of the plugin
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Description for users that describes what the plugin does
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Plugin version
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Plugin author
        /// </summary>
        string Author { get; }


        /// <summary>
        /// Method to register the plugin's configuration and services
        /// </summary>
        void Startup(PluginBuilder startup);
    }
}