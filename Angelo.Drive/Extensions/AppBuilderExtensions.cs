using System;
using System.IO;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using Angelo.Drive.Services;
using Angelo.Connect.Abstractions;
using Angelo.Jobs.Server;
using Angelo.Connect.Models;

namespace Angelo.Drive
{
    public static class AppBuilderExtensions
    {
        public static void UseDrives(this IApplicationBuilder app)
        {
            Ensure.NotNull(app);

            var settings = app.ApplicationServices.GetService<DriveSettings>();

            Ensure.That(Directory.Exists(settings.FileSystemRoot));

            //var db = app.ApplicationServices.GetService<DriveDbContext>();
            //var folderManager = app.ApplicationServices.GetService<IFolderManager<FileDocument>>();

            //app.ApplicationServices.GetService<LibraryIOService>().EnsurePhysicalSeeded(db, folderManager);
        }

        public static void UseAegisOIDC(this IApplicationBuilder app)
        {        
            var aegisOptions = app.ApplicationServices.GetService<IOptions<AegisOptions>>()?.Value;

            Ensure.NotNull(aegisOptions);

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseIdentityServerAuthentication(
                new IdentityServerAuthenticationOptions
                {
                    Authority = aegisOptions.Authority,
                    ApiName = aegisOptions.ApiName,
                    ApiSecret = aegisOptions.ApiSecret,                
                    NameClaimType = aegisOptions.NameClaimType,
                    RoleClaimType = aegisOptions.RoleClaimType,
                    RequireHttpsMetadata = false,
                    AutomaticAuthenticate = true,
                    AutomaticChallenge = true
                }
            );
        }
    }
}
