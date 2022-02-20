using NReader.Abstractions;

namespace NReader.Core;

public class MappedSource
{
    public long Id { get; init; }

    public Source Source { get; init; }
}
