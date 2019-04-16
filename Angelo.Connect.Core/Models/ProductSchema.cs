using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Rendering;

namespace Angelo.Connect.Models
{
    public class ProductSchema
    {
        public ProductSchema()
        {
            Features = new List<Feature>();
            SiteTemplates = new List<SiteTemplate>();
        }

        public string Id { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string Version { get; set; }
        public int BaseSiteMB { get; set; }
        public int IncClientMB { get; set; }
        public List<Feature> Features { get; set; }
        public List<SiteTemplate> SiteTemplates { get; set; }

        protected void MergeFeatures(IEnumerable<Feature> addonFeatures)
        {
            foreach (var addonFeature in addonFeatures)
            {
                var existingFeature = Features.Where(x => x.Id == addonFeature.Id).FirstOrDefault();
                if (existingFeature != null)
                    Features.Remove(existingFeature);
                Features.Add(addonFeature);
            }
        }


        protected void MergeSiteTemplates(IEnumerable<SiteTemplate> addonSiteTemplates)
        {
            foreach (var addonSiteTemplate in addonSiteTemplates)
                if (SiteTemplates.Where(x => x.Id == addonSiteTemplate.Id).Any() == false)
                    SiteTemplates.Add(addonSiteTemplate);
        }

        public void MergeAddon(ProductSchema addonSchema)
        {
            IncClientMB = IncClientMB + addonSchema.IncClientMB;
            MergeFeatures(addonSchema.Features);
            MergeSiteTemplates(addonSchema.SiteTemplates);
        }
    }
}