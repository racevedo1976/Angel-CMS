using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

using System.Collections;

namespace Angelo.Plugins.Internal
{
    public class PluginDirectoryContents : IDirectoryContents
    {
        public List<PluginFileInfo> Entries { get; private set; }
        public bool Exists { get; private set; } = false;

        public PluginDirectoryContents(Assembly assembly, string subpath)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            var files = new List<PluginFileInfo>();
            var resourcePath = GetResourcePath(assembly, subpath);

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.ToLower().StartsWith(resourcePath))
                {
                    files.Add(new PluginFileInfo(assembly, resourceName));
                }
            }

            if (files.Count > 0)
            {
                Exists = true;
                Entries = files;
            }
        }

        public IEnumerator<IFileInfo> GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Entries.GetEnumerator();
        }

        private string GetResourcePath(Assembly assembly, string subpath)
        {
            var assemblyName = assembly.GetName().Name.ToLower();

            subpath = subpath.Replace('/', '.').ToLower();

            if (subpath.StartsWith("."))
                subpath = subpath.Substring(1);

            if (!subpath.StartsWith(assemblyName))
                subpath = assemblyName + "." + subpath;

            return subpath;
        }
    }

}
