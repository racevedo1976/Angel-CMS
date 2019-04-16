using System;
using System.IO;
using System.Linq;
using System.Reflection;

using Microsoft.Extensions.FileProviders;

namespace Angelo.Plugins
{  
    public class PluginFileInfo : IFileInfo
    {

        private ManifestResourceInfo _resourceInfo;
        private string _resourceName;
        private Assembly _assembly;

        public bool Exists { get; private set; }
        public string Name { get; private set; }
        public string PhysicalPath { get; private set; }
        public long Length { get; private set; }
        public DateTimeOffset LastModified { get; private set; }
        public bool IsDirectory { get; private set; } = false;

        public PluginFileInfo(Assembly assembly, string fileName)
        {
            var resourceName = GetResourceName(assembly, fileName);

            _assembly = assembly;
            _resourceName = resourceName;
            _resourceInfo = assembly.GetManifestResourceInfo(resourceName);

            if (_resourceInfo != null)
            {
                Exists = true;
                PhysicalPath = resourceName;
                Name = GetFileName(resourceName);
                Length = GetLength(resourceName);
                LastModified = File.GetCreationTime(_assembly.Location);
            }
        }

        public Stream CreateReadStream()
        {
            if (Exists)
            {
                return _assembly.GetManifestResourceStream(_resourceName);
            }

            return null;
        }

        private string GetFileName(string resourceName)
        {
            var nameParts = resourceName.Split('.');
            var length = nameParts.Length;

            return nameParts[length - 2] + "." + nameParts[length - 1];
        }

        private long GetLength(string resourceName)
        {
            var stream = _assembly.GetManifestResourceStream(resourceName);
            var length = stream.Length;

            stream.Dispose();

            return length;
        }

        private string GetResourceName(Assembly assembly, string fileName)
        {
            var assemblyName = assembly.GetName().Name.ToLower();

            fileName = fileName.Replace('/', '.').ToLower();

            if (fileName.StartsWith("."))
                fileName = fileName.Substring(1);

            if (!fileName.StartsWith(assemblyName))
                fileName = assemblyName + "." + fileName;

            foreach (var resourceName in assembly.GetManifestResourceNames())
            {
                if (resourceName.ToLower() == fileName)
                {
                    fileName = resourceName;
                    break;
                }
            }

            return fileName;
        }
    }
}
