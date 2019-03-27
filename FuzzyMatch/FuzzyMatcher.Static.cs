using System;
using System.Collections.Generic;
using System.Linq;

namespace FuzzyMatch
{
    public partial class FuzzyMatcher
    {
        // Score consts
        private const int AdjacencyBonus = 5; // bonus for adjacent matches
        private const int SeparatorBonus = 10; // bonus if match occurs after a separator
        private const int CamelBonus = 10; // bonus if match is uppercase and prev is lower

        private const int LeadingLetterPenalty = -3; // penalty applied for every letter in searchedString before the first match

        private const int MaxLeadingLetterPenalty = -9; // maximum penalty for leading letters
        private const int UnmatchedLetterPenalty = -1; // penalty for every letter that doesn't matter

        /// <summary>
        /// Does a simple fuzzy search for a <paramref name="pattern"/> within a string.
        /// <para>This is a very simple form of fuzzy-matching and does not anything beyond a boolean indicating the matching succeeded or failed.</para>
        /// </summary>
        /// <param name="stringToSearch">The string to search for the pattern in.</param>
        /// <param name="pattern">The pattern to search for in the string.</param>
        /// <returns>true if each character in pattern is found sequentially within the <paramref name="stringToSearch"/>; otherwise, false.</returns>
        public static bool SimpleFuzzyMatch(string stringToSearch, string pattern)
        {
            var patternIdx = 0;
            var strIdx = 0;
            var patternLength = pattern.Length;
            var strLength = stringToSearch.Length;

            while (patternIdx != patternLength && strIdx != strLength)
            {
                if (char.ToLower(pattern[patternIdx]) == char.ToLower(stringToSearch[strIdx]))
                    ++patternIdx;
                ++strIdx;
            }

            return patternLength != 0 && strLength != 0 && patternIdx == patternLength;
        }

        /// <summary>
        /// Does a more complex and Sublime Text-like fuzzy search for a pattern within a string, and gives the search a score on how well it matched.
        /// </summary>
        /// <param name="stringToSearch">The string to search for the pattern in.</param>
        /// <param name="pattern">The pattern to search for in the string.</param>
        /// <param name="formatString">Set to <c>true</c> to format the string returned in the <see cref="FuzzyMatchResult"/> return value by enclosing it in braces.
        /// <para>Skipping formatting saves CPU cycles, especially when using this method on a large list of strings.</para></param>
        /// <param name="includeOriginalStringInResult">When <c>true</c>, this will include the <paramref name="stringToSearch"/> in the returned result.
        /// Is useful when multiple strings are searched for the same pattern and you wish to build a sequence of <see cref="FuzzyMatchResult"/> objects.</param>
        /// <returns>Returns the details of the fuzzy match as a <see cref="FuzzyMatchResult"/> object.</returns>
        public static FuzzyMatchResult FuzzyMatch(string stringToSearch, string pattern, bool formatString = false, 
            bool includeOriginalStringInResult = true)
        {
            // Loop variables
            var score = 0;
            var patternIdx = 0;
            var patternLength = pattern.Length;
            var strIdx = 0;
            var strLength = stringToSearch.Length;
            var prevMatched = false;
            var prevLower = false;
            var prevSeparator = true; // true if first letter match gets separator bonus

            // Use "best" matched letter if multiple string letters match the pattern
            char? bestLetter = null;
            char? bestLower = null;
            int? bestLetterIdx = null;
            var bestLetterScore = 0;

            var matchedIndices = new List<int>();

            // Loop over strings
            while (strIdx != strLength)
            {
                var patternChar = patternIdx != patternLength ? pattern[patternIdx] as char? : null;
                var strChar = stringToSearch[strIdx];

                var patternLower = patternChar != null ? char.ToLower((char) patternChar) as char? : null;
                var strLower = char.ToLower(strChar);
                var strUpper = char.ToUpper(strChar);

                var nextMatch = patternChar != null && patternLower == strLower;
                var rematch = bestLetter != null && bestLower == strLower;

                var advanced = nextMatch && bestLetter != null;
                var patternRepeat = bestLetter != null && patternChar != null && bestLower == patternLower;

                if (advanced || patternRepeat)
                {
                    score += bestLetterScore;
                    matchedIndices.Add((int) bestLetterIdx);
                    bestLetter = null;
                    bestLower = null;
                    bestLetterIdx = null;
                    bestLetterScore = 0;
                }

                if (nextMatch || rematch)
                {
                    var newScore = 0;

                    // Apply penalty for each letter before the first pattern match
                    // Note: Math.Max because penalties are negative values. So max is smallest penalty.
                    if (patternIdx == 0)
                    {
                        var penalty = Math.Max(strIdx * LeadingLetterPenalty, MaxLeadingLetterPenalty);
                        score += penalty;
                    }

                    // Apply bonus for consecutive bonuses
                    if (prevMatched)
                        newScore += AdjacencyBonus;

                    // Apply bonus for matches after a separator
                    if (prevSeparator)
                        newScore += SeparatorBonus;

                    // Apply bonus across camel case boundaries. Includes "clever" isLetter check.
                    if (prevLower && strChar == strUpper && strLower != strUpper)
                        newScore += CamelBonus;

                    // Update pattern index IF the next pattern letter was matched
                    if (nextMatch)
                        ++patternIdx;

                    // Update best letter in searchedString which may be for a "next" letter or a "rematch"
                    if (newScore >= bestLetterScore)
                    {
                        // Apply penalty for now skipped letter
                        if (bestLetter != null)
                            score += UnmatchedLetterPenalty;

                        bestLetter = strChar;
                        bestLower = char.ToLower((char) bestLetter);
                        bestLetterIdx = strIdx;
                        bestLetterScore = newScore;
                    }

                    prevMatched = true;
                }
                else
                {
                    score += UnmatchedLetterPenalty;
                    prevMatched = false;
                }

                // Includes "clever" isLetter check.
                prevLower = strChar == strLower && strLower != strUpper;
                prevSeparator = strChar == '_' || strChar == ' ';

                ++strIdx;
            }

            // Apply score for last match
            if (bestLetter != null)
            {
                score += bestLetterScore;

                var bestLetterIndexVal = bestLetterIdx.GetValueOrDefault();
                matchedIndices.Add(bestLetterIndexVal);
            }

            // formats the string; skip if necessary
            var formattedString = formatString
                ? FormatStringToEncloseMatches(stringToSearch, matchedIndices)
                : string.Empty; 

            return new FuzzyMatchResult
            {
                Score = score,
                MatchedIndices = matchedIndices.ToArray(),
                FormattedString = formattedString,
                OriginalString = includeOriginalStringInResult ? stringToSearch : string.Empty,
                DidMatch = patternIdx == patternLength
            };
        }

