using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace FuzzyMatch.Tests
{
    [TestFixture]
    public class FuzzyMatcherStaticTests
    {
        private const string ReferenceString = "a big brown fox jumps over the lazy dog";

        private const string ReferencePattern = "bbr";

        [Test]
        public void SimpleFuzzyMatchTest()
        {
            var match = FuzzyMatcher.SimpleFuzzyMatch(ReferenceString, ReferencePattern);

            Assert.IsTrue(match);
        }

        [Test]
        public void FuzzyMatchWithScoreTest()
        {
            var match = FuzzyMatcher.FuzzyMatch(ReferenceString, ReferencePattern);

            Assert.IsInstanceOf<FuzzyMatchResult>(match);
            Assert.IsTrue(match.DidMatch);
            Assert.AreEqual(-16, match.Score);
        }

        [Test]
        public void FuzzyMatchWordsWithPartialSpellingTest()
        {
            var facility = TestUtilities.DictionaryWords[133956];
            var facilityPartialSpellingMatchTest = FuzzyMatcher.FuzzyMatch(facility, "failt");

            Assert.IsTrue(facilityPartialSpellingMatchTest.DidMatch);
            Assert.IsTrue(facilityPartialSpellingMatchTest.Score == 15);            
        }

        [Test]
        public void FuzzyMatchWithNoMatchAndSubZeroScoreTest()
        {
            var faciobrachial = TestUtilities.DictionaryWords[133963];
            var test = FuzzyMatcher.FuzzyMatch(faciobrachial, "yt");

            Assert.IsFalse(test.DidMatch);
            Assert.IsTrue(test.Score == -13);
        }

        [Test]
        public void FuzzyMatchFormattedConsecutiveStringTest()
        {
            var nescience = TestUtilities.DictionaryWords[252662];

            var test = FuzzyMatcher.FuzzyMatch(stringToSearch: nescience,
                                               pattern: "nes",
                                               formatString: true,
                                               includeOriginalStringInResult: true);

            Assert.IsTrue(test.DidMatch);
            Assert.IsTrue(test.Score == 15);
            Assert.IsTrue(test.FormattedString.Length > test.OriginalString.Length);
        }

        [Test]
        public void FuzzyMatchFormattedNonConsecutiveStringTest()
        {
            var nescience = TestUtilities.DictionaryWords[252662];

            var test = FuzzyMatcher.FuzzyMatch(stringToSearch: nescience,
                                               pattern: "neie",
                                               formatString: true);

            Assert.IsTrue(test.DidMatch);
            Assert.IsTrue(test.Score == 16);
            Assert.IsNotEmpty(test.FormattedString);
        }

        [Test]
        public void FuzzyMatchFormattedWithCustomFormatterFuncTest()
        {
            var plantagenet = TestUtilities.DictionaryWords[300000];

            Func<string, IEnumerable<int>, string> formatterFunc = (str, indices) =>
            {
                var insertedCount = 0;
                return indices.Aggregate(str, (theStr, theIndex) => theStr.Insert(theIndex + (insertedCount++), "(")
                                                                          .Insert(theIndex + (insertedCount++) + 1, ")"));
            };

            var test = FuzzyMatcher.FuzzyMatch(stringToSearch: plantagenet,
                                               pattern: "planet",
                                               formatterFunc: formatterFunc);

            Assert.IsTrue(test.DidMatch);
            Assert.IsTrue(test.Score == 25);
            Assert.IsTrue(test.MatchedIndices.Length == 6);
            var parenthesesCount = test.FormattedString.Count(c => c == '(' || c == ')');
            Assert.IsTrue(parenthesesCount == 12);
        }

        [Test]
        public void FuzzyMatchDidNotMatchButScoreAboveZeroTest()
        {
            var plantaginaceae = TestUtilities.DictionaryWords[300001];
            
            var test = FuzzyMatcher.FuzzyMatch(plantaginaceae, "planet", true);

            Assert.IsFalse(test.DidMatch);
            Assert.IsTrue(test.MatchedIndices.Length == 5);
            Assert.IsTrue(test.Score > 0);
            Assert.IsTrue(test.Score == 17);
        }

        [Test]
        public void FuzzyMatchUsingLinqTest1()
        {
            var results = TestUtilities.DictionaryWords
                .Select(w => FuzzyMatcher.FuzzyMatch(w, "fal"))
                .Where(r => r.DidMatch)
                .ToList();

            Assert.IsNotEmpty(results);
            Assert.IsTrue(results.Count == 4956);
        }
        
        [Test]
        public void FuzzyMatcherUsingLinqTest2()
        {
            var results = TestUtilities
                          .DictionaryWords
                          .Select(w => FuzzyMatcher.FuzzyMatch(w, "involute", true))
                          .Where(r => r.DidMatch || r.Score > 0)
                          .ToList();

            Assert.IsNotEmpty(results);
            Assert.IsTrue(results.Count == 13839);

            var didMatch = results.Where(r => r.DidMatch).ToList();

            Assert.IsNotEmpty(didMatch);
            Assert.IsTrue(didMatch.Count == 16);
        }
    }
}