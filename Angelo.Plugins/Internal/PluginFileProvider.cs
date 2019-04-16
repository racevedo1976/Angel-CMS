using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;


namespace Angelo.Plugins.Internal
{
    public class PluginFileProvider : IFileProvider
    {
        private PluginContext _pluginContext;
        private static Dictionary<string, Assembly> _resourceMap;

        public PluginFileProvider(PluginContext pluginContext)
        {
            _pluginContext = pluginContext;


            // creating static resource map only once
            if (_resourceMap == null)
            {
                _resourceMap = new Dictionary<string, Assembly>();

                foreach (var assembly in pluginContext.Assemblies)
                {
                    foreach (var resourceName in assembly.GetManifestResourceNames())
                    {
                        _resourceMap.Add(resourceName, assembly);
                    }
                }
            }
        }

        public IDirectoryContents GetDirectoryContents(string path)
        {
            // Temporarily supporting standard & old path conventions
            // TODO: Depricate & remove old path convention
            var contents = GetDirectoryContentsNew(path);

            if (contents.Count() >= 0)
                return contents;

            //else
            return GetDirectoryContentsOld(path);
        }

        public IFileInfo GetFileInfo(string path)
        {
            // Temporarily supporting standard & old path conventions
            // TODO: Depricate & remove old path convention
            var file = GetFileInfoNew(path);

            if (file.Exists)
                return file;

            return GetFileInfoOld(path);
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        #region Get files under wwwroot folder using standard path
        private IDirectoryContents GetDirectoryContentsNew(string path)
        {         
            // Disallowing browsing of embedded content by folder
            return NotFoundDirectoryContents.Singleton;
        }

        private IFileInfo GetFileInfoNew(string path)
        {
            // Safety check to make sure we don't try to handle MVC routes
            if (HasFileExt(path))
            {
                if (!path.StartsWith("/"))
                    path = "/" + path;

                var resouceName = path.ToLower().Replace("/", ".");

                foreach (var entry in _resourceMap)
                {
                    if (entry.Key.ToLower().EndsWith(resouceName))
                    {
                        return new PluginFileInfo(entry.Value, "wwwroot" + path);
                    }
                }

            }
            return new NotFoundFileInfo(path);
        }
        #endregion

        #region Get files under assets folder using old path convention 
        private IDirectoryContents GetDirectoryContentsOld(string subpath)
        {
            var pattern = new Regex(@"[/]?assets[/]{1}([^/]+)([^\?]*)");
            var match = pattern.Match(subpath.ToLower());

            if (match.Success)
            {
                var pluginName = match.Groups[1].Value;
                var folderPath = "assets" + match.Groups[2].Value;

                var assembly = GetAssemblyByPluginName(pluginName);

                if (assembly != null)
                {
                    return new PluginDirectoryContents(assembly, folderPath);
                }
            }

            return NotFoundDirectoryContents.Singleton;
        }

        private IFileInfo GetFileInfoOld(string path)
        {
            var pattern = new Regex(@"[/]?assets[/]{1}([^/]+)([^\?]+)");
            var match = pattern.Match(path.ToLower());

            if (match.Success)
            {
                var pluginName = match.Groups[1].Value;
                var filePath = "assets" + match.Groups[2].Value;
                var assembly = GetAssemblyByPluginName(pluginName);

                if (assembly != null)
                {
                    return new PluginFileInfo(assembly, filePath);
                }
            }

            return new NotFoundFileInfo(path);
        }
        #endregion

        private Assembly GetAssemblyByPluginName(string shortName)
        {
            shortName = shortName.ToLower();

            return _pluginContext.Assemblies.FirstOrDefault(a =>
                a.GetName().Name.ToLower().EndsWith($".{shortName}")
            );
        }

        private bool HasFileExt(string path)
        {
            var ext = System.IO.Path.GetExtension(path);

            return !String.IsNullOrWhiteSpace(ext);
        }

    }
}
