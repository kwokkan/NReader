using NReader.Abstractions;

namespace NReader.Core;

public class MappedFeed
{
    public long Id { get; init; }

    public Feed Feed { get; init; }
}
