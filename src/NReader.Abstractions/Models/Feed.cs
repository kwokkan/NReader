using System;
using System.Collections.Generic;

namespace NReader.Abstractions
{
    public class Feed
    {
        public static readonly Feed Empty = new Feed { Articles = Array.Empty<Article>() };

        public string Id { get; set; } = "Default";

        public string Title { get; set; } = "Default";

        public Uri Uri { get; set; }

        public IReadOnlyCollection<Article> Articles { get; set; } = Array.Empty<Article>();
    }
}
