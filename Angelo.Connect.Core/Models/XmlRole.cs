using Angelo.Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Angelo.Connect.Models
{
    public class XmlRole : IXmlImport
    {
        public string Id { get; set; }
        public string ConcurrencyStamp { get; set; }
        public bool IsDefault { get; set; }
        public bool IsLocked { get; set; }
        public string Name { get; set; }
        public string PoolId { get; set; }

        public List<XmlClaim> Claims { get; set; }

        private const string ROLE_TAG = "ROLE";
        private const string CLAIMS_TAG = "CLAIMS";
        private const string CLAIM_TAG = "CLAIM";

        public XmlRole()
        {
            Claims = new List<XmlClaim>();
        }

        public async Task ImportXmlAsync(XmlReader reader)
        {
            if (!reader.IsBeginTag(ROLE_TAG))
                await reader.ReadBeginTagAsync(ROLE_TAG);
            Name = reader.GetRequiredAttribute("name");
            bool bvalue;
            if (bool.TryParse(reader.GetAttribute("is_default"), out bvalue))
                IsDefault = bvalue;
            if (bool.TryParse(reader.GetAttribute("is_locked"), out bvalue))
                IsLocked = bvalue;
            while (await reader.ReadNextTagAsync(ROLE_TAG))
            {
                if (reader.IsBeginTag(CLAIMS_TAG))
                {
                    while (await reader.ReadNextTagAsync(CLAIMS_TAG))
                    {
                        if (reader.IsBeginTag(CLAIM_TAG))
                        {
                            var claim = new XmlClaim();
                            await claim.ImportXmlAsync(reader);
                            Claims.Add(claim);
                        }
                    }
                }
            }
        }

    }
}
