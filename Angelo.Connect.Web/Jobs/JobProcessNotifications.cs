using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using Angelo.Jobs;
using Angelo.Connect.Logging;
using Angelo.Connect.Data;
using System.Text;
using Angelo.Connect.Models;
using System.Linq;
using Angelo.Connect.Services;
using System.Collections.Generic;
using Angelo.Identity.Models;
using MimeKit;
using MimeKit.Utils;
using Microsoft.AspNetCore.Hosting;
using MailKit.Net.Smtp;
using Angelo.Connect.Configuration;

namespace Angelo.Connect.Web.Jobs
{
    public class JobProcessNotifications : IJob
    {
        private NotificationProcessor _notificationProcessor;

        public JobProcessNotifications(NotificationProcessor notificationProcessor)
        {
            _notificationProcessor = notificationProcessor;
        }

        public async Task ExecuteAsync()
        {
            _notificationProcessor.ExecuteBatch();
        }

    }
}
