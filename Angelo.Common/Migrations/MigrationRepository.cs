using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Angelo.Common.Migrations
{
    public class MigrationRepository : DbContext
    {
        private const string _versionKey = "Migrations.Schema";
        private const string _version = "1.0.0";
        private const string _schema = "app";
        private const string _metaTableName = "Meta";
        private const string _historyTableName = "Migrations";

        private static bool _frameworkMigrated = false;

        protected DbSet<MigrationRecord> MigrationHistory { get; set; }
        protected DbSet<MetaKeyValuePair> MetaValues { get; set; }

        public MigrationRepository(DbContextOptions<MigrationRepository> options) : base(options)
        {
            if(_frameworkMigrated == false)
            {
                // Test connection. If success, let's migrate our internal repo 
                // Otherwise we'll check again before committing any data.

                if(Database.TryTestConnection() == true)
                {
                    MigrateFrameworkModels().Wait();
                }
            }
        }

        public async Task<bool> HasHistory(IAppMigration migrationInfo)
        {
            if (TryTestForTable(_historyTableName) == false)
                return false;

            // else
            return await MigrationHistory.AnyAsync(x =>
                x.Id == migrationInfo.Id
                && x.Migration == migrationInfo.Migration
            );
        }

        public void LogHistory(IAppMigration migrationInfo, MigrationResult result, DateTime started, DateTime finished)
        {
            var migrationRecord = new MigrationRecord()
            {
                Id = migrationInfo.Id,
                Migration = migrationInfo.Migration,
                Executed = started,
                Duration = finished.Subtract(started).Milliseconds,
                Result = result.Status.ToString(),
                StatusCode = (int)result.Status,
                Message = result.Message
            };

            MigrationHistory.Add(migrationRecord);
        }

        public async Task SetMetaValue(string key, string value)
        {
            if (string.IsNullOrEmpty(key.Trim()))
                throw new ArgumentNullException(nameof(key));

            // update if table & entity exists
            if(TryTestForTable(_metaTableName) == true)
            {
                var entity = await MetaValues.FirstOrDefaultAsync(x => x.Key == key);

                if(entity != null)
                {
                    entity.Value = value;
                    return;
                }
            }

            // add for insert
            MetaValues.Add(new MetaKeyValuePair(key, value));          
        }

        public async Task<string> GetMetaValue(string key)
        {
            if (string.IsNullOrEmpty(key.Trim()))
                throw new ArgumentNullException(nameof(key));

            if (TryTestForTable(_metaTableName) == false)
                return null;

            var entity = await MetaValues.FirstOrDefaultAsync(x => x.Key == key);

            return entity?.Value;
        }

        public override int SaveChanges()
        {
            return SaveChangesAsync().Result;
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            // test to see if the framework is current
            if (!_frameworkMigrated)
            {
                await MigrateFrameworkModels();
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MetaKeyValuePair>(entity => {
                entity.ToTable(_metaTableName, _schema).HasKey(x => x.Key);
            });

            modelBuilder.Entity<MigrationRecord>(entity => {
                entity.ToTable(_historyTableName, _schema).HasKey(x => new { x.Id, x.Migration });
            });
        }

        private async Task MigrateFrameworkModels()
        {
            // ensure the database exists
            try
            {
                Database.TestConnection();
            }
            catch (Exception ex)
            {
                throw new Exception("Cannot create Migration Repository.", ex);
            }

            // create schema if needed
            if(Database.SchemaExists(_schema) == false)
            {
                Database.CreateSchema(_schema);
            }


            // create meta table if needed
            if (Database.TableExists(_metaTableName, _schema) == false)
            {
                // initial version. do not change
                Database.ExecuteSqlCommand($@"
                    CREATE TABLE [{_schema}].[{_metaTableName}]
                    (       
                        [Key] nvarchar(150) NOT NULL, 
                        [Value] nvarchar(4000) NULL,
                        CONSTRAINT [PK_{_metaTableName}] PRIMARY KEY ([Key] ASC)
                    );
                ");
            }

            // Create history table 
            if(Database.TableExists(_historyTableName, _schema) == false)
            {
                // initial version, do not change
                Database.ExecuteSqlCommand($@"
                    CREATE TABLE [{_schema}].[{_historyTableName}]
                    (
                        [Id] nvarchar(32) NOT NULL,          
                        [Migration] nvarchar(150) NOT NULL, 
                        [Executed] datetime NOT NULL,
                        [Duration] int NOT NULL,
                        [StatusCode] INT NOT NULL,
                        [Result] nvarchar(32) NOT NULL,
                        [Message] nvarchar(4000) NULL,
                        CONSTRAINT [PK_{_historyTableName}] PRIMARY KEY CLUSTERED ([Id] ASC, [Migration])
                    );

                    CREATE NONCLUSTERED INDEX [IX_Executed] ON [{_schema}].[{_historyTableName}](Executed);
                ");
            }

            // check for version discrepancy and apply fixes

            var registeredVersion = await GetMetaValue(_versionKey);

            if (_version != registeredVersion)
            {
                // remove old version table if present
                if(Database.TableExists("__AppMigrations", "dbo"))
                {
                    Database.DropTable("__AppMigrations", "dbo");
                }

                // persiste updated migration version 
                await SetMetaValue(_versionKey, _version);
            }

            // update static migration flag
            _frameworkMigrated = true;
        }

        private bool TryTestForTable(string tableName, string schema = _schema)
        {
            if(Database.TryTestConnection() == true)
            { 
                return Database.TableExists(tableName, schema);
            }

            // else
            return false;
        }
    }
}
