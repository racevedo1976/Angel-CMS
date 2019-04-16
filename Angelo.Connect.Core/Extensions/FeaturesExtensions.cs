using Angelo.Connect.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Extensions
{
    public static class FeaturesExtensions
    {
        public static Feature GetFeature(IEnumerable<Feature> features, string id)
        {
            foreach (var item in features)
                if (item.Id.Equals(id, StringComparison.OrdinalIgnoreCase) && item.GetSettingValue<bool>("enabled"))
                    return item;
            return null;
        }

        public static Feature Get(this IEnumerable<Feature> features, string featureId)
        {
            foreach (var item in features)
                if (item.Id.Equals(featureId, StringComparison.OrdinalIgnoreCase))
                    return item;
            return null;
        }

        public static bool HasFeature(this IEnumerable<Feature> features, string id)
        {
            return (GetFeature(features, id) != null);
        }

        public static TObject GetSettings<TObject>(this IEnumerable<Feature> features, string id)
        {
            var feature = GetFeature(features, id);
            if (feature == null)
                throw new Exception($"Unable to find Feature (Id={id})");
            else
                return feature.Settings.ToObject<TObject>();
        }

        public static TObject GetSettingsOrDefault<TObject>(this IEnumerable<Feature> features, string id, TObject defaultSettings)
        {
            try
            {
                var feature = GetFeature(features, id);
                if (feature != null)
                    return feature.Settings.ToObject<TObject>();
            }
            catch
            {
                // ignore the error and return the default value.
            }
            return defaultSettings;
        }

        public static IEnumerable<Feature> GetFeaturesOfType(this IEnumerable<Feature> features, string type)
        {
            var list = new List<Feature>();
            foreach (var item in features)
                if (item.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                    list.Add(item);
            return list;
        }

        public static T GetSettingValue<T>(this Feature feature, string setting)
        {
            return feature.Settings.Value<T>(setting);
            
        }


    }
}



