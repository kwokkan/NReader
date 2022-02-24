using NReader.Abstractions;

namespace NReader.Storage.Abstractions;

public class StoredSource
{
    public IStoredSourceId Id { get; init; }

    public Source Source { get; init; }

    public StoredSource(IStoredSourceId id, Source source)
    {
        Id = id;
        Source = source;
    }
}
