﻿using NReader.Abstractions;
using NReader.Blazor;
using NReader.Extensions.Test;
using NReader.Extensions.Xkcd;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNReader(this IServiceCollection services)
        {
            services.AddNReaderCore();

            services.AddNReaderStaticSources();

            services.AddNReaderStorageProvider();

            services.AddNReaderInitialisation();

            return services;
        }

        private static IServiceCollection AddNReaderStaticSources(this IServiceCollection services)
        {
            services.AddScoped<Source, TestSource>();

            services
                .AddScoped<Source, XkcdSource>()
                .AddHttpClient<XkcdSource>()
            ;

            return services;
        }

        private static IServiceCollection AddNReaderStorageProvider(this IServiceCollection services)
        {
            services.AddSqliteStorageProvider();

            return services;
        }

        private static IServiceCollection AddNReaderInitialisation(this IServiceCollection services)
        {
            services.AddScoped<Initialisation>();

            return services;
        }
    }
}
