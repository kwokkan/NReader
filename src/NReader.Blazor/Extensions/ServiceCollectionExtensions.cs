using NReader.Abstractions;
using NReader.Extensions.Test;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNReaderStaticSources(this IServiceCollection services)
        {
            services.AddScoped<Source, TestSource>();

            return services;
        }
    }
}
