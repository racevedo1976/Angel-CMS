using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Angelo.Aegis.Configuration
{
    public abstract class OptionsFactory<TOptions> : IOptions<TOptions> where TOptions : class, new()
    {
        private IEnumerable<IConfigureOptions<TOptions>> _configurations;

        public TOptions Value
        {
            get {
                var options = Create();

                // run registered configurations
                foreach (var action in _configurations)
                {
                    action.Configure(options);
                }

                // run factory configuration
                Configure(options);

                return options;
            }
        }

        public OptionsFactory(IServiceProvider serviceProvider)
        {
            _configurations = serviceProvider.GetServices<IConfigureOptions<TOptions>>();
        }

        public abstract TOptions Create();

        public abstract void Configure(TOptions options);
    }
}
