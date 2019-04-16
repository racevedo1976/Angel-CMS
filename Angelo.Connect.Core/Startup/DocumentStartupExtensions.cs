using System;

using Angelo.Connect.Abstractions;
//using Angelo.Connect.Documents;
using Angelo.Connect.Models;
using Angelo.Connect.Services;
using Angelo.Connect.Configuration;
using Angelo.Connect.Security;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DocumentStartupExtensions
    {
        public static IServiceCollection AddConnectDocuments(this IServiceCollection services, Action<ConnectDocumentOptions> optionsDelegate = null)
        {
            // Options
            if (optionsDelegate != null)
            {
                var options = new ConnectDocumentOptions();
                if (optionsDelegate != null)
                    optionsDelegate.Invoke(options);
                // registering options as singleton
                services.AddSingleton(options);
            }

            // Generic Document / Folder Services
            services.AddTransient<TagManager>();
            services.AddTransient<CategoryManager>();
            services.AddTransient<SharedFolderManager>();
            services.AddTransient<ResourceManager>();

            // FileDocument Services
            services.AddTransient<IFolderManager<FileDocument>, FolderManager<FileDocument>>();
            services.AddTransient<IDocumentService<FileDocument>, FileDocumentService>();

            //ContentBrowser Services
            services.AddTransient<FileFolderBrowser>();
            services.AddScoped<IContentBrowser, FileFolderBrowser>();
            services.AddScoped<IContentBrowser, SiteLibraryBrowser>();
            services.AddScoped<IContentBrowser, ClientLibraryBrowser>();
            //For testing and proof of concept only. Replace with real implementation for the content browser.
            // services.AddTransient<IContentBrowser, BlogCategoryBrowser>();
            // services.AddTransient<IContentBrowser, CalendarBrowser>();

            //Shared Content Provider
            services.AddTransient<ISharedContent, SharedContentProvider>();

            //ISecurityGroupProviders  Groups: Connection, notification, etc
            services.AddTransient<ISecurityGroupProvider, ConnectionGroupProvider>();

            

            return services;
        }
    }
}
