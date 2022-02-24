using NReader.Abstractions;

namespace NReader.Storage.Abstractions;

public class StoredFeed
{
    public IStoredFeedId Id { get; init; }

    public Feed Feed { get; init; }

    public StoredFeed(IStoredFeedId id, Feed feed)
    {
        Id = id;
        Feed = feed;
    }
}
