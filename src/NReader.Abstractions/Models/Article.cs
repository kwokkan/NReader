using System;
using System.Collections.Generic;

namespace NReader.Abstractions
{
    public class Article
    {
        public static readonly Article Empty = new Article { Pages = Array.Empty<Page>() };

        public string Id { get; set; }

        public string Title { get; set; }

        public Uri Uri { get; set; }

        public IReadOnlyCollection<Page> Pages { get; set; } = Array.Empty<Page>();
    }
}
