using System;
using Microsoft.Extensions.DependencyInjection;

namespace Angelo.Jobs
{
	public class JobFactory : IJobFactory
	{
		private IServiceProvider _provider;

		public JobFactory(IServiceProvider provider)
		{
			_provider = provider;
		}

		public object Create(Type type)
		{
			return _provider.GetRequiredService(type);
		}
	}
}
