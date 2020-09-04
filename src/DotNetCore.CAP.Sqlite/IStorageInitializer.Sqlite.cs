using DotNetCore.CAP.Persistence;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetCore.CAP
{
    public class SqliteStorageInitializer : IStorageInitializer
    {
        private readonly ILogger _logger;
        private readonly IOptions<SqliteOptions> _options;

        public SqliteStorageInitializer(
            ILogger<SqliteStorageInitializer> logger,
            IOptions<SqliteOptions> options)
        {
            _options = options;
            _logger = logger;
        }

        public virtual string GetPublishedTableName()
        {
            return $"\"{_options.Value.Schema}\".\"published\"";
        }

        public virtual string GetReceivedTableName()
        {
            return $"\"{_options.Value.Schema}\".\"received\"";
        }

        public async Task InitializeAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested) return;

            var sql = CreateDbTablesScript(_options.Value.Schema);
            using (var connection = new SqliteConnection(_options.Value.ConnectionString))
                connection.ExecuteNonQuery(sql);

            await Task.CompletedTask;

            _logger.LogDebug("Ensuring all create database tables script are applied.");
        }


        protected virtual string CreateDbTablesScript(string schema)
        {
            var batchSql = $@"
CREATE SCHEMA IF NOT EXISTS ""{schema}"";

CREATE TABLE IF NOT EXISTS {GetReceivedTableName()}(
	""Id"" BIGINT PRIMARY KEY NOT NULL,
    ""Version"" VARCHAR(20) NOT NULL,
	""Name"" VARCHAR(200) NOT NULL,
	""Group"" VARCHAR(200) NULL,
	""Content"" TEXT NULL,
	""Retries"" INT NOT NULL,
	""Added"" TIMESTAMP NOT NULL,
    ""ExpiresAt"" TIMESTAMP NULL,
	""StatusName"" VARCHAR(50) NOT NULL
);

CREATE TABLE IF NOT EXISTS {GetPublishedTableName()}(
	""Id"" BIGINT PRIMARY KEY NOT NULL,
    ""Version"" VARCHAR(20) NOT NULL,
	""Name"" VARCHAR(200) NOT NULL,
	""Content"" TEXT NULL,
	""Retries"" INT NOT NULL,
	""Added"" TIMESTAMP NOT NULL,
    ""ExpiresAt"" TIMESTAMP NULL,
	""StatusName"" VARCHAR(50) NOT NULL
);";
            return batchSql;
        }
    }
}
