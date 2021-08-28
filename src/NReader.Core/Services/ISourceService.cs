using System.Collections.Generic;
using System.Threading.Tasks;

using NReader.Abstractions;

namespace NReader.Core
{
    public interface ISourceService
    {
        Task<IEnumerable<Source>> GetSourcesAsync();
    }
}
