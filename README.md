| BooruSharp     | BooruSharp.Others  |
| -------------- | ------------ |
| [![NuGet](https://img.shields.io/nuget/v/BooruSharp.svg)](https://www.nuget.org/packages/BooruSharp/) | [![NuGet](https://img.shields.io/nuget/v/BooruSharp.Others.svg)](https://www.nuget.org/packages/BooruSharp.Others/) |
| ![Nuget](https://img.shields.io/nuget/dt/BooruSharp) | ![Nuget](https://img.shields.io/nuget/dt/BooruSharp.Others) |

| GitHub CI      | AppVeyor CI  |
| -------------- | ------------ |
| [![Build (GitHub CI)](https://github.com/Xwilarg/BooruSharp/workflows/.NET%20Core/badge.svg)](https://github.com/Xwilarg/BooruSharp/actions) | [![Build (AppVeyor)](https://ci.appveyor.com/api/projects/status/n503kvx6b0a45bcv/branch/master?svg=true)](https://ci.appveyor.com/project/Xwilarg/boorusharp/branch/master) |

[![codecov](https://codecov.io/gh/Xwilarg/BooruSharp/branch/master/graph/badge.svg)](https://codecov.io/gh/Xwilarg/BooruSharp)
[![Unit tests](https://img.shields.io/appveyor/tests/xwilarg/boorusharp.svg)](https://ci.appveyor.com/project/Xwilarg/boorusharp/branch/master/tests)
[![CodeFactor](https://www.codefactor.io/repository/github/xwilarg/boorusharp/badge)](https://www.codefactor.io/repository/github/xwilarg/boorusharp)

# BooruSharp
BooruSharp is a C# library to browse Booru websites easily<br/>
You can download it from [NuGet](https://www.nuget.org/packages/BooruSharp):
```
Install-Package BooruSharp
```
```
Install-Package BooruSharp.Others
```
BooruSharp currently handle the following websites:
 - booru.allthefallen.moe
 - danbooru.donmai.us
 - e621.net
 - e926.net
 - furry.booru.org
 - gelbooru.com
 - konachan.com
 - lolibooru.moe
 - realbooru.com
 - rule34.xxx
 - safebooru.org
 - sakugabooru.com
 - beta.sankakucomplex.com
 - xbooru.com
 - yande.re
 
BooruSharp.Others allow to handle more websites that aren't booru:
 - pixiv.net

# Documentation

Version >= 2.0.0: https://boorusharp.zirk.eu/<br/>
Version <= 1.4.8: https://github.com/Xwilarg/BooruSharp/wiki<br/>
Transition from 1.4.8 to 2.0.0: https://boorusharp.zirk.eu/transition.html

# Features availability

| Booru | Multiple Random Images | Post by ID | Post by MD5 | Tag by ID | Comment API | Last Comments API | Wiki API | Related Tag API | Post Count API | Favorite API |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Atfbooru		| ✔️ | ✔️ | ✔️ | ✔️ | ✔️ | ❌ | ✔️ | ✔️ | ❌ | ❌ |
| Danbooru Donmai	| ✔️ | ✔️ | ✔️ | ✔️ | ✔️ | ❌ | ✔️ | ✔️ | ❌ | ❌ |
| E621			| ✔️ | ❌ | ✔️ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| E926			| ✔️ | ❌ | ✔️ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| Furry Booru		| ❌ | ✔️ | ❌ | ✔️ | ✔️ | ✔️ | ❌ | ❌ | ✔️ | ✔️ |
| Gelbooru		| ✔️ | ✔️ | ✔️ | ✔️ | ✔️ | ✔️ | ❌ | ❌ | ✔️ | ✔️ |
| Konachan		| ✔️ | ❌ | ❌ | ✔️ | ✔️ | ✔️ | ✔️ | ✔️ | ✔️ | ❌ |
| Lolibooru		| ✔️ | ❌ | ❌ | ✔️ | ❌ | ✔️ | ✔️ | ✔️ | ✔️ | ❌ |
| Realbooru		| ❌ | ✔️ | ❌ | ✔️ | ✔️ | ✔️ | ❌ | ❌ | ✔️ | ✔️ |
| Rule 34		| ❌ | ✔️ | ❌ | ✔️ | ❌ | ❌ | ❌ | ❌ | ✔️ | ✔️ |
| Safebooru		| ❌ | ✔️ | ❌ | ✔️ | ❌ | ❌ | ❌ | ❌ | ✔️ | ✔️ |
| Sakugabooru		| ✔️ | ❌ | ❌ | ✔️ | ✔️ | ❌ | ✔️ | ✔️ | ✔️ | ❌ |
| Sankaku Complex	| ✔️ | ❌ | ❌ | ❌ | ✔️ | ✔️ | ✔️ | ❌ | ❌ | ❌ |
| Xbooru		| ❌ | ✔️ | ❌ | ✔️ | ✔️ | ✔️ | ❌ | ❌ | ✔️ | ✔️ |
| Yandere		| ✔️ | ❌ | ❌ | ✔️ | ✔️ | ❌ | ✔️ | ✔️ | ✔️ | ❌ |
| Pixiv		| ❌ | ✔️ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✔️ | ✔️ |

# Examples

Random image:
```Cs
BooruSharp.Booru.Gelbooru booru = new BooruSharp.Booru.Gelbooru();
BooruSharp.Search.Post.SearchResult result = await booru.GetRandomPostAsync("hibiki_(kantai_collection)", "school_swimsuit");

Console.WriteLine("Image preview URL: " + result.previewUrl.AbsoluteUri + Environment.NewLine +
                  "Image URL: " + result.fileUrl.AbsoluteUri + Environment.NewLine +
                  "Image is safe: " + (result.rating == BooruSharp.Search.Post.Rating.Safe) + Environment.NewLine +
                  "Tags on the image: " + String.Join(", ", result.tags));
```

Get many random images at once:
```Cs
BooruSharp.Booru.SankakuComplex booru = new BooruSharp.Booru.SankakuComplex();
BooruSharp.Search.Post.SearchResult[] result = await booru.GetRandomPostsAsync(10, "ifrit_(arknights)", "silence_(arknights)");

Console.WriteLine(string.Join(Environment.NewLine, result.Select(x => "Random Image: " + x.fileUrl)));
```

Get tag:
```Cs
BooruSharp.Booru.Safebooru booru = new BooruSharp.Booru.Safebooru();
BooruSharp.Search.Tag.SearchResult result = await booru.GetTagAsync("cirno");

Console.WriteLine("Tag type: " + result.type + Environment.NewLine +
                  "ID: " + result.id);
```

Get Wiki entry:
```Cs
BooruSharp.Booru.Konachan booru = new BooruSharp.Booru.Konachan();
BooruSharp.Search.Wiki.SearchResult result = await booru.GetWikiAsync("loli");

Console.WriteLine("Description: " + result.body + Environment.NewLine +
                  "ID: " + result.id + Environment.NewLine +
                  "Created at: " + result.creation.ToString("dd/MM/yy HH:mm:ss") + Environment.NewLine +
                  "Last update at: " + result.lastUpdate.ToString("dd/MM/yy HH:mm:ss"));
```
Get related tags:
```Cs
BooruSharp.Booru.Yandere booru = new BooruSharp.Booru.Yandere();
BooruSharp.Search.Related.SearchResult[] results = await booru.GetRelatedAsync("see_through");

Console.WriteLine(String.Join(Environment.NewLine,
    results.Select(delegate (BooruSharp.Search.Related.SearchResult res) { return ("Name: " + res.name +" (" + res.count + ")"); })));
```
Get comments:
```Cs
BooruSharp.Booru.Lolibooru booru = new BooruSharp.Booru.Lolibooru();
BooruSharp.Search.Comment.SearchResult[] results = await booru.GetCommentAsync(134097);

Console.WriteLine(String.Join(Environment.NewLine,
    results.Select(delegate (BooruSharp.Search.Comment.SearchResult res) { return ("Author: " + res.authorName + ", the " + res.creation.ToString("dd/MM/yy HH:mm:ss") + " - " + res.body); })));
```
Add to favorite:
```Cs
BooruSharp.Booru.Safebooru booru = new BooruSharp.Booru.Safebooru();
booru.SetBooruAuth(new BooruSharp.Booru.BooruAuth("yourUserId", "yourPasswordHash")); // See https://boorusharp.zirk.eu/#authentification
await booru.AddFavoriteAsync(1759793);
```
Get all character tags containing a string:
```Cs
BooruSharp.Booru.Yandere yandere = new BooruSharp.Booru.Yandere();
BooruSharp.Search.Tag.SearchResult[] results = await yandere.GetTagsAsync("tsukiko");
Console.WriteLine(String.Join(Environment.NewLine,
	results.Where(delegate (BooruSharp.Search.Tag.SearchResult res) { return (res.type == BooruSharp.Search.Tag.TagType.Character); })
           .Select(delegate (BooruSharp.Search.Tag.SearchResult res) { return (res.name); })));
```

# Want to contribute?
Feel free to open a [pull request](https://github.com/Xwilarg/BooruSharp/pulls).

# Need more help?
Feel free to [open an issue](https://github.com/Xwilarg/BooruSharp/issues) or come ask [on Discord](https://discord.gg/H6wMRYV).
