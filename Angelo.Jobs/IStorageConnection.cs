using System;
using System.Threading.Tasks;
using Angelo.Jobs.Models;
using Angelo.Jobs.Server;

namespace Angelo.Jobs
{
	public interface IStorageConnection : IDisposable
	{
		Task StoreJobAsync(DelayedJob job);

		Task StoreJobAsync(CronJob job);

		Task UpdateCronJobAsync(CronJob job);

		Task<IFetchedJob> FetchNextJobAsync();

		Task<IFetchedJob> FetchNextDelayedJobAsync(DateTime from, DateTime to);

		Task<CronJob[]> GetCronJobsAsync();

		Task<CronJob> GetCronJobByNameAsync(string name);

		Task RemoveCronJobAsync(string name);
	}

	public static class JobStorageConnectionExtensions
	{
		public static Task<IFetchedJob> FetchNextDelayedJob(this IStorageConnection @this, DateTime to)
			=> @this.FetchNextDelayedJobAsync(DateTime.MinValue, to);

		public static Task<IFetchedJob> FetchNextDelayedJob(this IStorageConnection @this)
			=> @this.FetchNextDelayedJobAsync(DateTime.MinValue, DateTime.UtcNow);
	}
}
