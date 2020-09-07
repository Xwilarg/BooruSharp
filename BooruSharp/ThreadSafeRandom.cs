using System;
using System.Security.Cryptography;
using System.Threading;

namespace BooruSharp
{
    /// <summary>
    /// Represents a thread-safe, pseudo-random number generator.
    /// </summary>
    internal sealed class ThreadSafeRandom : Random, IDisposable
    {
        private static readonly RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();

        private readonly ThreadLocal<Random> _localRandom = new ThreadLocal<Random>(() =>
        {
            var buffer = new byte[4];
            // RNGCryptoServiceProvider is thread-safe for use in this manner
            _global.GetBytes(buffer);
            return new Random(BitConverter.ToInt32(buffer, 0));
        });

        public override int Next()
        {
            return _localRandom.Value.Next();
        }

        public override int Next(int maxValue)
        {
            return _localRandom.Value.Next(maxValue);
        }

        public override int Next(int minValue, int maxValue)
        {
            return _localRandom.Value.Next(minValue, maxValue);
        }

        public override double NextDouble()
        {
            return _localRandom.Value.NextDouble();
        }

        public override void NextBytes(byte[] buffer)
        {
            _localRandom.Value.NextBytes(buffer);
        }

        public void Dispose() => _localRandom.Dispose();
    }
}
