using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Angelo.Connect.Logging
{
    public class DbLogService
    {
        private string _defaultCategory = "DbLogService Events";
        private DbLogContext _dbLogContext;
        private DbLogger _logger;

        public string Category
        {
            get { return _logger.Category; }
            set { _logger.Category = value; }
        } 

        public string ResourceId
        {
            get { return _logger.ResourceId; }
            set { _logger.ResourceId = value; }
        }

        public DbLogService(DbLogContext dbLogContext, DbLoggerProvider logProvider)
        {
            _dbLogContext = dbLogContext;
            _logger = logProvider.CreateLogger(_defaultCategory, null);
        }

        public DbLogService SetCategory(string category)
        {
            this.Category = category;
            return this;
        }

        public DbLogService SetCategory(string category, string resource)
        {
            this.Category = category;
            this.ResourceId = resource;
            return this;
        }

        public void Log(string message)
        {
            _logger.Log(LogLevel.Information, new EventId(0), message, null, null);
        }

        public void Log(string eventName, string message)
        {
            _logger.Log(LogLevel.Information, new EventId(1, eventName), message, null, null);
        }

        public void Log(LogLevel level, string message)
        {
            _logger.Log(level, new EventId(0, "NA"), message, null, null);
        }

        public void Log(LogLevel level, string eventName, string message)
        {
            _logger.Log(level, new EventId(1, eventName), message, null, null);
        }

        public void Log(LogLevel level, string eventName, string message, string userId)
        {
            _logger.Log(level, new EventId(1, eventName), message, userId);
        }

        public void LogTrace(string message)
        {
            _logger.Log(LogLevel.Trace, new EventId(0, "NA"), message, null, null);
        }

        public void LogTrace(string eventName, string message)
        {
            _logger.Log(LogLevel.Trace, new EventId(1, eventName), message, null, null);
        }

        public void LogWarning(string message)
        {
            _logger.Log(LogLevel.Warning, new EventId(0, "NA"), message, null, null);
        }

        public void LogWarning(string eventName, string message)
        {
            _logger.Log(LogLevel.Warning, new EventId(1, eventName), message, null, null);
        }

        public void LogError(string errorMessage)
        {
            _logger.Log(LogLevel.Error, new EventId(0, "NA"), errorMessage, null, null);
        }

        public void LogError(Exception exception)
        {
            _logger.Log(LogLevel.Error, new EventId(exception.HResult, exception.Message), exception.StackTrace, null, null);
        }

        public void LogError(string eventName, string errorMessage)
        {
            _logger.Log(LogLevel.Error, new EventId(1, eventName), errorMessage, null, null);
        }
        
        public void LogError(string eventName, Exception exception)
        {
            _logger.Log(LogLevel.Error, new EventId(1, eventName), exception.StackTrace, null, null);
        }

        public async Task LogAsync(string message)
        {
            await Task.Run(() => Log(message));
        }

        public async Task LogAsync(string eventName, string message)
        {
            await Task.Run(() => Log(eventName, message));
        }

        public async Task LogAsync(LogLevel level, string message)
        {
            await Task.Run(() => Log(level, message));
        }

        public async Task LogAsync(LogLevel level, string eventName, string message)
        {
            await Task.Run(() => Log(level, eventName, message));
        }

        public async Task LogAsync(LogLevel level, string eventName, string message, string userId)
        {
            await Task.Run(() => Log(level, eventName, message, userId));
        }

        public async Task LogWarningAsync(string message)
        {
            await Task.Run(() => LogWarning(message));
        }

        public async Task LogWarningAsync(string eventName, string message)
        {
            await Task.Run(() => LogWarning(eventName, message));
        }

        public async Task LogErrorAsync(string errorMessage)
        {
            await Task.Run(() => LogError(errorMessage));
        }

        public async Task LogErrorAsync(Exception exception)
        {
            await Task.Run(() => LogError(exception));
        }

        public async Task LogErrorAsync(string eventName, string errorMessage)
        {
            await Task.Run(() => LogError(eventName, errorMessage));
        }

        public async Task LogErrorAsync(string eventName, Exception exception)
        {
            await Task.Run(() => LogError(eventName, exception));
        }

        public async Task<IEnumerable<DbLogEvent>> GetEventsByResource(string resourceId)
        {
            return await _dbLogContext.Events.Where(x => x.ResourceId == resourceId).ToListAsync();
        }

        public async Task<IEnumerable<DbLogEvent>> GetEventsByCategory(string category)
        {
            return await _dbLogContext.Events.Where(x => x.Category == category).ToListAsync();
        }

        public async Task<IEnumerable<DbLogEvent>> GetEventsByCategory(string category, DateTime startDate, DateTime endDate)
        {
            return await _dbLogContext.Events
                .Where(x => x.Category == category)
                .OrderByDescending(x => x.Id)
                .Where(x => x.Created >= startDate && x.Created <= endDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<DbLogEvent>> GetEventsByCategory(string category, int take, int? skip = null)
        {
            return await _dbLogContext.Events
                .Where(x => x.Category == category)
                .OrderByDescending(x => x.Id)
                .Skip(skip.HasValue ? skip.Value : 0)
                .Take(take)
                .ToListAsync();
        }
     
    }
}
