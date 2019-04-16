using Angelo.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Angelo.Connect.Models
{
    public class XmlClaim : IXmlImport
    {
        public string Type { get; set; }
        public string Value { get; set; }

        public async Task ImportXmlAsync(XmlReader reader)
        {
            if (!reader.IsBeginTag("CLAIM"))
                await reader.ReadBeginTagAsync("CLAIM");
            Type = reader.GetRequiredAttribute("type");
            Value = reader.GetRequiredAttribute("value");
        }

    }
}
