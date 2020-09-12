using System;
using System.Collections.Generic;
using BooruSharp.Booru;
using BooruSharp.Others;
using Xunit;

namespace BooruSharp.UnitTests
{
    internal static class Boorus
    {
        private static readonly Dictionary<Type, ABooru> _boorus = new Dictionary<Type, ABooru>();

        public static ABooru Get(Type type)
        {
            lock (_boorus)
            {
                if (!_boorus.TryGetValue(type, out var booruTask))
                {
                    booruTask = CreateBooru(type);
                    _boorus[type] = booruTask;
                }

                return booruTask;
            }
        }

        public static ABooru Get<T>() where T : ABooru
        {
            return Get(typeof(T));
        }

        private static ABooru CreateBooru(Type type)
        {
            var booru = (ABooru)Activator.CreateInstance(type);

            if (booru is Pixiv pixiv)
            {
                string userID = Environment.GetEnvironmentVariable("PIXIV_USER_ID");
                string password = Environment.GetEnvironmentVariable("PIXIV_PASSWORD");

                Skip.If(userID == null || password == null, "Pixiv user ID and/or password aren't set.");

                pixiv.Auth = new BooruAuth(userID, password);
            }

            return booru;
        }
    }
}
