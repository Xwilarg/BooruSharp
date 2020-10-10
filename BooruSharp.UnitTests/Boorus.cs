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
                string userID = Environment.GetEnvironmentVariable("PIXIV_USER_ID");
                string password = Environment.GetEnvironmentVariable("PIXIV_PASSWORD");

                Skip.If(userID is null || password is null, "Pixiv user ID and/or password aren't set.");

                await pixiv.LoginAsync(userID, password);
            }

            return booru;
        }
    }
}
