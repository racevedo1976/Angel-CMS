using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Angelo.Jobs.Models;
using Angelo.Jobs.Server;

namespace Angelo.Jobs
{
	public class SqlServerStorageConnection : IStorageConnection
	{
		internal SqlServerStorage _storage;
        internal string _schema;

		public SqlServerStorageConnection(SqlServerStorage storage)
		{
			_storage = storage;
            _schema = storage.Options.Schema;
		}

		public Task StoreJobAsync(DelayedJob job)
		{
			if (job == null) throw new ArgumentNullException(nameof(job));

			var sql = $@"
				INSERT INTO {_schema}.DelayedJobs
				(Id, Data, Due)
				VALUES
				(@id, @data, @due)";

			return _storage.UseConnectionAsync(connection =>
			{
				return connection.ExecuteAsync(sql, new
				{
					id = job.Id,
					data = job.Data,
					due = job.Due.HasValue ? NormalizeDateTime(job.Due.Value) : job.Due
				});
			});
		}

		public Task StoreJobAsync(CronJob job)
		{
			if (job == null) throw new ArgumentNullException(nameof(job));

			var sql = $@"
				INSERT INTO {_schema}.CronJobs
				(Id, Name, TypeName, Cron, LastRun)
				VALUES
				(@id, @name, @typeName, @cron, @lastRun)";

			return _storage.UseConnectionAsync(connection =>
			{
				return connection.ExecuteAsync(sql, new
				{
					id = job.Id,
					name = job.Name,
					typeName = job.TypeName,
					cron = job.Cron,
					lastRun = NormalizeDateTime(job.LastRun)
				});
			});
		}

		public Task UpdateCronJobAsync(CronJob job)
		{
			if (job == null) throw new ArgumentNullException(nameof(job));

			var sql = $@"
				UPDATE {_schema}.CronJobs
				SET TypeName = @typeName, Cron = @cron, LastRun = @lastRun
				WHERE Id = @id";

			return _storage.UseConnectionAsync(connection =>
			{
				return connection.ExecuteAsync(sql, new
				{
					id = job.Id,
					typeName = job.TypeName,
					cron = job.Cron,
					lastRun = job.LastRun
				});
			});
		}

		public Task<IFetchedJob> FetchNextJobAsync()
		{
			var sql = $@"
				DELETE TOP (1) from {_schema}.DelayedJobs with (readpast, updlock, rowlock)
				output DELETED.*
				WHERE Due IS NULL";

			return FetchNextDelayedJobCoreAsync(sql);
		}

		public Task<IFetchedJob> FetchNextDelayedJobAsync(DateTime from, DateTime to)
		{
			var sql = $@"
				DELETE TOP (1) from {_schema}.DelayedJobs with (readpast, updlock, rowlock)
				output DELETED.*
				WHERE Due >= @from AND Due < @to";

			return FetchNextDelayedJobCoreAsync(sql, new { from = NormalizeDateTime(from), to });
		}

		private DateTime NormalizeDateTime(DateTime from)
		{
			if (from == DateTime.MinValue)
			{
				return new DateTime(1754, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			}
			return from;
		}

		private async Task<IFetchedJob> FetchNextDelayedJobCoreAsync(string sql, object args = null)
		{
			DelayedJob fetchedJob = null;
			var connection = _storage.CreateAndOpenConnection();
			var transaction = connection.BeginTransaction(IsolationLevel.ReadCommitted);

			try
			{
				fetchedJob =
					(await connection.QueryAsync<DelayedJob>(sql, args, transaction))
					.FirstOrDefault();
			}
			catch (SqlException)
			{
				transaction.Dispose();
				_storage.ReleaseConnection(connection);
				throw;
			}

			if (fetchedJob == null)
			{
				transaction.Rollback();
				transaction.Dispose();
				_storage.ReleaseConnection(connection);
				return null;
			}

			return new SqlServerFetchedJob(
				fetchedJob,
				_storage,
				connection,
				transaction);
		}

		public Task<CronJob[]> GetCronJobsAsync()
		{
			var sql = $@"
				SELECT * FROM {_schema}.CronJobs";

			return _storage.UseConnectionAsync(async connection =>
			{
				return
					(await connection.QueryAsync<CronJob>(sql))
					.ToArray();
			});
		}

		public Task<CronJob> GetCronJobByNameAsync(string name)
		{
			var sql = $@"
				SELECT * FROM {_schema}.CronJobs
				WHERE Name = @name";

			return _storage.UseConnectionAsync(async connection =>
			{
				return
					(await connection.QueryAsync<CronJob>(sql, new { name }))
					.FirstOrDefault();
			});
		}

		public Task RemoveCronJobAsync(string name)
		{
			var sql = $@"
				DELETE FROM {_schema}.CronJobs
				WHERE Name = @name";

			return _storage.UseConnectionAsync(connection =>
			{
				return connection.ExecuteAsync(sql, new { name });
			});
		}

		public void Dispose()
		{
		}
	}
}
