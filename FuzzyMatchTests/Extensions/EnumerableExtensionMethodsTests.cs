using System.Linq;
using NUnit.Framework;

namespace FuzzyMatch.Tests
{
    [TestFixture]
    public class EnumerableExtensionMethodsTests
    {
        [Test]
        public void GetBestMatchTest()
        {
            var corvus = "corvus";
            var matchResults = TestUtilities.DictionaryWords.Select(w => FuzzyMatcher.FuzzyMatch(w, corvus));
            var bestMatch = matchResults.GetBestMatch();

            Assert.IsNotNull(bestMatch);
            Assert.IsInstanceOf<FuzzyMatchResult>(bestMatch);

            Assert.IsTrue(bestMatch.DidMatch);
            Assert.IsTrue(bestMatch.MatchedIndices.Length == corvus.Length);
        }

        [Test]
        public void GetBestMatchesTest1()
        {
            var prude = "prude";
            var bestMatches = TestUtilities.DictionaryWords.Select(w => FuzzyMatcher.FuzzyMatch(w, prude)).GetBestMatches();

            Assert.IsTrue(bestMatches.Length == 1);
        }

        [Test]
        public void GetBestMatchesTest2()
        {
            var falcie = "fal";
            var bestMatches = TestUtilities.DictionaryWords.Select(w => FuzzyMatcher.FuzzyMatch(w, falcie)).GetBestMatches();

            Assert.IsTrue(bestMatches.Length == 8);
        }
    }
}