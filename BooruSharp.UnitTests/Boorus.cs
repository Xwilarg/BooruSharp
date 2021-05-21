using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooruSharp.Booru;
using BooruSharp.Others;
using Xunit;

namespace BooruSharp.UnitTests
{
    internal static class Boorus
    {
        private static readonly Dictionary<Type, Task<ABooru>> _boorus = new Dictionary<Type, Task<ABooru>>();



        public static Task<ABooru> GetAsync(Type type)
        {
            lock (_boorus)
            {
                if (!_boorus.TryGetValue(type, out var booruTask))
                {
                    booruTask = Task.Run(() => CreateBooruAsync(type));
                    _boorus[type] = booruTask;
                }

                return booruTask;
            }
        }

        public static Task<ABooru> GetAsync<T>() where T : ABooru
        {
            return GetAsync(typeof(T));
        }

        private static async Task<ABooru> CreateBooruAsync(Type type)
        {
            var booru = (ABooru)Activator.CreateInstance(type);

            if (booru is Pixiv pixiv)
            {
                string refresh = Environment.GetEnvironmentVariable("PIXIV_REFRESH_TOKEN");

                Skip.If(refresh == null, "Pixiv tokens aren't set.");

                await pixiv.LoginAsync(refresh);
            }

            return booru;
        }
    }
}
