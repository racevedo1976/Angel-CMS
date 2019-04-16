using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Connect.Configuration
{
    public class ConnectCoreOptions
    {
        public string FileSystemRoot { get; set; }
        public string SearchIndexRoot { get; set; }
        public string TemplateExportPath { get; set; }
        public IEnumerable<CultureInfo> SupportedCultures { get; set; }
        public CultureInfo DefaultCulture { get; set; }
        public string AegisAuthority { get; set; }
        public string ConnectConnectionString { get; set; }
        public string IdentityConnectionString { get; set; }
        public string NotifyMeUnsubscribeDefaultDomain { get; set; }
        public string NotifyMeUnsubscribePath { get; set; }
        public string EmailServerHost { get; set; }
        public int EmailServerPort { get; set; }
        public string TemplatesPath { get; set; }
        public bool UseHttpsForAbsoluteUris { get; set; }
    }
}
