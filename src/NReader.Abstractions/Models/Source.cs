using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NReader.Abstractions
{
    public abstract class Source
    {
        private static readonly IReadOnlyCollection<Article> Empty = Array.Empty<Article>();

        public virtual string Title { get; }

        public virtual Uri Url { get; }

        public virtual Task<IReadOnlyCollection<Article>> GetArticlesAsync()
        {
            return Task.FromResult(Empty);
        }
    }
}
