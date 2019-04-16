using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Angelo.Jobs;
using Angelo.Identity;
using Angelo.Connect.Logging;
using Angelo.Connect.Data;
using Angelo.Common.Extensions;

namespace Angelo.Connect.Web.Jobs
{
    public class JobMigrateConnectDb : IJob
    {
        private DbLogService _logger;
        private ConnectDbContext _connectDb;

        private string _jobName = "ConnectDbContext Migration";
        
        public JobMigrateConnectDb(DbLogService logger, ConnectDbContext connectDb)
        {
            _connectDb = connectDb;
            _logger = logger;

            _logger.Category = "Jobs";
            _logger.ResourceId = _jobName;
        }

        public async Task ExecuteAsync()
        {
            var start = DateTime.Now;

            _logger.Log($"Starting {_jobName}  @ {start.ToString("HH:mm:ss.ffff")}");


            _connectDb.EnsureMigrated();


            var duration = DateTime.Now - start;

            _logger.Log($"Finished {_jobName} in {duration.Seconds}.{duration.Milliseconds} seconds");
        }
    }
}
