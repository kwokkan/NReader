namespace NReader.Storage.Abstractions;

public class GetArticlesSearchFilter
{
    public string? UserId { get; set; }

    public bool? Unread { get; set; }
}
