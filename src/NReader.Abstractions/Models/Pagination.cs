using System.Collections.Generic;

namespace NReader.Abstractions;

public class Pagination<T>
{
    public int Offset { get; set; }

    public int Total { get; set; }

    public IReadOnlyCollection<T> Results { get; set; }
}
