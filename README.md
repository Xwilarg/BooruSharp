[![Build status](https://ci.appveyor.com/api/projects/status/n503kvx6b0a45bcv/branch/master?svg=true)](https://ci.appveyor.com/project/Xwilarg/boorusharp/branch/master)
[![NuGet](https://img.shields.io/nuget/v/BooruSharp.svg)](https://www.nuget.org/packages/BooruSharp/)
[![codecov](https://codecov.io/gh/Xwilarg/BooruSharp/branch/master/graph/badge.svg)](https://codecov.io/gh/Xwilarg/BooruSharp)
[![Unit tests](https://img.shields.io/appveyor/tests/xwilarg/boorusharp.svg)](https://ci.appveyor.com/project/Xwilarg/boorusharp/branch/master/tests)
[![CodeFactor](https://www.codefactor.io/repository/github/xwilarg/boorusharp/badge)](https://www.codefactor.io/repository/github/xwilarg/boorusharp)
![Nuget](https://img.shields.io/nuget/dt/BooruSharp)

# BooruSharp
BooruSharp is a C# library to browse Booru websites easily<br/>
You can download it from [NuGet](https://www.nuget.org/packages/BooruSharp):
```
Install-Package BooruSharp
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
 - xbooru.com
 - yande.re

# Basic examples

Random image:
```Cs
BooruSharp.Booru.Gelbooru booru = new BooruSharp.Booru.Gelbooru();
BooruSharp.Search.Post.SearchResult result = await booru.GetRandomImage("hibiki_(kantai_collection)", "school_swimsuit");

Console.WriteLine("Image preview URL: " + result.previewUrl + Environment.NewLine +
                  "Image URL: " + result.fileUrl + Environment.NewLine +
                  "Image is safe: " + (result.rating == BooruSharp.Search.Post.Rating.Safe) + Environment.NewLine +
                  "Tags on the image: " + String.Join(", ", result.tags));
```

Get tag:
```Cs
BooruSharp.Booru.Safebooru booru = new BooruSharp.Booru.Safebooru();
BooruSharp.Search.Tag.SearchResult result = await booru.GetTag("cirno");

Console.WriteLine("Tag type: " + result.type + Environment.NewLine +
                  "ID: " + result.id);
```

Get Wiki entry:
```Cs
BooruSharp.Booru.Konachan booru = new BooruSharp.Booru.Konachan();
BooruSharp.Search.Wiki.SearchResult result = await booru.GetWiki("loli");

Console.WriteLine("Description: " + result.body + Environment.NewLine +
                  "ID: " + result.id + Environment.NewLine +
                  "Created at: " + result.creation.ToString("dd/MM/yy HH:mm:ss") + Environment.NewLine +
                  "Last update at: " + result.lastUpdate.ToString("dd/MM/yy HH:mm:ss"));
```
Get related tags:
```Cs
BooruSharp.Booru.Yandere booru = new BooruSharp.Booru.Yandere();
BooruSharp.Search.Related.SearchResult[] results = await booru.GetRelated("see_through");

Console.WriteLine(String.Join(Environment.NewLine,
    results.Select(delegate (BooruSharp.Search.Related.SearchResult res) { return ("Name: " + res.name +" (" + res.count + ")"); })));
```
Get comments:
```Cs
BooruSharp.Booru.Lolibooru booru = new BooruSharp.Booru.Lolibooru();
BooruSharp.Search.Comment.SearchResult[] results = await booru.GetComment(134097);

Console.WriteLine(String.Join(Environment.NewLine,
    results.Select(delegate (BooruSharp.Search.Comment.SearchResult res) { return ("Author: " + res.authorName + ", the " + res.creation.ToString("dd/MM/yy HH:mm:ss") + " - " + res.body); })));
```

# Advanced examples

Get all character tags containing a string:
```Cs
BooruSharp.Booru.Yandere yandere = new BooruSharp.Booru.Yandere();
BooruSharp.Search.Tag.SearchResult[] results = await yandere.GetTags("tsukiko");
Console.WriteLine(String.Join(Environment.NewLine,
	results.Where(delegate (BooruSharp.Search.Tag.SearchResult res) { return (res.type == BooruSharp.Search.Tag.TagType.Character); })
           .Select(delegate (BooruSharp.Search.Tag.SearchResult res) { return (res.name); })));
```
