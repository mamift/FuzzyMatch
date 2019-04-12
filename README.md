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

### Additional features
This library also includes additional APIs for formatting the string to indicate which part of the string was matched.
```C#
// formats a string by enclosing matched characters inside parentheses
Func<string, IEnumerable<int>, string> formatterFunc = (str, indices) =>
{
    var insertedCount = 0;
    return indices.Aggregate(str, (theStr, theIndex) => 
                theStr.Insert(theIndex + (insertedCount++), "(")
                      .Insert(theIndex + (insertedCount++) + 1, ")"));
};

var test = FuzzyMatcher.FuzzyMatch(stringToSearch: "plantagenet",
                                   pattern: "planet",
                                   formatterFunc: formatterFunc);
```
The ```test``` variable results in:
```JSON
{
    "DidMatch": true,
    "FormattedString": "(P)(l)(a)(n)tagen(e)(t)",
    "MatchedIndices": [0, 1, 2, 3, 9, 10],
    "OriginalString": "",
    "Score": 25
}
```

### Additional APIs over existing code

### Development
Requires Visual Studio 2017 Community Edition and .NET Core v1.0 or v2.0 support installed.

This repo also includes a unit test project, which uses NUnit.

## License
Licensed under MIT (the original C++ code was public domain).