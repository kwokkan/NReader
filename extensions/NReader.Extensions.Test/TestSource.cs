using System;

using NReader.Abstractions;

namespace NReader.Extensions.Test
{
    public class TestSource : Source
    {
        public override string Title => "Test Source";

        public override Uri Url => new Uri("https://localhost/");
    }
}
