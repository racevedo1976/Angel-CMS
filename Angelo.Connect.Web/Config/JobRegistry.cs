using Angelo.Jobs;
using Angelo.Connect.Web.Jobs;
using System;

namespace Angelo.Connect.Web
{
    public class JobRegistry : CronJobRegistry
    {
        public JobRegistry()
        {
            //RegisterJob<JobCountUsers>(nameof(JobCountUsers), Cron.Minutely());
            //RegisterJob<JobLongRunning>(nameof(JobLongRunning), "*/2 * * * *");
            RegisterJob<JobProcessNotifications>(nameof(JobProcessNotifications), "*/1 * * * *");

            // ldap bulk import job
            RegisterJob<JobImportLdapUsers>(nameof(JobImportLdapUsers), Cron.Daily());
            
        }
    }
}
