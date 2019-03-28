# FuzzyMatch
A C# library that attempts to implement Sublime Text's fuzzy matching algorithm.

**Targets.NET Standard 1.0+**, which is.NET **Framework 4.5,.NET Core 1.0+, Mono 4.6**, according to [Microsoft](https://docs.microsoft.com/en-us/dotnet/standard/net-standard).

This library implements the code described on this Medium article:
https://medium.com/forrest-the-woods/reverse-engineering-sublime-text-s-fuzzy-match-4cffeed33fdb

Specifically it's a C# port of the C++ code located here: https://github.com/forrestthewoods/lib_fts/blob/master/code/fts_fuzzy_match.h

## Usage:
The library provides an instance API (instantiate a new instance of ```FuzzyMatcher```) and a few static methods:

### Class API
```C#
var words = new []{ "abaisance", "abaised", "abaiser", "abaisse", "abaissed", "abaka" };
FuzzyMatcher instance = new FuzzyMatcher(words);
var matchResults = instance.FuzzyMatch("aba");
```
### Static API
```C#
var results = words.Select(word => FuzzyMatcher.FuzzyMatch(word, "aba", true)).Where(results => results.DidMatch || results.Score > 0).ToList();
```

### Additional APIs over existing code

### Development
Requires Visual Studio 2017 Community Edition and .NET Core v1.0 or v2.0 support installed.

This repo also includes a unit test project, which uses NUnit.

## License
Licensed under MIT (the original C++ code was public domain).