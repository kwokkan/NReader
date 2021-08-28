using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using NReader.Abstractions;

namespace NReader.Core
{
    public class SourceService : ISourceService
    {
        Task<IEnumerable<Source>> ISourceService.GetSourcesAsync()
        {
            IEnumerable<Source> sources = Array.Empty<Source>();

            return Task.FromResult(sources);
        }
    }
}
