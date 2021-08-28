using System;

namespace NReader.Abstractions
{
    public abstract class Source
    {
        public string Title { get; }

        public Uri Url { get; }
    }
}
