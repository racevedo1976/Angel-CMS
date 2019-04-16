using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Logging
{
    public class DbLogger : ILogger
    {
        private static object _locker = new Object();
        //private DbLogContext _dbLogContext;
        private DbContextOptions<DbLogContext> _db;

        public string Category { get; set; }
        public string ResourceId { get; set; }

        public bool IncludeConsole { get; set; } = true;

        public DbLogger(DbContextOptions<DbLogContext> db)
        {
            if (db == null)
                throw new ArgumentNullException(nameof(db));

            _db = db;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return new LogDisposer(new List<object>{});
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var details = state.ToString();

            if (formatter != null)
                details = formatter(state, exception);

            else if (exception != null)
                details = exception.Message;

            Log(logLevel, eventId, details, userId: null);
        }

        public void Log(LogLevel logLevel, EventId eventId, string details, string userId)
        {
            var dbLogEvent = new DbLogEvent
            {
                Category = Category,
                ResourceId = ResourceId,
                LogLevel = logLevel,
                Created = DateTime.Now,
                Message = details,
                EventCode = eventId.Id,
                EventName = eventId.Name,
                UserId = userId
            };

            using (var db = new DbLogContext(_db))
            {
                db.Events.Add(dbLogEvent);
                db.SaveChanges();
            }

            if (IncludeConsole)
                Console.WriteLine(details);
        }

        public class LogDisposer : IDisposable
        {
            private IList<object> _trashItems;

            public LogDisposer(IList<object> trashItems)
            {
                _trashItems = trashItems;
            }

            public void Dispose()
            {
                for (var i = 0; i < _trashItems.Count(); i++)
                {

                    if (_trashItems[i] is IDisposable)
                        (_trashItems[i] as IDisposable).Dispose();

                    else
                        _trashItems[i] = null;
                }

                System.GC.Collect();
            }
        }
    }

}

