using NReader.Core;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNReaderCore(this IServiceCollection services)
        {
            services
                .AddSingleton<ISourceService, SourceService>()
            ;

            return services;
        }
    }
}
