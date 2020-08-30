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
        private static readonly Dictionary<Type, Task<ABooru>> _boorus =
            new Dictionary<Type, Task<ABooru>>()
            {
                [typeof(Atfbooru)] = CreateBooruAsync<Atfbooru>(),
                [typeof(DanbooruDonmai)] = CreateBooruAsync<DanbooruDonmai>(),
                [typeof(E621)] = CreateBooruAsync<E621>(),
                [typeof(E926)] = CreateBooruAsync<E926>(),
                [typeof(Furrybooru)] = CreateBooruAsync<Furrybooru>(),
                [typeof(Gelbooru)] = CreateBooruAsync<Gelbooru>(),
                [typeof(Konachan)] = CreateBooruAsync<Konachan>(),
                [typeof(Lolibooru)] = CreateBooruAsync<Lolibooru>(),
                [typeof(Realbooru)] = CreateBooruAsync<Realbooru>(),
                [typeof(Rule34)] = CreateBooruAsync<Rule34>(),
                [typeof(Safebooru)] = CreateBooruAsync<Safebooru>(),
                [typeof(Sakugabooru)] = CreateBooruAsync<Sakugabooru>(),
                [typeof(SankakuComplex)] = CreateBooruAsync<SankakuComplex>(),
                [typeof(Xbooru)] = CreateBooruAsync<Xbooru>(),
                [typeof(Yandere)] = CreateBooruAsync<Yandere>(),
                [typeof(Pixiv)] = CreateBooruAsync<Pixiv>(),
            };



        public static Task<ABooru> GetAsync(Type type)
        {
            return _boorus[type];
        }

        public static Task<ABooru> GetAsync<T>() where T : ABooru
        {
            return GetAsync(typeof(T));
        }

        private static async Task<ABooru> CreateBooruAsync<T>() where T : ABooru
        {
            var booru = (ABooru)Activator.CreateInstance(typeof(T));

            if (booru is Pixiv pixiv)
            {
                string userID = Environment.GetEnvironmentVariable("PIXIV_USER_ID");
                string password = Environment.GetEnvironmentVariable("PIXIV_PASSWORD");

                Skip.If(userID == null || password == null, "Pixiv user ID and/or password aren't set.");

                await pixiv.LoginAsync(userID, password);
            }

            return booru;
        }
    }
}
