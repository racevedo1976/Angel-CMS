using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


using Angelo.Common.Extensions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Data;
using Angelo.Connect.Logging;
using Angelo.Connect.Models;
using Angelo.Connect.Rendering;
using Angelo.Connect.Widgets;

namespace Angelo.Connect.Services
{
    public class SiteTemplateExporter
    {
        public const string CURRENT_SCHEMA = "1.0";

        private ConnectDbContext _connectDb;
        private ConnectCoreOptions _coreOptions;
        private WidgetProvider _widgetProvider;
        private SiteTemplateManager _templateManager;
        private ContentManager _contentManager;
        private SiteManager _siteManager;
        private PageManager _pageManager;
        private PageMasterManager _masterPageManager;
        private SiteTemplate _baseTemplate;
        private SiteTemplate _exportTemplate;
        private DbLogService _logger;
        private JsonSerializerSettings _jsonSettings;

        private string _baseTemplateFolder;
        private string _baseDefaultThemeFolder;
        private string _exportRootFolder;
        private string _exportTemplateFolder;
        private string _exportDefaultThemeFolder;
        private string _exportDataFolder;

        private Site _site;

        private Dictionary<string, SiteTemplateMasterPage> _masterPageMap;
        private Dictionary<string, SiteTemplatePage> _pageMap;

