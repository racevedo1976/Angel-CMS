using System;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Angelo.Jobs.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Angelo.Drive.Services;
using Angelo.Plugins;
using Angelo.Connect.Abstractions;
using System.Collections.Generic;
using Angelo.Connect.Models;
using Angelo.Connect.Widgets;

namespace Angelo.Drive
{
    public static class PluginExtensions
    {
        public static void RegisterDocumentType<TDocument>(this PluginBuilder plugin, Action<PluginOptions> options)
        {
            throw new NotImplementedException();
        }

        public static void UseDocumentService<TDocumentService, TDocument>(this PluginOptions options) 
            where TDocumentService : IDocumentService<TDocument>
            where TDocument : class, IDocument
        {
            throw new NotImplementedException();
        }

        public static void UseFolderManager<TFolderManager>(this PluginOptions options) where TFolderManager : IFolderManager
        {
            throw new NotImplementedException();
        }

        public static void UseFullPage<TView>(this PluginOptions options)
        {
            throw new NotImplementedException();
        }

        public static void UseEditForm<TView>(this PluginOptions options)
        {
            throw new NotImplementedException();
        }

        public static void UseThumbnail<TView>(this PluginOptions options)
        {
            throw new NotImplementedException();
        }

        public static void ConfigureGridView(this PluginOptions options, Action<PluginGrid> configure)
        {
            throw new NotImplementedException();
        }

        public static void Add(this ICollection<string> columns, Func<FileDocument, string> name)
        {
            throw new NotImplementedException();
        }
    }
}
