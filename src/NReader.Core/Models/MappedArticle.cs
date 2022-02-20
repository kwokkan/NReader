using NReader.Abstractions;

namespace NReader.Core;

public class MappedArticle
{
    public long Id { get; init; }

    public Article Article { get; init; }
}
