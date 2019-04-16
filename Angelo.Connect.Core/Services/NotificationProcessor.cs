using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

using Angelo.Connect.Logging;
using System.Text;
using Angelo.Connect.Models;
using System.Linq;
using System.Collections.Generic;
using Angelo.Identity.Models;
using MimeKit;
using MimeKit.Utils;
using Microsoft.AspNetCore.Hosting;
using MailKit.Net.Smtp;
using System.Data.SqlClient;
using Dapper;
using Angelo.Connect.Configuration;
using Angelo.Connect.Extensions;
using Angelo.Connect.Security;

namespace Angelo.Connect.Services
{
    public class NotificationProcessor
    {
        private DbLogService _logger;
        private ConnectCoreOptions _options;
        private IHostingEnvironment _env;

        public NotificationProcessor(DbLogService logger,
            ConnectCoreOptions connectCoreOptions,
            IHostingEnvironment env)
        {
            _options = connectCoreOptions;
            _env = env;
            _logger = logger;
            _logger.Category = "Jobs";
            _logger.ResourceId = nameof(NotificationProcessor);
        }

        private string GenerateJobId()
        {
            return Guid.NewGuid().ToString("N");
        }

        // Assign this process to those notifications that need to be sent.
        protected void ResetNotificationsWithProcessingStatus(int maxProcessMinutes)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE cms.Notification");
            sb.AppendLine($"SET Status = '{NotificationStatus.Error}', ProcStopUTC = GETUTCDATE()");
            sb.AppendLine($"WHERE Status = '{NotificationStatus.Processing}'");
            sb.AppendLine($"AND ProcStartUTC < @targetUTC");
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                var targetUTC = DateTime.UtcNow.AddMinutes(-1 * maxProcessMinutes);
                conn.Execute(sb.ToString(), new { targetUTC = targetUTC });
                conn.Close();
            }
        }

        protected void MarkNotificationsForProcessing(string jobId, int maxProcessCount, int maxRetryCount)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE cms.Notification");
            sb.AppendLine($"SET ProcId = @JobId, Status = '{NotificationStatus.Processing}', ProcStartUTC = GETUTCDATE(), RetryCount = RetryCount + 1");
            sb.AppendLine($"WHERE Id in (");
            sb.AppendLine($"  SELECT TOP({maxProcessCount}) note.Id");
            sb.AppendLine($"  FROM cms.Notification note");
            sb.AppendLine($"  WHERE note.ScheduledUTC < @targetUTC");
            sb.AppendLine($"  AND(note.Status = '{NotificationStatus.Scheduled}' OR note.Status = '{NotificationStatus.Error}')");
            sb.AppendLine($"  AND note.RetryCount < {maxRetryCount}");
            sb.AppendLine($"  ORDER BY note.RetryCount)");
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                conn.Execute(sb.ToString(), new { JobId = jobId, targetUTC = DateTime.UtcNow });
                conn.Close();
            }
        }

        // Assign this process to the specified notifications.
        protected void MarkNotificationForProcessing(string jobId, string notificationId, int maxRetryCount)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE cms.Notification");
            sb.AppendLine($"SET ProcId = @JobId, Status = '{NotificationStatus.Processing}', ProcStartUTC = GETUTCDATE(), RetryCount = RetryCount + 1");
            sb.AppendLine($"WHERE Id = @NotificationId");
            sb.AppendLine($"AND(Status = '{NotificationStatus.Scheduled}' OR Status = '{NotificationStatus.Error}')");
            sb.AppendLine($"AND RetryCount < {maxRetryCount}");
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                conn.Execute(sb.ToString(), new { JobId = jobId, NotificationId = notificationId });
                conn.Close();
            }
        }

        // Unassign this process from any unprocessed notifications.
        protected void UnmarkNotifications(string jobId)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE cms.Notification");
            sb.AppendLine($"SET Status = '{NotificationStatus.Error}', ProcStopUTC = GETUTCDATE()");
            sb.AppendLine($"WHERE ProcId = '{jobId}' AND Status = '{NotificationStatus.Processing}'");
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                conn.Execute(sb.ToString());
                conn.Close();
            }
        }

        protected List<Notification> GetNotificationList(string procId)
        {
            List<Notification> list;
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                list = conn.Query<Notification>(
                    "select * from cms.Notification where ProcId = @procId",
                    new { procId = procId }
                    ).ToList();
                conn.Close();
            }
            return list;
        }

        protected void UpdateNotification(Notification note)
        {
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                var sql = @"
                    update cms.Notification 
                    set Status = @Status, ProcStopUTC = @ProcStopUTC 
                    where Id = @Id";
                conn.Open();
                conn.Execute(sql, note);
                conn.Close();
            }
        }

        protected NotificationEmailHeader GetNotificationEmailHeader(string emailHeaderId)
        {
            NotificationEmailHeader emailHeader;
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                emailHeader = conn.QueryFirstOrDefault<NotificationEmailHeader>(
                    "select * from cms.NotificationEmailHeader where Id = @Id",
                    new { Id = emailHeaderId });
                conn.Close();
            }
            return emailHeader;
        }

        protected void InsertNotificationEmailLog(NotificationEmailLog entry)
        {
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                conn.Execute(
                    "INSERT INTO cms.NotificationEmailLog(NotificationId, UserId, EmailAddress, SentUTC)" +
                    "VALUES(@NotificationId, @UserId, @EmailAddress, @SentUTC)",
                    entry);
                conn.Close();
            }
        }

        protected List<NotificationEmailLog> GetNotificationEmailLogs(string notificationId)
        {
            List<NotificationEmailLog> list;
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                list = conn.Query<NotificationEmailLog>(
                    "select * from cms.NotificationEmailLog where NotificationId = @Id",
                    new { Id = notificationId }).ToList();
                conn.Close();
            }
            return list;
        }

        protected void InsertNotificationSmsLog(NotificationSmsLog entry)
        {
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                conn.Execute(
                    "INSERT INTO cms.NotificationSmsLog(NotificationId, UserId, MobileNumber, WirelessProviderId, SentUTC)" +
                    "VALUES(@NotificationId, @UserId, @MobileNumber, @WirelessProviderId, @SentUTC)",
                    entry);
                conn.Close();
            }
        }

        protected List<NotificationSmsLog> GetNotificationSmsLogs(string notificationId)
        {
            List<NotificationSmsLog> list;
            using (var conn = new SqlConnection(_options.ConnectConnectionString))
            {
                conn.Open();
                list = conn.Query<NotificationSmsLog>(
                    "select * from cms.NotificationSmsLog where NotificationId = @Id",
                    new { Id = notificationId }).ToList();
                conn.Close();
            }
            return list;
        }

        protected List<User> GetAssignedEmailUsers(string notificationId)
        {
            List<User> users;
            List<string> userIds;

            List<string> groupIds;
            List<string> groupUserIds;

            using (var connectConn = new SqlConnection(_options.ConnectConnectionString))
            {
                connectConn.Open();
                string sql1 = @"
                    select distinct ugm.UserId
                    from cms.NotificationUserGroup nug
                    inner join cms.UserGroupMembership ugm on nug.UserGroupId = ugm.UserGroupId
                    where ((ugm.AllowEmailMessaging is not null) and (ugm.AllowEmailMessaging = 1))
                    and nug.NotificationId = @notificationId
                ";
                userIds = connectConn.Query<string>(sql1, new { notificationId = notificationId }).ToList();
                connectConn.Close();
            }


            using (var connectConn = new SqlConnection(_options.ConnectConnectionString))
            {
                connectConn.Open();
                string sql1 = @"
                    select distinct nug.UserGroupId
                    from cms.NotificationUserGroup nug
                    where nug.NotificationId = @notificationId
                ";
                groupIds = connectConn.Query<string>(sql1, new { notificationId = notificationId }).ToList();
                connectConn.Close();
            }

            using (var identityConn = new SqlConnection(_options.IdentityConnectionString))
            {
                identityConn.Open();
                string sqlGroups = @"
                     select distinct gm.UserId
                    from auth.GroupMembership gm
                    where gm.GroupId in @groupIds
                ";
                groupUserIds = identityConn.Query<string>(sqlGroups, new { groupIds = groupIds.ToArray() }).ToList();
                identityConn.Close();
            }

            if (groupUserIds.Any())
            {
                userIds.AddRange(groupUserIds);
            }


            using (var identityConn = new SqlConnection(_options.IdentityConnectionString))
            {
                identityConn.Open();
                string sql2 = @"
                    select Id, Email, EmailConfirmed, UserName, PhoneNumber, PhoneNumberConfirmed, WirelessProviderId 
                    from auth.[User] u 
                    where u.Id in @userIds
                ";
                users = identityConn.Query<User>(sql2, new { userIds = userIds.ToArray() }).ToList();
                identityConn.Close();
            }

            return users;
        }

        protected List<User> GetAssignedSmsUsers(string notificationId)
        {
            List<User> users;
            List<string> userIds;

            using (var connectConn = new SqlConnection(_options.ConnectConnectionString))
            {
                connectConn.Open();
                string sql1 = @"
                    select distinct ugm.UserId
                    from cms.NotificationUserGroup nug
                    inner join cms.UserGroupMembership ugm on nug.UserGroupId = ugm.UserGroupId
                    where ((ugm.AllowSmsMessaging is not null) and (ugm.AllowSmsMessaging = 1))
                    and nug.NotificationId = @notificationId
                ";
                userIds = connectConn.Query<string>(sql1, new { notificationId = notificationId }).ToList();
                connectConn.Close();
            }

            using (var identityConn = new SqlConnection(_options.IdentityConnectionString))
            {
                identityConn.Open();
                string sql2 = @"
                    select Id, Email, EmailConfirmed, UserName, PhoneNumber, PhoneNumberConfirmed, WirelessProviderId 
                    from auth.""User"" u 
                    where u.Id in @userIds
                ";
                users = identityConn.Query<User>(sql2, new { userIds = userIds.ToArray() }).ToList();
                identityConn.Close();
            }
            return users;
        }

        protected WirelessProvider GetWirelessProvider(string id)
        {
            WirelessProvider entry;
            using (var connectConn = new SqlConnection(_options.IdentityConnectionString))
            {
                connectConn.Open();
                string sql = @"select * from auth.WirelessProvider where Id = @id";
                entry = connectConn.QueryFirstOrDefault<WirelessProvider>(sql, new { id = id });
                connectConn.Close();
            }
            return entry;
        }

        protected String GetDomainOfNotification(string notificationId)
        {
            String entry;
            using (var connectConn = new SqlConnection(_options.ConnectConnectionString))
            {
                connectConn.Open();
                string sql = @"select d.DomainKey
                    from cms.Notification n
                    inner join cms.SiteDomain d on n.OwnerId = d.SiteId
                    where n.id= @id and d.IsDefault = 1";
                entry = connectConn.QueryFirstOrDefault<String>(sql, new { id = notificationId, ownerLevel = OwnerLevel.Site });
                connectConn.Close();
            }
            return entry;
        }

        protected string BuildUnsubcsribeLink(string notificationId, string userId)
        {
            var model = new NotificationUnsubscribe()
            {
                NotificationId = notificationId,
                UserId = userId
            };
            var eModel = ModelEncryptor.Encrypt(model);
            var domain = GetDomainOfNotification(notificationId);
            if (string.IsNullOrEmpty(domain))
                domain = _options.NotifyMeUnsubscribeDefaultDomain;
            var link = domain + _options.NotifyMeUnsubscribePath + "?eModel=" + eModel;
            return link;
        }

        protected MimeEntity CreateEmailBody(Notification note, User user)
        {
            var builder = new BodyBuilder();
            var sb = new StringBuilder();
            var unsubscribeLink = BuildUnsubcsribeLink(note.Id, user.Id);
            var emailHeader = GetNotificationEmailHeader(note.EmailHeaderId);

            sb.AppendLine("<table style=\"border: solid;\">");
            if (emailHeader != null)
            {
                var imagePath = _env.WebRootPath + emailHeader.Path; //"\\img\\Admin\\EmailHeaders\\EmailHeader2.jpg"
                var image = builder.LinkedResources.Add(imagePath);
                image.ContentId = MimeUtils.GenerateMessageId();
                sb.AppendLine("<tr><td>");
                sb.AppendLine($"<img src=\"cid:{image.ContentId}\">");
                sb.AppendLine("</td></tr>");
            }
            sb.AppendLine("<tr><td><div style=\"min-height:300px;\">");
            sb.AppendLine(note.EmailBody);
            sb.AppendLine("</div></td></tr>");
            sb.AppendLine("<tr><td>");
            sb.AppendLine($"To unsubscribe from this email notification <a href=\"{unsubscribeLink}\">CLICK HERE</a>.");
            sb.AppendLine("</td></tr>");
            sb.AppendLine("</table>");
            builder.HtmlBody = sb.ToString();
            return builder.ToMessageBody();
        }

        protected void SendEmail(Notification note, User user)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("SchoolInSites", "donotreply@schoolinsites.com"));
            msg.To.Add(new MailboxAddress(user.Email, user.Email));
            msg.Subject = note.EmailSubject;
            msg.Body = CreateEmailBody(note, user);

            var host = _options.EmailServerHost; 
            var port = _options.EmailServerPort;

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(host, port, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate("joey", "password");

                client.Send(msg);
                client.Disconnect(true);
            }
        }

    private bool IsValidEmail(string source)
    {
        return new EmailAddressAttribute().IsValid(source);
    }

    protected int ProcessNotificationEmail(Notification note, List<User> assignedUsers)
        {
            int errorCount = 0;
            var sentList = GetNotificationEmailLogs(note.Id);
            var sentUserIds = sentList.Select(log => log.UserId).ToList();
            // Remove the users that have already received this email.
            var sendList = assignedUsers.Where(u => sentUserIds.Contains(u.Id) == false).ToList();
            foreach (User user in sendList)
            {
                try
                {
                    if (string.IsNullOrEmpty(user.Email))
                        _logger.Log("WARNING", $"[NotificationId:{note.Id}, UserId:{user.Id}] No email address defined for user.");
                    else if (user.EmailConfirmed == false)
                        _logger.Log("WARNING", $"[NotificationId:{note.Id}, UserId:{user.Id}, Email:{user.Email}] Email address has not been confirmed.");
                    else if (IsValidEmail(user.Email) == false)
                        _logger.Log("WARNING", $"[NotificationId:{note.Id}, UserId:{user.Id}, Email:{user.Email}] Invalid email address.");
                    else
                    {
                        SendEmail(note, user);
                        InsertNotificationEmailLog(new NotificationEmailLog()
                        {
                            SentUTC = DateTime.UtcNow,
                            NotificationId = note.Id,
                            UserId = user.Id,
                            EmailAddress = user.Email
                        });
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"[NotificationId:{note.Id}, UserId:{user.Id}, Email:{user.Email ?? "(undefined)"}] ";
                    _logger.Log("ERROR", msg + ex.Message);
                    errorCount++;
                }
            }
            return errorCount;
        }

        protected MailboxAddress CreateSmsMailboxAddress(string toNumber, string providerId)
        {
            var provider = GetWirelessProvider(providerId);
            if (provider == null)
                throw new Exception("Unable to find WirelessProviderId:" + providerId ?? "(undefined)");

            var cNum = new String(toNumber.Where(Char.IsDigit).ToArray());
            if (cNum.Length != 10)
                throw new Exception($"Mobile numbers used in SMS email addresses must be 10 digits ({toNumber ?? ""}).");

            var address = cNum + "@" + provider.SmsDomain;
            return new MailboxAddress("", address);
        }

        protected void SendSms(string toNumber, string providerId, string textMessage)
        {
            var bodyText = (textMessage.Length > 160) ? textMessage.Substring(0, 160) : textMessage;

            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress("SchoolInSites", "donotreply@schoolinsites.com"));
            msg.To.Add(CreateSmsMailboxAddress(toNumber, providerId));
            msg.Subject = "";
            msg.Body = new TextPart("plain") { Text = bodyText };

            var host = _options.EmailServerHost;
            var port = _options.EmailServerPort;

            using (var client = new SmtpClient())
            {
                client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                client.Connect(host, port, false);

                // Note: since we don't have an OAuth2 token, disable
                // the XOAUTH2 authentication mechanism.
                client.AuthenticationMechanisms.Remove("XOAUTH2");

                // Note: only needed if the SMTP server requires authentication
                //client.Authenticate("joey", "password");

                client.Send(msg);
                client.Disconnect(true);
            }
        }

        private bool IsPhoneNumberValid(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
                return false;
            if (phoneNumber.Where(Char.IsDigit).Count() == 10)
                return true;
            else
                return false;
        }

        protected int ProcessNotificationSms(Notification note, List<User> assignedUsers)
        {
            int errorCount = 0;
            var sentList = GetNotificationSmsLogs(note.Id);
            var sentUserIds = sentList.Select(log => log.UserId).ToList();
            // Remove the users that have already received this sms.
            var sendList = assignedUsers.Where(u => sentUserIds.Contains(u.Id) == false).ToList();
            foreach (User user in sendList)
            {
                try
                {
                    if (string.IsNullOrEmpty(user.PhoneNumber))
                        _logger.Log("WARNING", $"[NotificationId:{note.Id}, UserId:{user.Id}] No phone number defined for user.");
                    else if (user.PhoneNumberConfirmed == false)
                        _logger.Log("WARNING", $"[NotificationId:{note.Id}, UserId:{user.Id}, PhoneNumber:{user.PhoneNumber}] Phone number has not been confirmed.");
                    else if (IsPhoneNumberValid(user.PhoneNumber) == false)
                        _logger.Log("WARNING", $"[NotificationId:{note.Id}, UserId:{user.Id}, PhoneNumber:{user.PhoneNumber}] Invalid phone number.");
                    else
                    {
                        SendSms(user.PhoneNumber, user.WirelessProviderId, note.SmsMessage);
                        InsertNotificationSmsLog(new NotificationSmsLog()
                        {
                            SentUTC = DateTime.UtcNow,
                            NotificationId = note.Id,
                            UserId = user.Id,
                            MobileNumber = user.PhoneNumber,
                            WirelessProviderId = user.WirelessProviderId
                        });
                    }
                }
                catch (Exception ex)
                {
                    var msg = $"[NotificationId:{note.Id}, Userd:{user.Id}, Cell:{user.PhoneNumber ?? "(undefined)"}] ";
                    _logger.Log("ERROR", msg + ex.Message);
                    errorCount++;
                }
            }
            return errorCount;
        }

        protected void ProcessNotifications(string jobId)
        {
            var list = GetNotificationList(jobId);
            foreach (Notification note in list)
            {
                try
                {
                    var errorCount = 0;
                    var emailUsers = GetAssignedEmailUsers(note.Id);
                    if (note.SendEmail) errorCount += ProcessNotificationEmail(note, emailUsers);
                    var smsUsers = GetAssignedSmsUsers(note.Id);
                    if (note.SendSms) errorCount += ProcessNotificationSms(note, smsUsers);
                    if (errorCount == 0)
                        note.Status = NotificationStatus.Sent;
                    else
                        note.Status = NotificationStatus.Error;
                    note.ProcStopUTC = DateTime.UtcNow;
                    UpdateNotification(note);
                }
                catch (Exception ex)
                {
                    var msg = $"[NotificationId:{note.Id}] ";
                    _logger.Log("ERROR", ex.Message);
                    note.Status = NotificationStatus.Error;
                    note.ProcStopUTC = DateTime.UtcNow;
                    UpdateNotification(note);
                }
            }
        }

        // Execute all of the notifications that are scheduled to be sent now.
        public void ExecuteBatch()
        {
            string jobId = GenerateJobId();
            DateTime jobStartDT = DateTime.Now;
            _logger.Log("START", $"Starting Batch Job @ {jobStartDT.ToString("HH:mm:ss.ffff")} ProcId: {jobId}");
            try
            {
                ResetNotificationsWithProcessingStatus(maxProcessMinutes: 10);
                MarkNotificationsForProcessing(jobId, maxProcessCount: 100, maxRetryCount: 1000);
                try
                {
                    ProcessNotifications(jobId);
                }
                finally
                {
                    UnmarkNotifications(jobId);
                }
            }
            catch (Exception ex)
            {
                _logger.Log("ERROR", ex.Message);
            }
            TimeSpan duration = DateTime.Now - jobStartDT;
            _logger.Log("STOP", $"Finished Batch Job in {duration.TotalSeconds}.{duration.Milliseconds} seconds  ProcId: {jobId}");
        }

        // Execute a single notification to be sent now.
        public void ExecuteNotification(string notificationId)
        {
            string jobId = GenerateJobId();
            DateTime jobStartDT = DateTime.Now;
            _logger.Log("START", $"Starting Single Notification Job @ {jobStartDT.ToString("HH:mm:ss.ffff")} ProcId: {jobId}");
            try
            {
                MarkNotificationForProcessing(jobId, notificationId, maxRetryCount: 1000);
                try
                {
                    ProcessNotifications(jobId);
                }
                finally
                {
                    UnmarkNotifications(jobId);
                }
            }
            catch (Exception ex)
            {
                _logger.Log("ERROR", ex.Message);
            }
            TimeSpan duration = DateTime.Now - jobStartDT;
            _logger.Log("STOP", $"Finished Single Notification Job in {duration.TotalSeconds}.{duration.Milliseconds} seconds  ProcId: {jobId}");
        }


    }
}
