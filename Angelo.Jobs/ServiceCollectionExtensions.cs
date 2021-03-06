using System;
using Microsoft.Extensions.DependencyInjection;
using Angelo.Jobs.Server;

namespace Angelo.Jobs
{
	public static class ServiceCollectionExtensions
	{
		public static void AddJobs(
			this IServiceCollection services,
			Action<JobsOptions> configure)
		{
			services.AddSingleton<IJobsManager, JobsManager>();
			services.AddSingleton<IJobFactory, JobFactory>();
			services.AddSingleton<IProcessingServer, ProcessingServer>();

			// Processors
			services.AddTransient<FireAndForgetJobProcessor>();
			services.AddTransient<DelayedJobProcessor>();
			services.AddTransient<CronJobProcessor>();

			var options = new JobsOptions();
			configure(options);
			options.Extension?.AddServices(services);
			services.AddSingleton(options);
		}
	}
}
