using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Logging;
using Angelo.Common.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Angelo.Connect.Logging
{
    public class DbLoggerProvider : ILoggerProvider
    {
        private DbContextOptions<DbLogContext> _db;

        public DbLoggerProvider(DbContextOptions<DbLogContext> db)
        {
            _db = db;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return CreateLogger(categoryName, null);
        }

        public DbLogger CreateLogger(string categoryName, string resourceId)
        {
            return new DbLogger(_db)
            {
                Category = categoryName,
                ResourceId = resourceId,
                IncludeConsole = true
            };
        }


        public void Dispose()
        {
        }
    }
}

