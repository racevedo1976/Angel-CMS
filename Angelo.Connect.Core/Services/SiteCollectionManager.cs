using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Connect.Data;
using Angelo.Connect.Models;

namespace Angelo.Connect.Services
{
    public class SiteCollectionManager
    {
        private ConnectDbContext _db;

        public SiteCollectionManager(ConnectDbContext dbContext)
        {
            _db = dbContext;
        }

        public async Task<string> CreateSiteCollectionAsync(SiteCollection siteCollection)
        {
            Ensure.That(siteCollection != null);

            try
            {
                siteCollection.Id = KeyGen.NewGuid();
                _db.SiteCollections.Add(siteCollection);

                await _db.SaveChangesAsync();
                return siteCollection.Id;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ICollection<SiteCollection>> GetSiteCollectionsAsync(string clientId)
        {
            return await _db.SiteCollections.Where(x => x.ClientId == clientId).OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<SiteCollection> GetSiteCollectionByIdAsync(string siteCollectionId)
        {
            return await _db.SiteCollections
                         .Where(x => x.Id == siteCollectionId)
                         .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Site>> GetSiteCollectionSitesAsync(string siteCollectionId)
        {
            Ensure.Argument.NotNull(siteCollectionId, "siteCollectionId");

            // Get list of sites associated with the current Site Collection
            var siteIds = await _db.SiteCollectionMaps
                        .Where(x => x.SiteCollectionId == siteCollectionId)
                        .Select(x => x.SiteId).Distinct().ToListAsync();

            // Get site information for each associated site above
            var sites = _db.Sites.Where(x => siteIds.Contains(x.Id));

            return sites;
        }

        public async Task<ICollection<Site>> GetNonSitesAsync(string collectionId, string clientId)
        {
            Ensure.Argument.NotNull(collectionId);
            Ensure.Argument.NotNullOrEmpty(clientId);

            // Get list of sites already in the collection
            var activeSiteIds = await _db.SiteCollectionMaps
                .Where(x => x.SiteCollectionId == collectionId)
                .Select(x => x.SiteId)
                .Distinct()
                .ToListAsync();

            // Get a list of all sites for the Client that aren't already in the collection
            var availableSites = await _db.Sites
                           .Where(x => !activeSiteIds.Contains(x.Id) && x.ClientId == clientId)
                           .ToListAsync();

            return availableSites;
        }

        public async Task<bool> SitesAddNewAsync(string collectionId, string siteId)
        {
            Ensure.Argument.NotNull(collectionId);
            Ensure.Argument.NotNull(siteId);

            try
            {
                if ((await _db.SiteCollectionMaps.AnyAsync(x => x.SiteCollectionId == collectionId && x.SiteId == siteId)) == false)
                {
                    var newSite = new SiteCollectionMap();
                    newSite.Site = await _db.Sites.FirstOrDefaultAsync(x => x.Id == siteId);
                    newSite.SiteCollection = await _db.SiteCollections.FirstOrDefaultAsync(x => x.Id == collectionId);

                    _db.SiteCollectionMaps.Add(newSite);
                    await _db.SaveChangesAsync();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteSiteFromSiteCollectionAsync(string siteId, string collectionId)
        {
            Ensure.Argument.NotNull(siteId);
            Ensure.Argument.NotNull(collectionId);

            try
            {
                _db.SiteCollectionMaps.RemoveRange(_db.SiteCollectionMaps.Where(x => x.SiteId == siteId && x.SiteCollectionId == collectionId));
                await _db.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateSiteCollectionDetailsAsync(SiteCollection model)
        {
            Ensure.Argument.NotNull(model);

            var tempModel = await _db.SiteCollections.FirstOrDefaultAsync(x => x.Id == model.Id);

            if (tempModel == null)
            {
                return false;
            }

            tempModel.Name = model.Name;

            try
            {
                await _db.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
