using NReader.Abstractions;

namespace NReader.Storage.Abstractions;

public class StoredSource
{
    public IStoredKey Key { get; init; }

    public Source Source { get; init; }

    public StoredSource(IStoredKey key, Source source)
    {
        Key = key;
        Source = source;
    }
}
