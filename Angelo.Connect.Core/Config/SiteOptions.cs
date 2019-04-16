using System;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Http;

using Angelo.Connect.Models;

namespace Angelo.Connect
{
    public class SiteOptions : Collection<SiteOptionItem>
    {
        public string Get(string siteKey, string optionKey)
        {
            return Items.FirstOrDefault(x =>
                x.SiteKey == siteKey &&
                x.Key == optionKey
            )?.Value;
        }     
    }

    public class SiteOptionItem
    {
        public string SiteKey { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
