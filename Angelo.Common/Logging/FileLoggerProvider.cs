using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

using Microsoft.Extensions.Logging;

namespace Angelo.Common.Logging
{
    public class FileLoggerProvider : ILoggerProvider
    {
        private Options _options;

        public class Options
        {
            public string FilePath { get; set; }
            public bool IncludeConsole { get; set; } = true;
            public IEnumerable<string> Filters { get; set; }
        }

        public static FileLoggerProvider Create(Action<Options> configure)
        {
            var options = new Options();
            configure(options);

            return new FileLoggerProvider(options);
        }

        public FileLoggerProvider(Options options)
        {
            _options = options;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new FileLogger() {
                LogFile = _options.FilePath,
                IncludeConsole = _options.IncludeConsole,
                Filters = _options.Filters
            };
        }

        public void Dispose()
        {
        }

        private class FileLogger : ILogger
        {
            private static object _locker = new Object();


            public string LogFile { get; set; } = @"\application.log";
            public bool IncludeConsole { get; set; } = true;
            public IEnumerable<string> Filters { get; set; }

            public IDisposable BeginScope<TState>(TState state)
            {
                return null;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return true;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
                lock (_locker)
                {
                    using (var stream = new FileStream(LogFile, FileMode.Append, FileAccess.Write, FileShare.None, bufferSize: 4096))
                    {
                        using (var writer = new StreamWriter(stream))
                        {
                            var details = formatter(state, exception);

                            if(IncludeConsole)
                                Console.WriteLine(details);

                            if (Filters == null || (state != null && Filters.Contains(state.ToString())))
                            {
                                writer.WriteLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "]");
                                writer.WriteLine(details);
                                writer.WriteLine();
                            }

                        }
                    }
                }
            }
        }
    }
}

