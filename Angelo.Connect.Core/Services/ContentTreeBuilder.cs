using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Widgets;
using Angelo.Connect.Rendering;

namespace Angelo.Connect.Services
{
    public class ContentTreeBuilder
    {
        private WidgetProvider _widgetProvider;
        private ContentTree _targetTree;
        private ConnectDbContext _dbContext;

        private const string DEFAULT_ROOT_ZONE = "body";

        public ContentTreeBuilder
        (
            ConnectDbContext dbContext,
            WidgetProvider widgetProvider, 
            ContentTree contentTree
        )
        {
            _dbContext = dbContext;
            _widgetProvider = widgetProvider;
            _targetTree = contentTree;

            if(_targetTree.ContentNodes == null)
            {
                _targetTree.ContentNodes = new List<ContentNode>();
            }
        }

        public ContentTreeBuilder SeedFromFile(string jsonFilePath)
        {
            if (string.IsNullOrEmpty(jsonFilePath))
                throw new ArgumentNullException(nameof(jsonFilePath));

            if (!File.Exists(jsonFilePath))
                throw new NullReferenceException($"Failed to seed content: Missing File {jsonFilePath}");

            var jsonData = File.ReadAllText(jsonFilePath).Trim();

            if (string.IsNullOrEmpty(jsonData))
                throw new NullReferenceException($"Failed to seed content: File is empty {jsonFilePath}");


            var seedData = JsonConvert.DeserializeObject<IEnumerable<SeedEntry>>(jsonData);

            CreateRecursively(seedData, _targetTree.ContentNodes, null);

            return this;
        }

        public ContentTreeBuilder AddContent(Action<NamedContent> optionsBuilder, string zoneName = null, string parentNodeId = null)
        {
            ContentNode node = null;
            var options = new NamedContent();

            optionsBuilder.Invoke(options);

            if (options.WidgetType != null && options.ModelName != null)
            {
                var content = _widgetProvider.Create(options.WidgetType, options.ModelName);

                if (content == null)
                    throw new Exception($"Could not build tree content. Invalid ModelName: {options.ModelName}.");

                node = CreateContentNode(options.WidgetType, options.Style, content, zoneName, parentNodeId);
                _targetTree.ContentNodes.Add(node);
            }

            return this;
        }

        public ContentTreeBuilder AddContent(IWidgetModel content, string zoneName = null, string parentNodeId = null)
        {
            ContentNode node = null;
            var config = _widgetProvider.GetWidgetConfig(content);

            _widgetProvider.Create(config.WidgetType, content);

            node = CreateContentNode(config.WidgetType, null, content, zoneName, parentNodeId);

            return this;
        }

        public BranchBuilder AddLayout(Action<LayoutContent> layoutBuilder, string zoneName = null, string parentNodeId = null)
        {
            ContentNode node = null;
            var options = new LayoutContent();

            layoutBuilder.Invoke(options);

            node = CreateLayoutNode(options.LayoutType, options.Style, zoneName, parentNodeId);
            _targetTree.ContentNodes.Add(node);

            return new BranchBuilder(this, node);
        }

        public void AddRootContent(string rootZoneName, Action<NamedContent> optionsBuilder)
        {
            AddContent(optionsBuilder, rootZoneName);
        }

        public void AddRootContent(string rootZoneName, IWidgetModel content)
        {
            AddContent(content, rootZoneName);
        }

        public void AddRootContent(Action<NamedContent> optionsBuilder)
        {
            AddContent(optionsBuilder);
        }

        public void AddRootContent(IWidgetModel content)
        {
            AddRootContent(content);
        }

        public BranchBuilder AddRootLayout(string rootZoneName, Action<LayoutContent> layoutBuilder)
        {
            return AddLayout(layoutBuilder, rootZoneName);
        }

        public BranchBuilder AddRootLayout(Action<LayoutContent> layoutBuilder)
        {
            return AddLayout(layoutBuilder);
        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }

        private ContentNode CreateContentNode(string widgetType, ContentStyle style, IWidgetModel widgetModel, string zoneName = null, string parentNodeId = null)
        {
            var widgetConfig = _widgetProvider.GetWidgetConfig(widgetType);

            if (widgetConfig == null)
                throw new Exception($"Could not build tree content. Invalid ContentType: {widgetType}.");

            var widgetViewId = widgetConfig.GetDefaultViewId();

            if (widgetViewId == null)
                throw new Exception($"Could not build tree content. No default view for ContentType: {widgetType}.");

            if (zoneName == null)
                zoneName = DEFAULT_ROOT_ZONE;

            var zoneIndex = _targetTree.ContentNodes.Where(x =>
                    x.ParentId == parentNodeId 
                    && x.Zone == zoneName
                ).Count();

            // if we made it this far we can create the node
            return new ContentNode(style)
            {
                Id = Guid.NewGuid().ToString("N"),
                ContentTreeId = _targetTree.Id,
                ParentId = parentNodeId,
                Zone = zoneName,
                Index = zoneIndex,
                WidgetType = widgetType,
                WidgetId = widgetModel?.Id,
                ViewId = widgetViewId,
            };
        }

