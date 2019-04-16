using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Configuration;
using Angelo.Connect.Models;
using Angelo.Connect.Rendering;
using Angelo.Connect.Services;
using Angelo.Connect.UI.Components;
using Angelo.Connect.UI.ViewModels;

namespace Angelo.Connect.Rendering
{
    public static class ControllerExtensions2
    {

        public static IActionResult PartialContentView(this Controller controller, Action<ContentBindings> options)
        {
            var bindings = new ContentBindings();
            options.Invoke(bindings);

            return PartialContentView(controller, bindings);
        }


        public static IActionResult PartialContentView(this Controller controller, ContentBindings contentBindings)
        {
            var services = controller.HttpContext.RequestServices;
            var context = CreateBaseRenderingContext(services, contentBindings);

            SetRenderingContext(controller, context);

            return controller.PartialView("/UI/Views/Rendering/PartialShell.cshtml", context);
        }

        public static IActionResult MasterPageView(this Controller controller, ContentBindings contentBindings, MasterPageSettings masterPageSettings, ShellSettings shellSettings = null)
        {
            var services = controller.HttpContext.RequestServices;
            var baseContext = CreateBaseRenderingContext(services, contentBindings);
            var ShellContext = CreateMasterRenderingContext(services, baseContext, masterPageSettings, shellSettings);

            SetRenderingContext(controller, ShellContext);

            return controller.View("/UI/Views/Rendering/MasterShell.cshtml", ShellContext);
        }

        public static IActionResult MasterPageView(this Controller controller, ContentBindings contentBindings, ShellSettings shellSettings = null)
        {
            var services = controller.HttpContext.RequestServices;
            var baseContext = CreateBaseRenderingContext(services, contentBindings);
            var ShellContext = CreateMasterRenderingContext(services, baseContext, shellSettings);

            SetRenderingContext(controller, ShellContext);

            return controller.View("/UI/Views/Rendering/MasterShell.cshtml", ShellContext);
        }

        public static IActionResult MasterPageView(this Controller controller, string contentViewPath, object contentViewModel, ShellSettings shellSettings = null)
        {
            var services = controller.HttpContext.RequestServices;
            var baseContext = CreateGenericRenderingContext(contentViewPath, contentViewModel);
            var ShellContext = CreateMasterRenderingContext(services, baseContext, shellSettings);

            SetRenderingContext(controller, ShellContext);

            return controller.View("/UI/Views/Rendering/MasterShell.cshtml", ShellContext);
        }

        public static IActionResult MasterPageView(this Controller controller, string contentViewPath, string windowTitle)
        { 
            return MasterPageView(controller, contentViewPath, null, windowTitle);
        }

        public static IActionResult MasterPageView(this Controller controller, string contentViewPath, object contentViewModel, string windowTitle)
        {
            var shellSettings = new ShellSettings(windowTitle);

            return MasterPageView(controller, contentViewPath, contentViewModel, shellSettings);
        }

     
        public static IActionResult MasterPageDesigner(this Controller controller, string masterPageId, string masterPageVersionCode, ShellSettings shellSettings)
        {
            var contentView = "/UI/Views/Public/Layouts/Empty.cshtml";
            var baseContext = CreateGenericRenderingContext(contentView, null);

            var masterPageSettings = new MasterPageSettings
            {
                MasterPageId = masterPageId,
                VersionCode = masterPageVersionCode,
                Editable = true
            };

            var services = controller.HttpContext.RequestServices;
            var ShellContext = CreateMasterRenderingContext(services, baseContext, shellSettings);

            SetRenderingContext(controller, ShellContext);

            return controller.View("/UI/Views/Rendering/MasterShell.cshtml", ShellContext);
        }


        private static RenderingContext CreateGenericRenderingContext(string contentViewPath, object contentViewModel)
        {
            var bindings = new ContentBindings
            {
                ContentType = "Generic",
                ViewPath = contentViewPath,
                ViewModel = contentViewModel
            };

            return new RenderingContext(bindings);
        }

        private static RenderingContext CreateBaseRenderingContext(IServiceProvider services, ContentBindings contentBindings)
        {
            var contentManager = services.GetService<ContentManager>();
            var renderingContext = new RenderingContext(contentBindings);

            if (!string.IsNullOrEmpty(contentBindings.ContentType) && !string.IsNullOrEmpty(contentBindings.ContentId) && contentBindings.ContentType != ContentBindings.GENERIC_CONTENT_TYPE)
            {
                var version = contentBindings.VersionCode == null
                    ? contentManager.GetPublishedVersionInfo(contentBindings.ContentType, contentBindings.ContentId).Result
                    : contentManager.GetVersionInfo(contentBindings.ContentType, contentBindings.ContentId, contentBindings.VersionCode).Result;

                if (version != null)
                {
                    renderingContext.ContentVersionInfo = ToVersionInfo(version);
                    renderingContext.ContentTreeId = contentManager.GetContentTreeId(version).Result;
                }
            }

            return renderingContext;
        }

