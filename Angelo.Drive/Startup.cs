using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Serialization;

using Angelo.Connect.Data;
using Angelo.Common.Extensions;
using Angelo.Drive.Services;
using Angelo.Drive.Jobs;
using Angelo.Jobs;
using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Angelo.Drive.Data;
using Angelo.Connect.Extensions;
using Angelo.Connect.Abstractions;
using Angelo.Common.Abstractions;
using Angelo.Plugins;
using Angelo.Drive.Abstraction;


namespace Angelo.Drive
{
    public class Startup
    { 

        private IHostingEnvironment Environment { get; set; }
        private IConfigurationRoot Configuration { get; set; }
        private string FileSystemRootPath { get; set; }
        private string FileSystemCachePath { get; set; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("settings.json", optional:false, reloadOnChange: true)
                .AddJsonFile($"settings.{env.EnvironmentName}.json", optional: true);
               
            Environment = env;
            Configuration = builder.Build();

            FileSystemRootPath = System.IO.Path.Combine(env.ContentRootPath, "wwwroot");
            FileSystemCachePath = System.IO.Path.Combine(env.ContentRootPath, "wwwroot", "cache");
            EnsurePathExists(FileSystemRootPath);
            EnsurePathExists(FileSystemCachePath);
            EnsurePathExists(System.IO.Path.Combine(FileSystemCachePath, LibraryIOService.ThumbnailCachePath));
        }

        public Startup(PluginBuilder pluginBuilder)
        {
            pluginBuilder.RegisterDocumentType<FileDocument>(options =>
            {
                // Register DocumentService
                options.UseDocumentService<FileDocumentService, FileDocument>();

                // Register FolderService
                //options.UseFolderManager<DefaultFolderManager>();

                // Register UI components
                //options.UseFullPage<DocumentFullPageViewComponent>();
                //options.UseEditForm<DocumentEditFormViewComponent>();
                //options.UseThumbnail<DocumentThumbnailViewComponent>();

                options.ConfigureGridView(grid =>
                {
                    grid.Columns.Add(x => x.Title);
                    grid.Columns.Add(x => x.GetMimeType());
                });
            });
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Configuring Options
            services.Configure<AegisOptions>(Configuration.GetSection("OpenIdConnect"));
            services.Configure<CultureOptions>(Configuration.GetSection("CultureOptions"));
            services.AddOptions();

            var serviceProvider = services.BuildServiceProvider();
            var aegisOptions = serviceProvider.GetService<IOptions<AegisOptions>>().Value;
            var cultureOptions = serviceProvider.GetService<IOptions<CultureOptions>>().Value;

            // Adding Drives
            services.AddDrives(options => {
                //options.DriveDbConnectionString = Configuration.GetConnectionString("DriveDB");
                //options.LogDbConnectionString = Configuration.GetConnectionString("ConnectDB");
                options.FileSystemRoot = FileSystemRootPath;
                options.FileSystemCache = FileSystemCachePath;
            }, Configuration);

            // Adding Connect
            services.AddConnectCore();
            services.AddDbContext<ConnectDbContext>(options => {
                options.UseSqlServer(
                    Configuration.GetConnectionString("ConnectDB"),
                    settings =>
                    {
                        settings.MigrationsAssembly("Angelo.Connect.Web");
                        settings.UseRowNumberForPaging();
                    }
                );
            });
            
            // Adding Jobs
            services.AddJobs(options => {
                options.UseSqlServer( db => {
                    db.ConnectionString = Configuration.GetConnectionString("ConnectDB");
                    db.Schema = "app";
                });
            });
            services.AddTransient<JobZip>();

            // Adding MVC
            services.AddMvc(options =>
                {
                    options.CacheProfiles.Add("Default",
                        new CacheProfile()
                        {
                            Location = ResponseCacheLocation.Any,
                           
                            Duration = 2592000  //= in seconds , 3600 for an hour, 2592000 = month, 86400 = day
                        });
                }
            ).AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
            });

            services.Configure<FormOptions>(options =>
            {
                options.MultipartBodyLengthLimit = long.MaxValue;
                options.BufferBodyLengthLimit = long.MaxValue;
                options.BufferBody = true;
            });

            // Startup Routines (need to pass FSroot, so I can't usethe extension method
            serviceProvider = services.BuildServiceProvider();
            var provider = new Angelo.Drive.Data.SeedDocumentData(serviceProvider.GetService<ConnectDbContext>(),
                                                FileSystemRootPath,
                                                serviceProvider.GetService<IFolderManager<FileDocument>>(),
                                                serviceProvider.GetService<IDocumentService<FileDocument>>(),
                                                serviceProvider.GetService<IDocumentUploadService<FileDocument>>());
            //services.AddTransient<IStartupAction, SeedDocumentData>(
            //   x => provider
            //);
            services.AddTransient<SeedDocumentData>(
               x => provider
            );

           
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();
            loggerFactory.AddDebug();

            if (!env.IsProduction())
            {
                //using (var scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
                //{
                //    scope.ServiceProvider.GetService<DriveDbContext>().EnsureMigrated();
                //}
            }
       
            app.UseCors(policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
            });

            // Removing OID temporarily to get the base AJAX logic fixed from the FileDoc->Core refactor
            //app.UseAegisOIDC();
            app.UseMvcWithDefaultRoute();
            app.UseDrives();
            app.UseJobs();

            // run startup methods
            app.RunStartupActions();
        }

        private void EnsurePathExists(string path)
        {
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}