using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace FuzzyMatch
{
    /// <summary>
    /// Defines a class that allows searching a sequence of strings using a fuzzy-matching algorithm.
    /// </summary>
    /// <remarks>
    /// The fuzzy-matching algorithm is adapted from https://medium.com/forrest-the-woods/reverse-engineering-sublime-text-s-fuzzy-match-4cffeed33fdb
    /// </remarks>
    public partial class FuzzyMatcher
    {
        /// <summary>
        /// The list of possible <see cref="string"/>s to fuzzy match against.
        /// </summary>
        public List<string> SearchStrings { get; }

        /// <summary>
        /// Creates a new instance with a given sequence of strings.
        /// </summary>
        /// <param name="stringsToFuzzyMatchAgainst"></param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="stringsToFuzzyMatchAgainst"/> is <see langword="null"/></exception>
        /// <exception cref="T:System.NotSupportedException"><paramref name="stringsToFuzzyMatchAgainst"/> is empty</exception>
        public FuzzyMatcher(IEnumerable<string> stringsToFuzzyMatchAgainst)
        {
            if (stringsToFuzzyMatchAgainst == null) 
                throw new ArgumentNullException(nameof(stringsToFuzzyMatchAgainst));

            SearchStrings = stringsToFuzzyMatchAgainst.ToList();
            if (SearchStrings.Count == 0) 
                throw new NotSupportedException($"Creating an instance of {nameof(FuzzyMatcher)} class requires a non-empty sequence of strings!");
        }

        /// <summary>
        /// Fuzzy match against all the loaded <see cref="SearchStrings"/> with a given <see cref="pattern"/>.
        /// <para>Results in the list are not sorted.</para>
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="formatString">Set to <c>true</c> to format a string that indicates which characters matched
        /// the fuzzy search.
        /// <para><c>true</c> will slow down the search on a large count of <see cref="SearchStrings"/>.</para>
        /// </param>
        /// <param name="includeOriginalStringInResult">When <c>false</c>, omits the original string in the <see cref="FuzzyMatchResult"/>.</param>
        /// <returns></returns>
        [SuppressMessage("ReSharper", "LoopCanBeConvertedToQuery")]
        [SuppressMessage("ReSharper", "ForCanBeConvertedToForeach")]
        public List<FuzzyMatchResult> FuzzyMatch(string pattern, bool formatString = false, bool includeOriginalStringInResult = true)
        {
            var matches = new List<FuzzyMatchResult>(SearchStrings.Count);

            for (var i = 0; i < SearchStrings.Count; i++)
            {
                var result = FuzzyMatch(SearchStrings[i], pattern, formatString, includeOriginalStringInResult);
                if (result.DidMatch) matches.Add(result);
            }

            return matches;
        }
    }
}