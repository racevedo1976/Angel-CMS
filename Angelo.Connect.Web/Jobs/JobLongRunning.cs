using System;
using System.Threading.Tasks;

using Angelo.Jobs;
using Angelo.Connect.Logging;

namespace Angelo.Connect.Web.Jobs
{
    public class JobLongRunning : IJob
    {
        private DbLogService _logger;

        public JobLongRunning(DbLogService logger)
        {
            _logger = logger;

            _logger.Category = "Jobs";
            _logger.ResourceId = "JobLongRunning";
        }

        public async Task ExecuteAsync()
        {
            var start = DateTime.Now;

            _logger.Log($"Starting {nameof(JobLongRunning)} @ {start.ToString("HH:mm:ss.ffff")}");

            // job should last between 10 to 30 seconds
            await Task.Delay(10000);
            await Task.Delay((int)Math.Floor(new Random().NextDouble() * 20000));

            var duration = DateTime.Now - start;

            _logger.Log($"Finished {nameof(JobLongRunning)} in {duration.Seconds}.{duration.Milliseconds} seconds");
        }
    }
}
