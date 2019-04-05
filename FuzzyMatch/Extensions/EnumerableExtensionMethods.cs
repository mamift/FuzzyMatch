using System;
using System.Collections.Generic;
using System.Linq;

namespace FuzzyMatch
{
    public static class EnumerableExtensionMethods
    {
        /// <summary>
        /// Returns the best match from the current sequence of <paramref name="results"/>.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="results"/> is <see langword="null"/></exception>
        public static FuzzyMatchResult GetBestMatch(this IEnumerable<FuzzyMatchResult> results) =>
            results.GetBestMatches()
                .OrderByDescending(bm => bm.Score)
                .ThenByDescending(obm => obm.MatchedIndices.Length)
                .FirstOrDefault();

        /// <summary>
        /// Returns an array of best matches from the current <paramref name="results"/>; where possible
        /// results have scores that are the highest in the sequence, or second highest.
        /// <para>Use this method for especially large sequences of <see cref="FuzzyMatchResult"/>s whereby multiple matches
        /// have the same highest score.</para>
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="results"/> is <see langword="null"/></exception>
        public static FuzzyMatchResult[] GetBestMatches(this IEnumerable<FuzzyMatchResult> results)
        {
            if (results == null) throw new ArgumentNullException(nameof(results));
            var matchResults = results as FuzzyMatchResult[] ?? results.ToArray();
            if (!matchResults.Any()) return default(FuzzyMatchResult[]);

            var matched = matchResults.Where(r => r.DidMatch).ToArray();
            int highestScore = matched.Max(r => r.Score), secondHighest = highestScore - 1;
            return matched.Where(m => m.Score == highestScore || m.Score == secondHighest).ToArray();
        }
    }
}