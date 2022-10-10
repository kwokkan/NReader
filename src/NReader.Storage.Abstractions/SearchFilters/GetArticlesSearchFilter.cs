namespace NReader.Storage.Abstractions;

public class GetArticlesSearchFilter
{
    public string? UserId { get; set; }

    public bool? Unread { get; set; }

    public int Offset { get; set; }

    public int Limit { get; set; } = 100;
}