        public SiteTemplateExporter
        (
            ConnectDbContext connectDb,
            ConnectCoreOptions coreOptions,
            ContentManager contentManager,
            WidgetProvider widgetProvider,
            SiteManager siteManager,
            SiteTemplateManager templateManager,
            PageMasterManager masterPageManager,
            PageManager pageManger,
            DbLogService logger
        )
        {
            _connectDb = connectDb;
            _coreOptions = coreOptions;
            _contentManager = contentManager;
            _siteManager = siteManager;
            _widgetProvider = widgetProvider;
            _templateManager = templateManager;
            _masterPageManager = masterPageManager;
            _pageManager = pageManger;
            _logger = logger;

            _exportRootFolder = Path.Combine(_coreOptions.FileSystemRoot, _coreOptions.TemplateExportPath.Replace("/", @"\"));

            _masterPageMap = new Dictionary<string, SiteTemplateMasterPage>();
            _pageMap = new Dictionary<string, SiteTemplatePage>();

            _jsonSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
            };
        }

        public async Task ExportSiteAsTemplate(string siteId, string templateId, string templateTitle, string processId)
        {
            if (siteId == null)
                throw new ArgumentNullException(nameof(siteId));

            if (string.IsNullOrWhiteSpace(templateId))
                throw new ArgumentNullException(nameof(templateId));

            if (string.IsNullOrWhiteSpace(templateTitle))
                throw new ArgumentNullException(nameof(templateTitle));

            templateId = templateId.Replace(" ", String.Empty).ToLower();

            // Get the site and site template for common access
            _site = await _siteManager.GetByIdAsync(siteId);
            _baseTemplate = _templateManager.GetTemplate(_site.SiteTemplateId);

            // Set IO paths for common access
            _exportTemplateFolder = Path.Combine(_exportRootFolder, templateId);
            _exportDataFolder = Path.Combine(_exportTemplateFolder, SiteTemplate.DATA_FOLDER_NAME);
            _exportDefaultThemeFolder = Path.Combine(_exportTemplateFolder, SiteTemplate.THEMES_FOLDER_NAME, SiteTemplate.THEMES_DEFAULT_NAME);
            _baseTemplateFolder = Path.Combine(_coreOptions.FileSystemRoot, _baseTemplate.TemplateFolder.Replace("/", @"\"));
            _baseDefaultThemeFolder = Path.Combine(_baseTemplateFolder, SiteTemplate.THEMES_FOLDER_NAME, SiteTemplate.THEMES_DEFAULT_NAME);

            _logger.SetCategory("Jobs.ExportTemplate", processId ?? Guid.NewGuid().ToString());
            _logger.Log($"Exporting site {_site.Title} as Template {templateTitle}, version {CURRENT_SCHEMA}");

            // Initialize a new template to export
            _exportTemplate = new SiteTemplate()
            {
                Schema = CURRENT_SCHEMA,
                Id = templateId,
                Title = templateTitle,
                PageTemplates = new List<PageTemplate>(),
                Pages = new List<SiteTemplatePage>(),
                MasterPages = new List<SiteTemplateMasterPage>(),
                NavMenus = new List<SiteTemplateNav>(),
                Themes = new List<SiteTemplateTheme>(),

                // scripts can be set directly from base w/o modification
                Scripts = _baseTemplate.Scripts
            };

            // Perform export steps
            ScaffoldExportFolder();
            ExportDefaultTheme();
            ExportPageTemplates();

            await ExportSiteCssAsTemplateCss();
            await ExportMasterPages();
            await ExportPages();

            WriteTemplateConfigFile();

            _logger.Log("Export complete");

            // Refresh template manager cache to pickup the exported template
            _templateManager.RefreshInternalCache();
        }

        private void ScaffoldExportFolder()
        {
            // Folder must be empty to prevent accidental overwrite of an existing template
            if (Directory.Exists(_exportTemplateFolder))
            {
                if (Directory.GetFileSystemEntries(_exportTemplateFolder).Count() != 0)
                    throw new Exception("Template export folder must be empty");
            }

            // Create export directories
            Directory.CreateDirectory(_exportDataFolder);
            Directory.CreateDirectory(_exportDefaultThemeFolder);



            _logger.LogTrace($"Created folder: {Path.Combine(_coreOptions.TemplateExportPath, _exportTemplate.Id)}");
        }

        private void WriteTemplateConfigFile()
        {            
            // using an anymous object to prevent export readonly properties
            var templateConfig = new
            {
                Schema = _exportTemplate.Schema,
                Id = _exportTemplate.Id,
                Title = _exportTemplate.Title,
                Description = _exportTemplate.Description,
                PageTemplates = _exportTemplate.PageTemplates,
                Themes = _exportTemplate.Themes,
                Scripts = _exportTemplate.Scripts,
                MasterPages = _exportTemplate.MasterPages,
                Pages = _exportTemplate.Pages,
            };

            // serialize and export
            var exportPath = Path.Combine(_exportTemplateFolder, SiteTemplate.CONFIG_FILE_NAME);
            var contents = JsonConvert.SerializeObject(templateConfig, _jsonSettings);

            File.WriteAllText(exportPath, contents);

            _logger.LogTrace($"Exported template configuration to {Path.GetFileName(exportPath)}");
        }

        private async Task ExportSiteCssAsTemplateCss()
        {
            var exportFilePath = Path.Combine(_exportTemplateFolder, SiteTemplate.TEMPLATE_CSS_FILE_NAME);
            var baseFilePath = Path.Combine(_baseTemplateFolder, SiteTemplate.TEMPLATE_CSS_FILE_NAME);

            var siteCss = await _siteManager.GetSiteSettingAsync(_site.Id, SiteSettingKeys.SITE_CSS);
            var baseCss = File.ReadAllText(baseFilePath);

            var exportCss = new StringBuilder();


            if (IsNotEmptyCss(baseCss))
            {
                exportCss.AppendLine($"/*-------- Begin Exported Template Css, Template: {_baseTemplate.Title} --------*/");
                exportCss.AppendLine();
                exportCss.Append(baseCss);
                exportCss.AppendLine();
            }
          
            exportCss.AppendLine($"/*-------- Begin Exported Site Css, Site: {_site.Title} --------*/");
            exportCss.AppendLine();
            exportCss.Append(siteCss?.Value ?? "/* empty */");
            exportCss.AppendLine();

            File.WriteAllText(exportFilePath, exportCss.ToString());

            _logger.LogTrace($"Created template css file");

            // Copy font assets if present
            var baseFontsFolderPath = Path.Combine(_baseTemplateFolder, "fonts");
            if (Directory.Exists(baseFontsFolderPath))
            {
                CopyFolder(baseFontsFolderPath, Path.Combine(_exportTemplateFolder, "fonts"), true);
                _logger.LogTrace($"Copied template font assets");
            }

            // Copy image assets if present
            var baseImagesFolderPath = Path.Combine(_baseTemplateFolder, "images");
            if (Directory.Exists(baseImagesFolderPath))
            {
                CopyFolder(baseImagesFolderPath, Path.Combine(_exportTemplateFolder, "images"), true);
                _logger.LogTrace($"Copied template image assets");
            }
        }

        private void ExportDefaultTheme()
        {
            // Get the base template default theme settings
            var baseTheme = _baseTemplate.Themes.First(x => x.IsDefault);
            
        
            // Create entry for default theme in settings
            var defaultTheme = new SiteTemplateTheme
            {
                Id = SiteTemplate.THEMES_DEFAULT_NAME,
                Title = "Default Theme",
                Description = "The default theme for this template",
                CoreStyles = baseTheme.CoreStyles
            };

            _exportTemplate.Themes.Add(defaultTheme);


            // Export the base theme as template theme
            var baseFilePath = Path.Combine(_baseDefaultThemeFolder, SiteTemplate.THEME_CSS_FILE_NAME);
            var exportFilePath = Path.Combine(_exportDefaultThemeFolder, SiteTemplate.THEME_CSS_FILE_NAME);

            var exportCss = new StringBuilder();
            var baseCss = "/* empty */";

            if (File.Exists(baseFilePath))
            {
                baseCss = File.ReadAllText(baseFilePath);
            }

            exportCss.AppendLine($"/*-------- Begin Exported Theme Css, Theme: {_baseTemplate.Title} ({baseTheme.Title}) --------*/");
            exportCss.AppendLine();
            exportCss.AppendLine(baseCss);


            File.WriteAllText(exportFilePath, exportCss.ToString());

            _logger.LogTrace("Created default theme css file");


            // Copy font assets if present
            var baseFontsFolderPath = Path.Combine(_baseDefaultThemeFolder, "fonts");
            if (Directory.Exists(baseFontsFolderPath))
            {
                CopyFolder(baseFontsFolderPath, Path.Combine(_exportDefaultThemeFolder, "fonts"), true);
                _logger.LogTrace($"Copied theme font assets");
            }

            // Copy image assets if present
            var baseImagesFolderPath = Path.Combine(_baseDefaultThemeFolder, "images");
            if (Directory.Exists(baseImagesFolderPath))
            {
                CopyFolder(baseImagesFolderPath, Path.Combine(_exportDefaultThemeFolder, "images"), true);
                _logger.LogTrace($"Copied theme image assets");
            }

        }

        private void ExportPageTemplates()
        {
            foreach(var basePageTemplate in _baseTemplate.PageTemplates)
            {
                // Copy settings 

                // NOTE: SeedData is deprecated on 1.1 and above schemas
                // NOTE: PreviewImage is by convention.. no need to set

                var exportPageTemplate = new PageTemplate
                {
                    Id = basePageTemplate.Id,
                    Title = basePageTemplate.Title
                };

                _exportTemplate.PageTemplates.Add(exportPageTemplate);

                // Copy view .cshtml file
                var sourcePath = Path.Combine(_baseTemplateFolder, basePageTemplate.Id);
                var exportPath = Path.Combine(_exportTemplateFolder, exportPageTemplate.Id);

                File.Copy(sourcePath, exportPath);

                _logger.LogTrace($"Copied page template {exportPageTemplate.Title} to {Path.GetFileName(exportPath)}");
            }

        }

        private async Task ExportMasterPages()
        {
            // get all master pages
            var masterPages = await _masterPageManager.GetMasterPagesAsync(_site.Id);
            int sequence = 0;

            // get published version info for each master page
            foreach(var master in masterPages)
            {
                var versionInfo = await _masterPageManager.GetPublishedVersion(master.Id);

                // only export published masterPages
                if (versionInfo != null)
                {
                    // Create the entry in settings (with friendly ambiguous id)
                    var masterPageSettings = new SiteTemplateMasterPage
                    {
                        Id = $"master{++sequence}",
                        Title = master.Title,
                        Template = master.TemplateId,
                    };
          
                    // write the tree content to file
                    var seedFileName = masterPageSettings.Id + ".json";

                    await ExportTreeData(seedFileName, versionInfo);

                    // map data file to seed file collection
                    masterPageSettings.SeedData = new string[] { seedFileName };

                    // add to template and map to original id
                    _exportTemplate.MasterPages.Add(masterPageSettings);
                    _masterPageMap.Add(master.Id, masterPageSettings);

                    _logger.LogTrace($"Exported master page {master.Title} to {seedFileName}");
                }
            }
        }

        private async Task ExportPages()
        {
            // get all pages
            var sitePages = await _pageManager.GetPagesAsync(_site.Id);
            int sequence = 0;

            // ordering by Path
            sitePages = sitePages.OrderBy(x => x.Path).ToList();

            // get published version info for each master page
            foreach (var page in sitePages)
            {
                var versionInfo = await _pageManager.GetPublishedVersion(page.Id);
               
                // only export published masterPages
                if (versionInfo != null)
                {
                    // retreive corresponding master page settings from template
                    var masterPageSettings = _masterPageMap[page.PageMasterId];

                    // Create the entry in settings (with friendly ambiguous id)
                    var pageSettings = new SiteTemplatePage
                    {
                        Id = $"page{++sequence}",
                        Title = page.Title,
                        Path = page.Path,
                        Master = masterPageSettings.Id,
                    };

                    // write the tree content to file
                    var seedFileName = pageSettings.Id + ".json";

                    await ExportTreeData(seedFileName, versionInfo);

                    // map data file to seed file collection
                    pageSettings.SeedData = new string[] { seedFileName };

                    // add to template and map to original id
                    _exportTemplate.Pages.Add(pageSettings);
                    _pageMap.Add(page.Id, pageSettings);

                    _logger.LogTrace($"Exported page {page.Title} to {seedFileName}");
                }
            }
        }

        private async Task ExportTreeData(string fileName, ContentVersion version)
        {
            var exportPath = Path.Combine(_exportDataFolder, fileName);
            var treeId = await _contentManager.GetContentTreeId(version.ContentType, version.ContentId, version.VersionCode);
            var treeNodes = await _contentManager.GetContentNodes(treeId);

            // Don't export the embedded site-nav at this time until site templates 
            // support exporting / importing of nav menus. By not exporting, the site 
            // will create the main nav using it's named model provider 

            treeNodes = treeNodes.Where(x => x.Zone != "site-nav");

            // Recursively build the export model starting with root nodes
            var exportData = RecursivelyExportTreeData(treeNodes, null);

            // Serialize and write the data file
            var content = JsonConvert.SerializeObject(exportData, _jsonSettings);

            File.WriteAllText(exportPath, content);
        }

        private IEnumerable<object> RecursivelyExportTreeData(IEnumerable<ContentNode> treeNodes, string parentNodeId)
        {
            var exportBranch = new List<object>();
            var treeBranch = treeNodes
                .Where(x => x.ParentId == parentNodeId) 
                .OrderBy(x => x.Zone)
                .ThenBy(x => x.Index);

            foreach(var node in treeBranch)
            {
                try
                {
                    var style = node.GetStyle();

                    var exportNode = new
                    {
                        Zone = node.Zone,
                        Type = node.WidgetType,
                        View = node.ViewId,
                        Style = new
                        {
                            Background = style.BackgroundClass,
                            Classes = style.NodeClasses,
                            PaddingTop = style.PaddingTop,
                            PaddingBottom = style.PaddingBottom
                        },
                        Model = _widgetProvider.ExportSettings(node.WidgetType, node.WidgetId),
                        Children = RecursivelyExportTreeData(treeNodes, node.Id)
                    };

                    exportBranch.Add(exportNode);
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex);
                }
            }

            return exportBranch.Count > 0 ? exportBranch : null;
        }

        private bool IsNotEmptyCss(string contents)
        {
            if (contents == null) return true;

            var regex = new Regex(@"{[\s\w\W]+:[\s\w\W]+}");
            var result = regex.Match(contents);

            return result.Success;
        }

        private static void CopyFolder(string sourceFolderPath, string destFolderPath, bool deepCopy)
        {
            var sourceFolderInfo = new DirectoryInfo(sourceFolderPath);

            if (!sourceFolderInfo.Exists)
                throw new DirectoryNotFoundException(sourceFolderPath);
         
            // Create destination folder if needed
            if (!Directory.Exists(destFolderPath))
            {
                Directory.CreateDirectory(destFolderPath);
            }

            // Get the files in the directory and copy them to the new location.            
            var filesToCopy = sourceFolderInfo.GetFiles();

            foreach (var file in filesToCopy)
            {
                file.CopyTo(Path.Combine(destFolderPath, file.Name), false);
            }

            // Recursively copy child folders
            if (deepCopy)
            {
                var foldersToCopy = sourceFolderInfo.GetDirectories();

                foreach (var folder in foldersToCopy)
                {
                    CopyFolder(folder.FullName, Path.Combine(destFolderPath, folder.Name), deepCopy);
                }
            }
        }

    }
}
