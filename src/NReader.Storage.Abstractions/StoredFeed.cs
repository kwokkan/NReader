using NReader.Abstractions;

namespace NReader.Storage.Abstractions;

public class StoredFeed
{
    public IStoredKey Key { get; init; }

    public Feed Feed { get; init; }

    public StoredFeed(IStoredKey key, Feed feed)
    {
        Key = key;
        Feed = feed;
    }
}
