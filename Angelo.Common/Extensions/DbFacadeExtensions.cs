using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbFacadeExtensions
    {
        // will throw an error if the connection fails
        public static void TestConnection(this DatabaseFacade db)
        {
            try
            {
                var cnInfo = db.GetDbConnection();
                var cnBuilder = new SqlConnectionStringBuilder(cnInfo.ConnectionString);

                // default timeout may be too long... let's optimize since we're dealing
                // with dedicated local & internal network servers
                cnBuilder.ConnectTimeout = 2; // 2 seconds.

                var connection = new SqlConnection(cnBuilder.ToString());

                connection.Open();
                connection.Close();
            }
            catch (Exception ex)
            {
                throw new Exception($"Connection test failed for {db.GetType().FullName}.", ex);
            }
        }

        // will return false if the connetion fails
        public static bool TryTestConnection(this DatabaseFacade db)
        {
            try
            {
                db.TestConnection();
                return true;
            }
            catch (Exception ex)
            {
                // intentionally suppressed
            }

            return false;
        }

        public static string GetInitialCatalog(this DatabaseFacade db)
        {
            var cnInfo = db.GetDbConnection();
            var cnBuilder = new SqlConnectionStringBuilder(cnInfo.ConnectionString);

            return cnBuilder.InitialCatalog;
        }

        public static bool ConstraintExists(this DatabaseFacade db, string table, string schema, string constraint)
        {
            var catalog = db.GetInitialCatalog();

            var sql = $@"
                SELECT [constraint_name] 
                FROM 
                    INFORMATION_SCHEMA.TABLE_CONSTRAINTS 
                WHERE 
                    [table_catalog] = '{catalog}'
                    AND [table_schema] = '{schema}'
                    AND [table_name] = '{table}'
                    AND [constraint_name] = '{constraint}'
           ";
            var result = TryExecuteScaler(db, sql);

            return result != null;
        }


        public static bool SchemaExists(this DatabaseFacade db, string schemaName)
        {
            var sql = $"SELECT [name] FROM [sys].[schemas] WHERE [name] = '{schemaName}'";
            var result = TryExecuteScaler(db, sql);

            return result != null;
        }

        public static bool TableExists(this DatabaseFacade db, string table, string schema)
        {
            if (db.SchemaExists(schema) == false)
                return false;

            // else
            return TableExists(db, $"[{schema}].[{table}]");
        }

        public static bool TableExists(this DatabaseFacade db, string table)
        {
            var sql = $"SELECT ISNULL(OBJECT_ID('{table}','U'), 0)";
            var tableId = (int)TryExecuteScaler(db, sql);

            return tableId > 0;
        }

        public static bool HasColumn(this DatabaseFacade db, string table, string schema, string column)
        {
            var tableId = GetTableId(db, table, schema);

            if (tableId > 0)
            {
                var sql = $"SELECT [name] FROM [sys].[columns] WHERE [object_id] = {tableId} AND [name] = '{column}'";
                var result = TryExecuteScaler(db, sql);

                return result != null;
            }

            return false;
        }

        public static void CreateSchema(this DatabaseFacade db, string schema)
        {
            db.ExecuteSqlCommand( $@"
                IF NOT EXISTS (SELECT * FROM [sys].[schemas] WHERE [name] = '{schema}')
                BEGIN
                    EXEC('CREATE SCHEMA [{schema}]')
                END
            ");
        }

        public static void DropSchema(this DatabaseFacade db, string schema)
        {
            db.ExecuteSqlCommand($@"
                IF EXISTS (SELECT * FROM sys.schemas WHERE name = '{schema}')
                BEGIN
                    EXEC('DROP SCHEMA [{schema}]')
                END
            ");
        }

        public static void AlterTableName(this DatabaseFacade db, string table, string schema, string tableName)
        {
            db.ExecuteSqlCommand($@"
                EXEC sp_rename '{schema}.{table}', '{tableName}'; 
            ");
        }

        public static void AlterTableSchema(this DatabaseFacade db, string table, string fromSchema, string toSchema)
        {
            db.ExecuteSqlCommand($@"
                ALTER SCHEMA [{toSchema}] TRANSFER [{fromSchema}].[{table}];  
            ");
        }

        public static void DropTable(this DatabaseFacade db, string table, string schema)
        {
            db.ExecuteSqlCommand($@"
                IF OBJECT_ID('[{schema}].[{table}]','U') IS NOT NULL 
                BEGIN
                    DROP TABLE [{schema}].[{table}]
                END
            ");
        }

        public static object ExecuteScaler(this DatabaseFacade db, string sql)
        {
            return ExecuteScalerAsync(db, sql).Result;
        }

        public static async Task<object> ExecuteScalerAsync(this DatabaseFacade db, string sql)
        {
            var connection = db.GetDbConnection();
            object result = null;

            try
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    result = await command.ExecuteScalarAsync();
                }
            }
            catch { throw; }
            finally
            {
                connection.Close();

            }

            return result;
        }

        public static void ExecuteNonQuery(this DatabaseFacade db, string sql)
        {
            ExecuteNonQueryAsync(db, sql).Wait();
        }

        public static async Task ExecuteNonQueryAsync(this DatabaseFacade db, string sql)
        {
            var connection = db.GetDbConnection();

            try
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    await command.ExecuteNonQueryAsync();
                }
            }
            catch { throw; }
            finally
            {
                connection.Close();
            }
        }

        // private methods
        private static object TryExecuteScaler(this DatabaseFacade db, string sql)
        {
            var connection = db.GetDbConnection();
            object result = null;

            try
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    result = command.ExecuteScalar();
                }
            }
            catch(Exception ex)
            {
                // intentionally suppressed
            }
            finally
            {
                connection.Close();
            }

            return result;
        }

        private static int GetTableId(DatabaseFacade db, string table, string schema)
        {
            var sql = $"SELECT ISNULL(OBJECT_ID('[{schema}].[{table}]','U'), 0)";

            return (int)TryExecuteScaler(db, sql);
        }
     
    }
}
