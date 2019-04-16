using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyModel;
using Microsoft.Extensions.Logging;

namespace Angelo.Plugins.Internal
{

    public class AssemblyProvider
    {
        private ILogger<AssemblyProvider> _logger;
        private string _pluginsFolder;

        /// <summary>
        /// Gets or sets the predicate that is used to filter discovered assemblies from a specific folder
        /// before thay have been added to the resulting assemblies set.
        /// </summary>
        public Func<Assembly, bool> IsCandidateAssembly { get; set; }

        /// <summary>
        /// Gets or sets the predicate that is used to filter discovered libraries from a web application dependencies
        /// before thay have been added to the resulting assemblies set.
        /// </summary>
        public Func<Library, bool> IsCandidateCompilationLibrary { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AssemblyProvider">AssemblyProvider</see> class.
        /// </summary>
        /// <param name="loggerFactory">A logger factory that is used to create a logger.</param>
        public AssemblyProvider(string pluginsFolder, ILoggerFactory loggerFactory)
        {
            _pluginsFolder = pluginsFolder;
            _logger = loggerFactory.CreateLogger<AssemblyProvider>() ;

            IsCandidateAssembly = assembly =>
              !assembly.FullName.StartsWith("Microsoft.") && !assembly.FullName.StartsWith("System.");

            IsCandidateCompilationLibrary = library =>
              library.Name != "NETStandard.Library" && !library.Name.StartsWith("Microsoft.") && !library.Name.StartsWith("System.");
        }

        /// <summary>
        /// Discovers and then gets the discovered assemblies from a specific folder and web application dependencies.
        /// </summary>
        /// <param name="path">The extensions path of a web application.</param>
        /// <returns></returns>
        public IEnumerable<Assembly> GetAssemblies()
        {
            List<Assembly> assemblies = new List<Assembly>();

            assemblies.AddRange(this.GetAssembliesFromPath(_pluginsFolder));
            assemblies.AddRange(this.GetAssembliesFromDependencyContext());
            return assemblies;
        }

        private IEnumerable<Assembly> GetAssembliesFromPath(string path)
        {
            List<Assembly> assemblies = new List<Assembly>();

            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                this._logger.LogInformation("Discovering and loading assemblies from path '{0}'", path);

                foreach (string extensionPath in Directory.EnumerateFiles(path, "*.dll"))
                {
                    Assembly assembly = null;

                    try
                    {
                        assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(extensionPath);

                        if (this.IsCandidateAssembly(assembly))
                        {
                            assemblies.Add(assembly);
                            this._logger.LogInformation("Assembly '{0}' is discovered and loaded", assembly.FullName);
                        }
                    }

                    catch (Exception e)
                    {
                        this._logger.LogWarning("Error loading assembly '{0}'", extensionPath);
                        this._logger.LogInformation(e.ToString());
                    }
                }
            }

            else
            {
                if (string.IsNullOrEmpty(path))
                    this._logger.LogWarning("Discovering and loading assemblies from path skipped: path not provided", path);

                else this._logger.LogWarning("Discovering and loading assemblies from path '{0}' skipped: path not found", path);
            }

            return assemblies;
        }

        private IEnumerable<Assembly> GetAssembliesFromDependencyContext()
        {
            List<Assembly> assemblies = new List<Assembly>();

            this._logger.LogInformation("Discovering and loading assemblies from DependencyContext");

            foreach (CompilationLibrary compilationLibrary in DependencyContext.Default.CompileLibraries)
            {
                if (this.IsCandidateCompilationLibrary(compilationLibrary))
                {
                    Assembly assembly = null;

                    try
                    {
                        assembly = Assembly.Load(new AssemblyName(compilationLibrary.Name));
                        assemblies.Add(assembly);
                        this._logger.LogInformation("Assembly '{0}' is discovered and loaded", assembly.FullName);
                    }

                    catch (Exception e)
                    {
                        this._logger.LogWarning("Error loading assembly '{0}'", compilationLibrary.Name);
                        this._logger.LogInformation(e.ToString());
                    }
                }
            }

            return assemblies;
        }
    }
}