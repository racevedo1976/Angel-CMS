using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Abstractions;
using Angelo.Connect.Services;
using Angelo.Connect.Models;
using Angelo.Connect.Logging;
using Angelo.Jobs.Server;
using Microsoft.Extensions.Configuration;
using Angelo.Drive.Abstraction;
using Angelo.Drive.Services;

namespace Angelo.Drive
{
	public static class ServiceCollectionExtensions
	{
		public static void AddDrives(this IServiceCollection services, Action<DriveSettings> optionsBuilder, IConfigurationRoot configuration)
		{
            Ensure.NotNull(services);
            Ensure.NotNull(optionsBuilder);

            var settings = new DriveSettings();

            if (optionsBuilder != null)
                optionsBuilder.Invoke(settings);

            Ensure.NotNullOrEmpty(settings.FileSystemRoot);

            services.AddSingleton(settings);

            // Logging
            services.AddDbContext<DbLogContext>(options => {
                options.UseSqlServer(
                    configuration.GetConnectionString("ConnectDB"),
                    b => b.MigrationsAssembly("Angelo.Connect.Web")
                );
            });


            var thumProcessor = new Services.ThumbnailProcessor(settings.FileSystemRoot);
            var nominalResolutionProcessor = new Services.NominalResolutionProcessor(settings.FileSystemRoot);
            services.AddTransient<IImagePostProcessor, ThumbnailProcessor>(provider => thumProcessor);
            services.AddTransient<IImagePostProcessor, NominalResolutionProcessor>(provider => nominalResolutionProcessor);


            services.AddTransient<Services.LibraryManager>();
            
            var ioService = new Services.LibraryIOService(settings.FileSystemRoot, settings.FileSystemCache);
            services.AddTransient<IDocumentDownloadService<FileDocument>, Services.LibraryIOService>(
               provider => ioService
            );
            services.AddTransient<IDocumentUploadService<FileDocument>, Services.LibraryIOService>(
               provider => ioService
            );
            services.AddTransient<IDocumentThumbnailService<FileDocument>, Services.LibraryIOService>(
               provider => ioService
            );

            // Folders and documents
            services.AddConnectDocuments();

            // Document IO
            // NOTE: I really didn't want to add these classes here, but I have to to run the JobZip job
            services.AddTransient<Services.LibraryZipService>();
            services.AddTransient<Jobs.JobZip>(); // The UI can see Zip jobs
            services.AddTransient<Services.LibraryIOService>(
               provider => new Services.LibraryIOService(settings.FileSystemRoot, settings.FileSystemCache)
            );

         
        }

    }
}
