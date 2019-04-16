using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Angelo.Jobs;
using Angelo.Identity;
using Angelo.Connect.Logging;
using Angelo.Identity.Models;


namespace Angelo.Connect.Web.Jobs
{
    public class JobCountUsers : IJob
    {
        private SecurityPoolManager _poolManager;
        private DbLogService _logger;

        public JobCountUsers(SecurityPoolManager poolManager, DbLogService logger)
        {
            _poolManager = poolManager;
            _logger = logger;

            _logger.Category = "Jobs";
            _logger.ResourceId = "JobCountUsers";
        }

        public async Task ExecuteAsync()
        {
            var message = "Excuted " + this.GetType().Name;
            try
            {
                var users = await _poolManager.GetUsersAsync("global-pool");
                var count = users.ToList().Count.ToString();
                message += $": There are {count} user(s).";
            }
            catch (Exception ex)
            {
                message += $": {ex.Message}";
            }
            _logger.Log(message);
        }
    }
}
