using System;
using Xunit;

namespace BooruSharp.UnitTests
{
    public sealed class SkipIfNoEnvTheory : TheoryAttribute
    {
        public SkipIfNoEnvTheory()
        {
            Timeout = 30000;

            string env = Environment.GetEnvironmentVariable("GELBOORU_USER_ID");
            if (env == null)
                Skip = "Environment variables not set";
        }
    }
}