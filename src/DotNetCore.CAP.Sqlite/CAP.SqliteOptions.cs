using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.CAP
{
    public class SqliteOptions : EFOptions
    {
        /// <summary>
        /// Gets or sets the database's connection string that will be used to store database entities.
        /// </summary>
        public string ConnectionString { get; set; }
    }


    internal class ConfigureSqliteOptions : IConfigureOptions<SqliteOptions>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public ConfigureSqliteOptions(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public void Configure(SqliteOptions options)
        {
            if (options.DbContextType == null) return;

            using var scope = _serviceScopeFactory.CreateScope();
            var provider = scope.ServiceProvider;
            using var dbContext = (DbContext)provider.GetRequiredService(options.DbContextType);
            options.ConnectionString = dbContext.Database.GetDbConnection().ConnectionString;
        }
    }
}
