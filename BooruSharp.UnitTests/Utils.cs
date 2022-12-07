﻿using BooruSharp.Booru;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BooruSharp.UnitTests
{
    public static class Utils
    {
        public static IEnumerable<object[]> BooruParams { get; } = new object[][]
        {
            new object[] { new BooruTestData() { BooruType = typeof(Atfbooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(DanbooruDonmai) } },
            new object[] { new BooruTestData() { BooruType = typeof(Derpibooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(E621) } },
            new object[] { new BooruTestData() { BooruType = typeof(E926) } },
            new object[] { new BooruTestData() { BooruType = typeof(Furrybooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(Gelbooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(Konachan) } },
            new object[] { new BooruTestData() { BooruType = typeof(Lolibooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(Ponybooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(Realbooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(Rule34) } },
            new object[] { new BooruTestData() { BooruType = typeof(Safebooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(Sakugabooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(SankakuComplex) } },
            new object[] { new BooruTestData() { BooruType = typeof(Twibooru) } },
            new object[] { new BooruTestData() { BooruType = typeof(Xbooru) } },
            new object[] {  new BooruTestData() { BooruType = typeof(Yandere) } },
            //new BooruTestData() { BooruType = typeof(Pixiv) }
        };

        private static readonly Dictionary<Type, Task<IBooru>> _boorus = new();

        public static Task<IBooru> GetAsync(Type type)
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

        private static async Task<IBooru> CreateBooruAsync(Type type)
        {
            var booru = (IBooru)Activator.CreateInstance(type);

            /*if (booru is Pixiv pixiv)
            {
                string refresh = Environment.GetEnvironmentVariable("PIXIV_REFRESH_TOKEN");

                Skip.If(refresh == null, "Pixiv tokens aren't set.");

                await pixiv.LoginAsync(refresh);
            }*/

            return booru;
        }
    }
}