using System;

namespace NReader.Abstractions
{
    public abstract class Source
    {
        public virtual string Title { get; }

        public virtual Uri Url { get; }
    }
}
