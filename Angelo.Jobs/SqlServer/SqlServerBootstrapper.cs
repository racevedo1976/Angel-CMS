using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Angelo.Jobs.Server;

namespace Angelo.Jobs
{
	public class SqlServerBootstrapper : BootstrapperBase
	{
		private IApplicationLifetime _appLifetime;

		public SqlServerBootstrapper(
			JobsOptions options,
			IStorage storage,
			IProcessingServer server,
			IApplicationLifetime appLifetime)
			: base(options, storage, server)
		{
			_appLifetime = appLifetime;
		}

		public override Task BootstrapCore()
		{
			_appLifetime.ApplicationStopping.Register(() => Server.Dispose());
			return Task.FromResult(0);
		}
	}
}
