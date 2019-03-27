using System.Diagnostics;

namespace FuzzyMatch
{
    /// <summary>
    /// Contains the results of a fuzzy-match. The return type of <see cref="FuzzyMatcher.FuzzyMatch(string,string,bool,bool)"/>.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public struct FuzzyMatchResult
    {
        /// <summary>
        /// The score assigned to the fuzzy match operation.
        /// </summary>
        public int Score { get; set; }

        /// <summary>
        /// The list of character indices (0-based) that matched.
        /// </summary>
        public int[] MatchedIndices { get; set; }

        /// <summary>
        /// If the option was specified, then the string will be formatted indicating which characters where matched.
        /// <para><seealso cref="FuzzyMatcher.FuzzyMatch(string,string,bool)"/>.</para>
        /// </summary>
        public string FormattedString { get; set; }

        /// <summary>
        /// The original string that was searched.
        /// </summary>
        public string OriginalString { get; set; }

        /// <summary>
        /// If the fuzzy-match operation succeeded.
        /// </summary>
        public bool DidMatch { get; set; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                var debugDisplayStr = $"Score = {Score}, DidMatch = {DidMatch}";
                if (string.IsNullOrWhiteSpace(FormattedString)) return debugDisplayStr;

                return $"{debugDisplayStr}, FormattedString = {FormattedString}";
            }
        }
    }
}