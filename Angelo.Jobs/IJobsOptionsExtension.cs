using Microsoft.Extensions.DependencyInjection;

namespace Angelo.Jobs
{
	public interface IJobsOptionsExtension
	{
		void AddServices(IServiceCollection services);
	}
}
