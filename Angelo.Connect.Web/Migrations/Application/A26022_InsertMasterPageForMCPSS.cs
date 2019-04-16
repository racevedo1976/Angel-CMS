using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Common.Migrations;
using Angelo.Common.Extensions;
using Angelo.Connect.Data;
using Angelo.Connect.Models;
using Angelo.Identity.Models;
using Angelo.Connect.Services;


namespace Angelo.Connect.Web.Migrations.Application
{
    public class A26022_InsertMasterPageForMCPSS : IAppMigration
    {
        
        public string Id { get; } = "A26022";

        public string Migration { get; } = "Insert master page for Mobile County";
        
        private ConnectDbContext _connectDb;
        private SitePublisher _sitePublisher;
        
        public A26022_InsertMasterPageForMCPSS(ConnectDbContext identityDb, SitePublisher sitePublisher)
        {
            _connectDb = identityDb;
            _sitePublisher = sitePublisher;
        }

        public async Task<MigrationResult> ExecuteAsync()
        {
            var output = new System.Text.StringBuilder();

            // Fail if cannot connect to db
            if (_connectDb.Database.TryTestConnection() == false)
                return MigrationResult.Failed("Cannot connect to database.");

            // site for each environment
            List<string> siteDomains = new List<string>(new string[] { "mcpss.schoolinsites.com", "localhost:60002", "cityscapetemplateqa.schoolinsites.com", "cityscapetemplateuat.schoolinsites.com" });
            List<string> siteIds = new List<string>();
            Site site = new Site();

            siteIds = _connectDb.SiteDomains.Where(x => siteDomains.Any(y => y == x.DomainKey)).Select(id => id.SiteId).ToList();

            foreach(var siteId in siteIds)
            {
                site.Id = siteId;
                _sitePublisher.CreateSysMasterPageV0(site);
            }
            
            
            return MigrationResult.Success($"System Master Page created for Mobile County");
        }
    }
}