        /// <summary>
        /// Does a more complex, Sublime Text-like fuzzy search for a <paramref name="pattern"/> within a <paramref name="stringToSearch"/>,
        /// using a given <paramref name="formatterFunc"/> to format the string.
        /// <para>An overload of <see cref="FuzzyMatch(string,string,bool,bool)"/>.</para>
        /// </summary>
        /// <param name="stringToSearch">The string to search for the pattern in.</param>
        /// <param name="pattern">The pattern to search for in the string.</param>
        /// <param name="formatterFunc">Pass a custom function to format the string using custom logic.</param>
        /// <param name="includeOriginalStringInResult">When <c>true</c>, this will include the <paramref name="stringToSearch"/> in the returned result. Is useful when
        /// multiple strings are searched for the same pattern and you wish to build a sequence of <see cref="FuzzyMatchResult"/> objects.</param>
        /// <returns></returns>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="formatterFunc"/> is <see langword="null"/></exception>
        public static FuzzyMatchResult FuzzyMatch(string stringToSearch, string pattern,
            Func<string, IEnumerable<int>, string> formatterFunc, bool includeOriginalStringInResult = false)
        {
            if (formatterFunc == null) throw new ArgumentNullException(nameof(formatterFunc));

            var result = FuzzyMatch(stringToSearch, pattern, false, includeOriginalStringInResult);

            result.FormattedString = formatterFunc(stringToSearch, result.MatchedIndices);

            return result;
        }

        /// <summary>
        /// Formats a given <paramref name="searchedString"/> using a given list of matched indices. Used by the static <see cref="FuzzyMatch(string,string,bool,bool)"/>
        /// method to format strings.
        /// </summary>
        /// <param name="searchedString">The string that was searched</param>
        /// <param name="matchedIndices">The list of integers, whereby each integer specifies the index in the string of a matching character</param>
        /// <param name="opener">Specify a custom opening character to enclose matching characters found in the search. Default is '{'</param>
        /// <param name="closer">Specify a custom closing character to enclose matching characters found in the search. Default is '}'</param>
        /// <returns></returns>
        public static string FormatStringToEncloseMatches(string searchedString, IEnumerable<int> matchedIndices,
            string opener = "{", string closer = "}")
        {
            var insertedCount = -1; /* counts the number of times an extra char was inserted, so that subsequent insertions 
                                      will place the inserted character at the right spot. Starts at -1
                                      in case the first char of a string is matched. */

            return matchedIndices.Aggregate(seed: searchedString,
                func: (current, matchedIndex) => current.Insert(matchedIndex + (insertedCount += opener.Length), opener)
                                                        .Insert(matchedIndex + (insertedCount += closer.Length) + 1, closer));
        }
    }
}
