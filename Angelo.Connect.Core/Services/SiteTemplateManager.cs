using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

using Angelo.Connect.Configuration;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Services
{
    public class SiteTemplateManager
    {
        private string _rootFolder;
        private List<SiteTemplate> _siteTemplates;
        private ConnectCoreOptions _options;

        public SiteTemplateManager(ConnectCoreOptions options)
        {
            
            _siteTemplates = new List<SiteTemplate>();
            _options = options;

            _rootFolder = Path.Combine(_options.FileSystemRoot, _options.TemplatesPath.Replace('/', '\\'));

            LoadAllTemplates();
        }

        public void RefreshInternalCache()
        {
            LoadAllTemplates();
        }

        public List<SiteTemplate> GetAll()
        {
            return _siteTemplates;
        }

        public SiteTemplate GetTemplate (string templateId)
        {
            Ensure.NotNullOrEmpty(templateId, "templateId cannot be null or emtpy");

            return _siteTemplates.FirstOrDefault(x => x.Id == templateId); 
        }

        private void LoadAllTemplates()
        {           
            if (Directory.Exists(_rootFolder))
            {
                var templateDirectories = Directory.GetDirectories(_rootFolder).ToList();

                templateDirectories.ForEach(LoadConfigFile);
            }
        }

        private void LoadConfigFile(string templateFolder)
        {
            var targetConfigFile = Path.Combine(templateFolder, "config.json");
            if (File.Exists(targetConfigFile))
            {
                var siteTemplate = JsonConvert.DeserializeObject<SiteTemplate>(File.ReadAllText(targetConfigFile));

                foreach (var pageTemplate in siteTemplate.PageTemplates)
                {
                    if (pageTemplate.ViewPath == null)
                    {
                        pageTemplate.ViewPath = "~/" + _options.TemplatesPath + "/" + siteTemplate.Id + "/" + pageTemplate.Id;
                    }
                }

                // Set the theme's parent id
                siteTemplate.Themes.ToList().ForEach(x => x.SiteTemplateId = siteTemplate.Id);

                _siteTemplates.Add(siteTemplate);

            }
        }

    }
}
