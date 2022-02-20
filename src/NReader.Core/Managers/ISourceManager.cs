using System.Collections.Generic;
using System.Threading.Tasks;
using NReader.Abstractions;

namespace NReader.Core;

public interface ISourceManager
{
    Task<IEnumerable<Source>> GetAllSourcesAsync();
}