        private static ShellContext CreateMasterRenderingContext(IServiceProvider services, RenderingContext baseContext, ShellSettings shellSettings)
        {
            var contentManager = services.GetService<ContentManager>();
            var masterPageManager = services.GetService<PageMasterManager>();
            var siteContextAccessor = services.GetService<IContextAccessor<SiteContext>>();
            var siteContext = siteContextAccessor.GetContext();

            var masterPage = masterPageManager.GetSiteDefaultAsync(siteContext.SiteId).Result;

            if (masterPage == null || masterPage.SiteId != siteContext.SiteId)
                throw new Exception($"Could not locate the deefault Master Page for Site {siteContext.SiteId}");

            var masterPageSettings = new MasterPageSettings
            {
                MasterPageId = masterPage.Id,
                VersionCode = null, // will retrieve published version
                Editable = false
            };

            
            return CreateMasterRenderingContext(services, baseContext, masterPageSettings, shellSettings);
        }

        private static ShellContext CreateMasterRenderingContext(IServiceProvider services, RenderingContext baseContext, MasterPageSettings masterPageSettings, ShellSettings shellSettings)
        {
            var siteContextAccessor = services.GetService<IContextAccessor<SiteContext>>();
            var contentManager = services.GetService<ContentManager>();
            var masterPageManager = services.GetService<PageMasterManager>();

            var siteContext = siteContextAccessor.GetContext();
            var masterPage = masterPageManager.GetByIdAsync(masterPageSettings.MasterPageId).Result;
            var masterPageContentType = typeof(PageMaster).FullName;

            if (masterPage == null)
                throw new Exception($"Could not locate a Master Page {masterPageSettings.MasterPageId}");

            var masterPageTemplate = siteContext.Template.PageTemplates.FirstOrDefault(x => x.Id == masterPage.TemplateId);

            if (masterPageTemplate == null)
                throw new Exception($"Could not locate Master Page Template {masterPage.TemplateId}");

            // Version Info & Content Tree
            var masterPageVersion = masterPageSettings.VersionCode == null
                ? contentManager.GetPublishedVersionInfo(masterPageContentType, masterPage.Id).Result
                : contentManager.GetVersionInfo(masterPageContentType, masterPage.Id, masterPageSettings.VersionCode).Result;

            if (masterPageVersion == null)
                throw new Exception($"Could not locate version info for Master Page {masterPage.Id}");

            var masterPageTreeId = contentManager.GetContentTreeId(masterPageVersion).Result;

            // Build master page bindings
            return new ShellContext(baseContext)
            {
                MasterPageId = masterPage.Id,
                MasterPageVersionInfo = ToVersionInfo(masterPageVersion),
                MasterPageTreeId = masterPageTreeId,
                MasterPageTemplate = masterPageTemplate,
                MasterPageEditable = masterPageSettings.Editable,

                MetaTags = shellSettings.MetaTags,
                Toolbar = shellSettings.Toolbar,
                WindowTitle = shellSettings.WindowTitle
            };
        }

        private static void SetRenderingContext(Controller controller, RenderingContext renderingContext)
        {
            // Add to view data for rendering
            controller.ViewData.SetRenderingContext(renderingContext);


            // Set Initial Tree Context
            if(renderingContext is ShellContext)
            {
                var ShellContext = renderingContext as ShellContext;

                controller.ViewData.SetTreeContext(new TreeContext
                {
                    Editable = ShellContext.MasterPageEditable,
                    TreeId = ShellContext.MasterPageTreeId,
                    AllowContainers = true
                });
            }
            else
            {
                controller.ViewData.SetTreeContext(new TreeContext
                {
                    Editable = renderingContext.ContentEditable,
                    TreeId = renderingContext.ContentTreeId
                });
            }
        }

        private static VersionInfo ToVersionInfo(ContentVersion version)
        {
            return new VersionInfo
            {
                VersionCode = version.VersionCode,
                VersionLabel = version.VersionLabel,
                Created = version.Created,
                Status = version.Status,
                UserId = version.UserId
            };
        }
    }
}
