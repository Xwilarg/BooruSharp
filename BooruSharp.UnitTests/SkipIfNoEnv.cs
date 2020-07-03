using System;
using Xunit;

namespace BooruSharp.UnitTests
{
    public sealed class SkipIfNoEnv : FactAttribute
    {
        public SkipIfNoEnv()
        {
            Timeout = 30000;

            string env = Environment.GetEnvironmentVariable("GELBOORU_USER_ID");
            if (env == null)
                Skip = "Environment variables not set";
        }
    }
}