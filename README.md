[![Build status](https://ci.appveyor.com/api/projects/status/n503kvx6b0a45bcv/branch/master?svg=true)](https://ci.appveyor.com/project/Xwilarg/boorusharp/branch/master)
[![codecov](https://img.shields.io/appveyor/tests/xwilarg/boorusharp.svg)](https://ci.appveyor.com/project/Xwilarg/boorusharp/build/tests)
[![codecov](https://codecov.io/gh/Xwilarg/BooruSharp/branch/master/graph/badge.svg)](https://codecov.io/gh/Xwilarg/BooruSharp)
[![CodeFactor](https://www.codefactor.io/repository/github/xwilarg/boorusharp/badge)](https://www.codefactor.io/repository/github/xwilarg/boorusharp)

# BooruSharp
BooruSharp is a C# library to browse Booru websites easily<br/>
BooruSharp currently handle the following websites:
 - e621.net
 - gelbooru.com
 - konachan.com
 - lolibooru.moe
 - rule34.xxx
 - safebooru.org
 - yande.re

# Examples

Random image:
```Cs
BooruSharp.Booru.Gelbooru booru = new BooruSharp.Booru.Gelbooru();
BooruSharp.ImageSearch.SearchResult result = booru.GetRandomImage("hibiki_(kantai_collection)", "school_swimsuit");

Console.WriteLine("Image preview URL: " + result.previewUrl + Environment.NewLine +
                  "Image URL: " + result.fileUrl + Environment.NewLine +
                  "Image is safe: " + (result.rating == BooruSharp.ImageSearch.Rating.Safe));
```
<br/>
Get tag:
```Cs
BooruSharp.Booru.Gelbooru booru = new BooruSharp.Booru.Gelbooru();
BooruSharp.TagSearch.SearchResult result = booru.GetTag("cirno");

Console.WriteLine("Tag type: " + result.type + Environment.NewLine +
                  "ID: " + result.id);
```