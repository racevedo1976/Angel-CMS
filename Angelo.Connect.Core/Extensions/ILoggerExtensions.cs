using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Angelo.Connect.Models;
using Angelo.Connect.Logging;
using Angelo.Connect.Abstractions;
using Microsoft.Extensions.Logging;

namespace Angelo.Connect.Extensions
{
    public static class ILoggerExtensions
    {
        public const string CreateEvent = "Create";

        public const string ReadEvent = "Read";

        public const string WriteEvent = "Write";

        public const string DeleteEvent = "Delete";

        public const string DownloadEvent = "Download";

        public const string ShareEvent = "Share";

        public static async Task<IEnumerable<DbLogEvent>> GetDocumentEventsAsync(this DbLogService logger, string documentId)
        {
            return await logger.GetEventsByResource(documentId);
        }

        public static async Task LogEventShareAsync(this DbLoggerProvider logger, FileDocument document, string ownerId, string userId)
        {
            await logger.LogDocumentShareMessageAsync(ShareEvent, document, ownerId, userId);
        }


        public static async Task LogEventCreateAsync(this DbLoggerProvider logger, FileDocument document, string ownerId)
        {
            await logger.LogDocumentMessageAsync(CreateEvent, document, ownerId);
        }

        public static async Task LogEventDeleteAsync(this DbLoggerProvider logger, FileDocument document, string ownerId)
        {
            await logger.LogDocumentMessageAsync(DeleteEvent, document, ownerId);
        }
        public static async Task LogEventReadAsync(this DbLoggerProvider logger, FileDocument document, string ownerId)
        {
            await logger.LogDocumentMessageAsync(ReadEvent, document, ownerId);
        }

        public static async Task LogEventWriteAsync(this DbLoggerProvider logger, FileDocument document, string ownerId)
        {
            await logger.LogDocumentMessageAsync(WriteEvent, document, ownerId);
        }

        public static async Task LogDocumentMessageAsync(this DbLoggerProvider logger, string eventName, IDocument document, string userId)
        {
            var log = logger.CreateDocumentLogger(document);
            var message = $"'{eventName}' performed on document '{document.Title}'({document.DocumentId}) by user '{userId}'.";

            await Task.Run(() => log.Log(LogLevel.Information, new EventId(1, eventName), message, null, null));
        }

        public static async Task LogDocumentShareMessageAsync(this DbLoggerProvider logger, string eventName, IDocument document, string ownerId, string userId)
        {
            var log = logger.CreateDocumentLogger(document);
            var message = $"Document '{document.Title}'({document.DocumentId}) shared with user '{userId}'.";

            await Task.Run(() => log.Log(LogLevel.Information, new EventId(1, eventName), message, null, null));
        }

        public static DbLogger CreateDocumentLogger(this DbLoggerProvider logger, IDocument document)
        {
            var category = document.GetType().Name;
            var resource = document.DocumentId;

            return logger.CreateLogger(category, resource);
        }

        public static DbLogger CreateDocumentLogger(this DbLoggerProvider logger, string documentId)
        {
            var category = typeof(FileDocument).Name;
            var resource = documentId;

            return logger.CreateLogger(category, resource);
        }

        public static async Task<LogSummary> GetLogSummaryAsync(this DbLogService logger, FileDocument document)
        {
            Ensure.NotNull(logger, $"{nameof(logger)} cannot be null.");
            Ensure.NotNull(document, $"{nameof(document)} cannot be null.");

            var logs = (await logger.GetDocumentEventsAsync(document.DocumentId)) ?? new DbLogEvent[0];
            return new LogSummary()
            {
                Created = DateTime.MinValue,
                Deleted = null,
                DocumentLog = null,
                DocumentLogId = null,
                LastRead = DateTime.MinValue,
                LastReadOther = DateTime.MinValue,
                LastReadUserId = null,
                LastWrite = DateTime.MinValue,
                ReadCount = 0,
                ReadCountOther = 0,
                WriteCount = 0
            };
        }
        public static LogSummary ToSummary(this IEnumerable<DbLogEvent> events, string userId)
        {
            events = events.OrderBy(x => x.Created).Reverse();

            var created = events.SingleOrDefault(x => x.EventName == CreateEvent)?.Created;
            var deleted = events.SingleOrDefault(x => x.EventName == DeleteEvent)?.Created;

            var myReads = events.Where(x => x.EventName == ReadEvent && x.UserId == userId);
            var otherReads = events.Where(x => x.EventName == ReadEvent && x.UserId != userId);
            var myDownloads = events.Where(x => x.EventName == DownloadEvent && x.UserId == userId);
            var otherDownloads = events.Where(x => x.EventName == DownloadEvent && x.UserId != userId);
            var myWrites = events.Where(x => x.EventName == WriteEvent && x.UserId == userId);
            var otherWrites = events.Where(x => x.EventName == WriteEvent && x.UserId != userId);

            var lastRead = myReads.FirstOrDefault()?.Created;
            var lastReadOther = otherReads.FirstOrDefault()?.Created;
            var lastReadUserId = otherReads.FirstOrDefault()?.UserId;
            var lastDownload = myDownloads.FirstOrDefault()?.Created;
            var lastDownloadOther = otherDownloads.FirstOrDefault()?.Created;
            var lastDownloadUserId = otherDownloads.FirstOrDefault()?.UserId;
            var lastWrite = myWrites.FirstOrDefault()?.Created;
            var lastWriteOther = otherWrites.FirstOrDefault()?.Created;
            var lastWriteUserId = otherWrites.FirstOrDefault()?.UserId;
            var readCount = myReads.Count(); var readCountOther = otherReads.Count();
            var downloadCount = myDownloads.Count(); var downloadCountOther = otherDownloads.Count();
            var writeCount = myWrites.Count(); var writeCountOther = otherWrites.Count();

            return new LogSummary()
            {
                Created = created,
                Deleted = deleted,
                LastRead = lastRead,
                LastReadOther = lastReadOther,
                LastReadUserId = lastReadUserId,
                LastWrite = lastWrite,
                LastWriteOther = lastWriteOther,
                LastWriteUserId = lastWriteUserId,
                ReadCount = readCount,
                ReadCountOther = readCountOther,
                DownloadCount = downloadCount,
                DownloadCountOther = downloadCountOther,
                WriteCount = writeCount,
                WriteCountOther = writeCountOther
            };
        }
    }
}