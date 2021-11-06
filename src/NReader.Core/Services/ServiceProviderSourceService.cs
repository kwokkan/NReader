using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using NReader.Abstractions;

namespace NReader.Core
{
    public class ServiceProviderSourceService : ISourceService
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderSourceService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        Task<IEnumerable<Source>> ISourceService.GetSourcesAsync()
        {
            var sources = _serviceProvider.GetServices<Source>();

            return Task.FromResult(sources);
        }
    }
}
