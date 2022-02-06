using NReader.Abstractions;
using NReader.Extensions.Test;
using NReader.Extensions.Xkcd;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNReaderStaticSources(this IServiceCollection services)
        {
            services.AddScoped<Source, TestSource>();

            services
                .AddScoped<Source, XkcdSource>()
                .AddHttpClient<XkcdSource>()
            ;

            return services;
        }

        public static IServiceCollection AddNReaderStorageProvider(this IServiceCollection services)
        {
            services.AddSqliteStorageProvider();

            return services;
        }
    }
}
