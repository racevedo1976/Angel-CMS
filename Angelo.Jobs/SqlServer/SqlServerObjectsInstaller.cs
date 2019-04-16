using System;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;
using Dapper;
using Microsoft.Extensions.Logging;

namespace Angelo.Jobs
{
	public static class SqlServerObjectsInstaller
	{
        private const string REQUIRED_VERSION = "1.0.0";

		public static void Install(SqlConnection connection, SqlServerOptions options, ILogger logger)
		{
			if (connection == null) throw new ArgumentNullException(nameof(connection));

			logger?.LogInformation("Installing Jobs SQL objects...");

			var script = GetStringResource(
				typeof(SqlServerObjectsInstaller).GetTypeInfo().Assembly,
				"Angelo.Jobs.Data.SqlServer.install.sql"
            );

	
            script = script.Replace("${schema}", options.Schema);
            script = script.Replace("${keystore}", options.KeyStoreTable);
            script = script.Replace("${version}", REQUIRED_VERSION);

			try
			{
				connection.Execute(script);
                logger?.LogInformation("Jobs SQL objects installed.");
            }
			catch (SqlException ex)
			{
               var errorMsg = "An exception occurred during running jobs install.sql";

                logger?.LogError(errorMsg);
                throw new Exception(errorMsg, ex);
			}

		}

		private static string GetStringResource(Assembly assembly, string resourceName)
		{
			using (var stream = assembly.GetManifestResourceStream(resourceName))
			{
				if (stream == null)
				{
					throw new InvalidOperationException(String.Format(
						"Requested resource `{0}` was not found in the assembly `{1}`.",
						resourceName,
						assembly));
				}

				using (var reader = new StreamReader(stream))
				{
					return reader.ReadToEnd();
				}
			}
		}
	}
}
