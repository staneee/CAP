using DotNetCore.CAP.Persistence;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Text;

namespace DotNetCore.CAP
{
    internal class SqliteCapOptionsExtension : ICapOptionsExtension
    {
        private readonly Action<SqliteOptions> _configure;

        public SqliteCapOptionsExtension(Action<SqliteOptions> configure)
        {
            _configure = configure;
        }

        public void AddServices(IServiceCollection services)
        {
            services.AddSingleton<CapStorageMarkerService>();
            services.Configure(_configure);
            services.AddSingleton<IConfigureOptions<SqliteOptions>, ConfigureSqliteOptions>();

            services.AddSingleton<IDataStorage, SqliteDataStorage>();
            services.AddSingleton<IStorageInitializer, SqliteStorageInitializer>();
            services.AddTransient<ICapTransaction, SqliteCapTransaction>();
        }
    }
}