        private ContentNode CreateLayoutNode(string layoutType, ContentStyle style, string zoneName = null, string parentNodeId = null)
        {
            var widgetType = "zone";
            var widgetConfig = _widgetProvider.GetWidgetConfig(widgetType);
            var widgetViewId = layoutType;

            if (widgetViewId == null || !widgetConfig.Views.Any(x => x.Id == layoutType))
                throw new Exception($"Could not build content zone. Invalid Layout: {layoutType}.");

            if (zoneName == null)
                zoneName = DEFAULT_ROOT_ZONE;

            var zoneIndex = _targetTree.ContentNodes.Where(x =>
                    x.ParentId == parentNodeId
                    && x.Zone == zoneName
                ).Count();

            // if we made it this far we can create the node
            // layouts have an empty model right now - so any guid will work
            return new ContentNode(style)
            {
                Id = Guid.NewGuid().ToString("N"),
                ContentTreeId = _targetTree.Id,
                ParentId = parentNodeId,
                Zone = zoneName,
                Index = zoneIndex,
                WidgetType = widgetType,
                WidgetId = Guid.NewGuid().ToString("N"), 
                ViewId = widgetViewId,
            };
        }

        private void CreateRecursively(IEnumerable<SeedEntry> seedEntries, ICollection<ContentNode> nodeCollection, ContentNode parentNode)
        {
            var branch = new List<ContentNode>();

            if (seedEntries == null) return;

            foreach (var seedItem in seedEntries)
            {
                ContentNode node = null;
                ContentStyle style = new ContentStyle();

                // TODO: Css classes have been moved under style (remove once old templates have been updated)
                if (!string.IsNullOrEmpty(seedItem.Css))
                    style.NodeClasses = seedItem?.Css;

                if (!string.IsNullOrEmpty(seedItem.Style?.Classes))
                    style.NodeClasses = seedItem.Style.Classes;

                if (!string.IsNullOrEmpty(seedItem.Style?.Background))
                    style.BackgroundClass = seedItem.Style.Background;

                if (!string.IsNullOrEmpty(seedItem.Style?.Padding))
                {
                    style.PaddingTop = seedItem.Style.Padding;
                    style.PaddingBottom = seedItem.Style.Padding;                    
                }
                else
                {
                    style.PaddingTop = seedItem.Style?.PaddingTop;
                    style.PaddingBottom = seedItem.Style?.PaddingBottom;
                }


                // TODO: rename "zone" to layout everywhere.  
                if (seedItem.Type == "zone")
                {
                    node = CreateLayoutNode(seedItem.View, style, seedItem.Zone, parentNode?.Id);
                }
                else
                {
                    var stringModel = JsonConvert.SerializeObject(seedItem.Model);
                    IWidgetModel widgetModel;

                    try
                    {
                        widgetModel = _widgetProvider.Create(seedItem.Type, stringModel);
                    }
                    catch (Exception)
                    {
                        widgetModel = null;
                    }

                    node = CreateContentNode(seedItem.Type, style, widgetModel, seedItem.Zone, parentNode?.Id);
                }


                nodeCollection.Add(node);

                if(seedItem.Children != null && seedItem.Children.Count() > 0)
                {
                    node.ChildNodes = new List<ContentNode>();
                    CreateRecursively(seedItem.Children, node.ChildNodes, node);
                }
            }
        }

        public class NamedContent
        {
            public string WidgetType { get; set; }
            public string ModelName { get; set; }
            public ContentStyle Style { get; set; }
        }

        public class LayoutContent
        {
            public string LayoutType { get; set; }
            public ContentStyle Style { get; set; }
        }

        public class SeedEntry
        {
            public string Zone { get; set; }
            public string Type { get; set; }
            public string View { get; set; }
            public string Css { get; set; }
            public SeedEntryStyle Style { get; set; }
            public JObject Model { get; set; }
            public IEnumerable<SeedEntry> Children { get; set; }
        }

        public class SeedEntryStyle
        {
            public string PaddingTop { get; set; }
            public string PaddingBottom { get; set; }
            public string Padding { get; set; }
            public string Background { get; set; }
            public string Classes { get; set; }
        }

        public class BranchBuilder
        {
            private ContentNode _parentNode;
            private ContentTreeBuilder _treeBuilder;

            public ContentTreeBuilder TreeBuilder
            {
                get {
                    return _treeBuilder;
                }
            }

            public BranchBuilder(ContentTreeBuilder treeBuilder, ContentNode parentNode)
            {
                _treeBuilder = treeBuilder;
                _parentNode = parentNode;

                if(_parentNode.ChildNodes == null)
                {
                    _parentNode.ChildNodes = new List<ContentNode>();
                }
            }

            public BranchBuilder AddChildContent(string zoneName, Action<NamedContent> optionsBuilder)
            {
                _treeBuilder.AddContent(optionsBuilder, zoneName, _parentNode.Id);

                return this;
            }

            public BranchBuilder AddChildContent(string zoneName, IWidgetModel content)
            {
                _treeBuilder.AddContent(content, zoneName, _parentNode.Id);

                return this;
            }

            public BranchBuilder AddChildLayout(string zoneName, Action<LayoutContent> layoutBuilder)
            {
                return _treeBuilder.AddLayout(layoutBuilder, zoneName, _parentNode.Id);
            }
        }

  
    }

}
