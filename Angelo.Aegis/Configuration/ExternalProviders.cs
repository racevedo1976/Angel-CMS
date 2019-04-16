using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Angelo.Aegis.Configuration
{
    public class ExternalProviders
    {
        public ExternalProviderOptions Google { get; set; }
        public ExternalProviderOptions Twitter { get; set; }
        public ExternalProviderOptions Facebook { get; set; }
        public ExternalProviderOptions Microsoft { get; set; }
    }
}
