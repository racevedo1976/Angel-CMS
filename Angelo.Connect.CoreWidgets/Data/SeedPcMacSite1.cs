using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Rendering;
using Angelo.Plugins;

namespace Angelo.Connect.CoreWidgets.Data
{
    public class SeedPcMacSite1 : IPluginStartupAction
    {
        private ConnectDbContext _dbContext;
        private ContentManager _contentManager;
        private PageMasterManager _masterPageManager;

        public SeedPcMacSite1(ConnectDbContext dbContext, ContentManager contentManager, PageMasterManager masterPageManager)
        {
            _dbContext = dbContext;
            _contentManager = contentManager;
            _masterPageManager = masterPageManager;
        }
   
        public void Invoke()
        {
            var siteId = HtmlDbSeedConstants.SiteId_PcMac1;
            var primaryMasterPageId = HtmlDbSeedConstants.MasterId_PcMac1_Primary;
            var secondaryMasterPageId = HtmlDbSeedConstants.MasterId_PcMac1_Secondary;
            var homePageId = HtmlDbSeedConstants.PageId_PcMac1_Home;

            // Seed when no primary master page version exists. Otherwise assume already seeded. 
            if (_dbContext.ContentVersions.Where(x => x.ContentId == primaryMasterPageId).Count() == 0)
            {
                // Published Versions of Master Pages
                _masterPageManager.CreateInitialVersion(primaryMasterPageId, true).Wait();
                _masterPageManager.CreateInitialVersion(secondaryMasterPageId, true).Wait();

                // Published Version of Home Page with Custom Content
                SeedHomePage(homePageId);

                // Published Version of Other Pages with Generic Content
                SeedOtherPages(siteId, homePageId);
            }

        }

   
        private void SeedHomePage(string homePageId)
        {
            // init version & content tree
            var publishedVersion = new ContentVersion
            {
                ContentId = homePageId,
                ContentType = HtmlDbSeedConstants.ContentType_SitePage,
                VersionLabel = "Master Page Content Added",
                UserId = "admin",
                Status = ContentStatus.Published,
            };

            var contentTree = new ContentTree(publishedVersion);

            _dbContext.ContentVersions.Add(publishedVersion);
            _dbContext.ContentTrees.Add(contentTree);


            // styles
            var topImageStyle = new ContentStyle
            {
                FullWidth = true,
                BackgroundClass = SiteTemplateConstants.Backgrounds.Default,
            };

            var topGridStyle = new ContentStyle
            {
                BackgroundClass = SiteTemplateConstants.Backgrounds.Default,
                PaddingTop = SiteTemplateConstants.RootContentPaddingTop,
                PaddingBottom = SiteTemplateConstants.RootContentPaddingBottom
            };

            var heroUnitStyle = new ContentStyle
            {
                BackgroundClass = SiteTemplateConstants.Backgrounds.Fancy,
                PaddingTop = SiteTemplateConstants.RootContentPaddingTop,
                PaddingBottom = SiteTemplateConstants.RootContentPaddingBottom
            };

            var bottomGridStyle = new ContentStyle
            {
                BackgroundClass = SiteTemplateConstants.Backgrounds.Contrast,
                PaddingTop = SiteTemplateConstants.RootContentPaddingTop,
                PaddingBottom = SiteTemplateConstants.RootContentPaddingBottom
            };


            // content nodes
            var homePageContentNodes = new ContentNode[]
            {
                // full width zone with image
                new ContentNode(topImageStyle)
                {
                    Id = Guid.NewGuid().ToString(),
                    ContentTreeId = contentTree.Id,
                    Zone = "body",
                    Index = 0,
                    WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImageMain,
                    WidgetType = "image",
                    ViewId = "image-fitted",
                    Locked = false,
                },

                // 4 images in a grid
                new ContentNode(topGridStyle)
                {
                    Id = Guid.NewGuid().ToString(),
                    ContentTreeId = contentTree.Id,
                    ParentId = null,
                    Zone = "body",
                    Index = 1,
                    WidgetId = null,
                    WidgetType = "zone",
                    ViewId = "zone-4",
                    Locked = false,

                    ChildNodes = new ContentNode[]
                    {
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-1",
                            Index = 0,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImage1,
                            WidgetType = "image",
                            ViewId = "image-fitted",
                            Locked = false
                        },
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-2",
                            Index = 1,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImage2,
                            WidgetType = "image",
                            ViewId = "image-fitted",
                            Locked = false
                        },
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-3",
                            Index = 2,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImage3,
                            WidgetType = "image",
                            ViewId = "image-fitted",
                            Locked = false
                        },
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-4",
                            Index = 3,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImage4,
                            WidgetType = "image",
                            ViewId = "image-fitted",
                            Locked = false
                        }
                    }
                },

                // Hero Unit
                new ContentNode(heroUnitStyle)
                {
                    Id = Guid.NewGuid().ToString(),
                    ContentTreeId = contentTree.Id,
                    ParentId = null,
                    Zone = "body",
                    Index = 2,
                    WidgetId = null,
                    WidgetType = "zone",
                    ViewId = "zone-1",
                    Locked = false,

                    ChildNodes = new ContentNode[]
                    {
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-1",
                            Index = 1,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Hero_InspirationWelcome,
                            WidgetType = "hero",
                            ViewId = "hero-hero1",
                            Locked = false,
                        }
                    }
                },

                // 4 more images in a grid
                new ContentNode(bottomGridStyle)
                {
                    Id = Guid.NewGuid().ToString(),
                    ContentTreeId = contentTree.Id,
                    ParentId = null,
                    Zone = "body",
                    Index = 3,
                    WidgetId = null,
                    WidgetType = "zone",
                    ViewId = "zone-4",
                    Locked = false,
                    ChildNodes = new ContentNode[]
                    {
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-1",
                            Index = 0,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImage5,
                            WidgetType = "image",
                            ViewId = "image-fitted",
                            Locked = false
                        },
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-2",
                            Index = 1,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImage6,
                            WidgetType = "image",
                            ViewId = "image-fitted",
                            Locked = false
                        },
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-3",
                            Index = 2,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImage7,
                            WidgetType = "image",
                            ViewId = "image-fitted",
                            Locked = false
                        },
                        new ContentNode
                        {
                            Id = Guid.NewGuid().ToString(),
                            ContentTreeId = contentTree.Id,
                            Zone = "cell-4",
                            Index = 3,
                            WidgetId = HtmlDbSeedConstants.WidgetId_Image_InspirationImage8,
                            WidgetType = "image",
                            ViewId = "image-fitted",
                            Locked = false
                        }
                    }
                }
            };


            _dbContext.ContentNodes.AddRange(homePageContentNodes);
            _dbContext.SaveChanges();
        }

        private void SeedOtherPages(string siteId, string homePageId)
        {
            var otherPages = _dbContext.Pages.Where(x => x.SiteId == siteId && x.Id != homePageId).ToList();

            foreach(var page in otherPages)
            {

                // Init published version and content tree
                var publishedVersion = new ContentVersion
                {
                    ContentId = page.Id,
                    ContentType = HtmlDbSeedConstants.ContentType_SitePage,
                    VersionLabel = "Master Page Content Added",
                    UserId = "admin",
                    Status = ContentStatus.Published,
                };

                var contentTree = new ContentTree(publishedVersion);

                _dbContext.ContentVersions.Add(publishedVersion);
                _dbContext.ContentTrees.Add(contentTree);
                _dbContext.SaveChanges();

                // build generic tree content
                SeedGenericTreeContent(contentTree, page);
            }

        }


        private void SeedGenericTreeContent(ContentTree contentTree, Page sitePage)
        {
            var treeBuilder = _contentManager.CreateTreeBuilder(contentTree);

            treeBuilder.AddRootContent("body", new Models.Title
            {
                Text = sitePage.Title
            });

            treeBuilder.AddRootContent("body", content => {
                content.WidgetType = "alert";
                content.ModelName = "page-seed-notice";
            });

            treeBuilder.AddRootContent("body", content => {
                content.WidgetType = "html";
                content.ModelName = "lorem-ipsum";
            });

            treeBuilder.SaveChanges();
        }
    }
}
