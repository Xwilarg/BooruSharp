| BooruSharp     | BooruSharp.Others  |
| -------------- | ------------ |
| [![NuGet](https://img.shields.io/nuget/v/BooruSharp.svg)](https://www.nuget.org/packages/BooruSharp/) | [![NuGet](https://img.shields.io/nuget/v/BooruSharp.Others.svg)](https://www.nuget.org/packages/BooruSharp.Others/) |
| ![Nuget](https://img.shields.io/nuget/dt/BooruSharp) | ![Nuget](https://img.shields.io/nuget/dt/BooruSharp.Others) |

| CI | Code Quality | Coverage |
| -- | ------------ | -------- |
| [![Build (GitHub CI)](https://github.com/Xwilarg/BooruSharp/workflows/.NET%20Core/badge.svg)](https://github.com/Xwilarg/BooruSharp/actions) | [![Codacy Badge](https://app.codacy.com/project/badge/Grade/07be9e8c69cd4d87b20987b9fcec7a0e)](https://www.codacy.com/manual/Xwilarg/BooruSharp?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Xwilarg/BooruSharp&amp;utm_campaign=Badge_Grade) | [![Codacy Badge](https://app.codacy.com/project/badge/Coverage/07be9e8c69cd4d87b20987b9fcec7a0e)](https://www.codacy.com/manual/Xwilarg/BooruSharp?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Xwilarg/BooruSharp&amp;utm_campaign=Badge_Coverage) |

# BooruSharp
BooruSharp is a C# library to browse Booru websites easily<br/>
You can download it from [NuGet](https://www.nuget.org/packages/BooruSharp):
```powershell
Install-Package BooruSharp
```
```powershell
Install-Package BooruSharp.Others
```
BooruSharp currently handle the following websites:
 - booru.allthefallen.moe
 - danbooru.donmai.us
 - e621.net
 - e926.net
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
 - www.furaffinity.net

## Documentation
Documentation for version > 2.0.0 isn't available for now\
Please either refer to the examples below or use the XML documentation in the code\
If you have any question, feel free to [contact me](#need-more-help)

## Features availability

| Booru | Multiple Random Images | Post by ID | Post by MD5 | Tag by ID | Comment API | Last Comments API | Wiki API | Related Tag API | Post Count API | Favorite API |
| ---                   | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| Atfbooru		| ✔️ | ✔️ | ✔️ | ✔️ | ✔️ | ❌ | ✔️ | ✔️ | ❌ | ❌ |
| Danbooru Donmai	| ✔️ | ✔️ | ✔️ | ✔️ | ✔️ | ❌ | ✔️ | ✔️ | ❌ | ❌ |
| E621			| ✔️ | ✔️ | ✔️ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
| E926			| ✔️ | ✔️ | ✔️ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |
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
| Pixiv		        | ❌ | ✔️ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✔️ | ✔️ |
| Furaffinity   | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ |

## Examples

### Random image
```Cs
var booru = new BooruSharp.Booru.Gelbooru();
var result = await booru.GetRandomPostAsync("hibiki_(kantai_collection)", "school_swimsuit");

Console.WriteLine("Image preview URL: " + result.PreviewUrl.AbsoluteUri + Environment.NewLine +
                  "Image URL: " + result.FileUrl.AbsoluteUri + Environment.NewLine +
                  "Image is safe: " + (result.Rating == BooruSharp.Search.Post.Rating.Safe) + Environment.NewLine +
                  "Tags on the image: " + String.Join(", ", result.Tags));
```

### Random image (Pixiv)
```Cs
var booru = new BooruSharp.Others.Pixiv();
await booru.LoginAsync("[refreshToken]"); // See https://github.com/Xwilarg/BooruSharp/#pixiv
var result = await booru.GetRandomPostAsync("シンボリルドルフ(ウマ娘)");
```

### Random image (Furaffinity)
```Cs
var booru = new BooruSharp.Others.Furaffinity();
await booru.LoginAsync("[cookie a]", "[cookie b]"); // See https://github.com/Xwilarg/BooruSharp/#furaffinity
var result = await booru.GetRandomPostAsync("Dragon");
```

### Many random images at once
```Cs
var booru = new BooruSharp.Booru.SankakuComplex();
var results = await booru.GetRandomPostsAsync(10, "ifrit_(arknights)", "silence_(arknights)");

Console.WriteLine(string.Join(Environment.NewLine, results.Select(x => "Random Image: " + x.FileUrl)));
```

### Get tag
```Cs
var booru = new BooruSharp.Booru.Safebooru();
var result = await booru.GetTagAsync("cirno");

Console.WriteLine("Tag type: " + result.Type + Environment.NewLine +
                  "ID: " + result.ID);
```

### Get Wiki entry
```Cs
var booru = new BooruSharp.Booru.Konachan();
var result = await booru.GetWikiAsync("loli");

Console.WriteLine("Description: " + result.Body + Environment.NewLine +
                  "ID: " + result.ID + Environment.NewLine +
                  "Created at: " + result.Creation.ToString("dd/MM/yy HH:mm:ss") + Environment.NewLine +
                  "Last update at: " + result.LastUpdate.ToString("dd/MM/yy HH:mm:ss"));
```
### Get related tags
```Cs
var booru = new BooruSharp.Booru.Yandere();
var results = await booru.GetRelatedAsync("see_through");

Console.WriteLine(String.Join(Environment.NewLine,
    results.Select(delegate (BooruSharp.Search.Related.SearchResult res) { return ("Name: " + res.Name +" (" + res.Count + ")"); })));
```
### Get comments
```Cs
var booru = new BooruSharp.Booru.Lolibooru();
var results = await booru.GetCommentAsync(134097);

Console.WriteLine(String.Join(Environment.NewLine,
    results.Select(delegate (BooruSharp.Search.Comment.SearchResult res) { return ("Author: " + res.AuthorName + ", the " + res.Creation.ToString("dd/MM/yy HH:mm:ss") + " - " + res.Body); })));
```
### Add to favorite
```Cs
var booru = new BooruSharp.Booru.Safebooru();
booru.SetBooruAuth(new BooruSharp.Booru.BooruAuth("yourUserId", "yourPasswordHash")); // See https://github.com/Xwilarg/BooruSharp/#booru
await booru.AddFavoriteAsync(1759793);
```
### Get all character tags containing a string
```Cs
var yandere = new BooruSharp.Booru.Yandere();
var results = await yandere.GetTagsAsync("tsukiko");
Console.WriteLine(String.Join(Environment.NewLine,
	results.Where(delegate (BooruSharp.Search.Tag.SearchResult res) { return (res.Type == BooruSharp.Search.Tag.TagType.Character); })
           .Select(delegate (BooruSharp.Search.Tag.SearchResult res) { return (res.Name); })));
```

## Authentification

### Booru
For booru authentification, you'll need your user id and your password hash\
To get it, I advise you to go on an image, open the developer panel (F12) and go in "Network"\
Then press the button to add the image to your favorite and look at the "Cookies" section of the last request
![Authentification](Preview/Auth.png)

### Pixiv
For Pixiv authentification, please use LoginAsync with a refresh token\
To get your refresh token, you can follow this tutorial: https://gist.github.com/ZipFile/c9ebedb224406f4f11845ab700124362

### Furaffinity
For Furaffinity, you need to call LoginAsync
To have your cookieA and cookieB values, do the same as Booru but get the "a" and "b" values in the "Cookie" section instead

## Want to contribute
Feel free to open a [pull request](https://github.com/Xwilarg/BooruSharp/pulls).

## Need more help
Feel free to [open an issue](https://github.com/Xwilarg/BooruSharp/issues) or come ask [on Discord](https://discord.gg/H6wMRYV).
