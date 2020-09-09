using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BooruSharp.Booru;
using BooruSharp.Others;
using Xunit;

namespace BooruSharp.UnitTests
{
    // TODO: make async methods into regular methods.
    // Pixiv now uses internal Task to achieve thread safety and
    // we don't need to wait until LoginAsync completes.
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

        private static Task<ABooru> CreateBooruAsync(Type type)
        {
            var booru = (ABooru)Activator.CreateInstance(type);

            if (booru is Pixiv pixiv)
            {
                string userID = Environment.GetEnvironmentVariable("PIXIV_USER_ID");
                string password = Environment.GetEnvironmentVariable("PIXIV_PASSWORD");

                Skip.If(userID == null || password == null, "Pixiv user ID and/or password aren't set.");

                pixiv.Auth = new BooruAuth(userID, password);
            }

            return Task.FromResult(booru);
        }
    }
}
