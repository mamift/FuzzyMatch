using NUnit.Framework;
using System.Diagnostics;
using System.Linq;

namespace FuzzyMatch.Tests
{
    [TestFixture]
    public class FuzzyMatcherTests
    {
        [Test]
        public void FuzzyMatcherInstanceTest()
        {
            FuzzyMatcher instance = new FuzzyMatcher(TestUtilities.DictionaryWords);
            
            var results = instance.FuzzyMatch("corvus", true);

            Assert.IsNotEmpty(results);
            Assert.AreNotSame(results.Count, TestUtilities.DictionaryWords.Count);
            Assert.IsTrue(results.Count == 12);
        }

        [Test]
        public void FuzzyMatcherIncludeNonMatchingResultsTest()
        {
            var instance = new FuzzyMatcher(TestUtilities.DictionaryWords);

            var results = instance.FuzzyMatch("ascendi", includeNonMatching: true);

            Assert.IsNotEmpty(results);
            Assert.IsTrue(results.All(r => r.Score > 0));
            Assert.AreNotSame(results.Count, TestUtilities.DictionaryWords.Count);
            Assert.IsTrue(results.Count == 25768);
        }

        [Test]
        public void FuzzyMatcherLinqTest()
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

        [Test]
        public void LinqBenchmarkTest()
        {
            var averageOfNoFormatting = LinqBenchmark();
            var averageOfFormatting = LinqBenchmark(formatString: true);

            Assert.AreNotSame(averageOfFormatting, averageOfNoFormatting);
            Assert.IsTrue(averageOfFormatting > averageOfNoFormatting);
        }

        private static float LinqBenchmark(int iterations = 10, bool formatString = false, bool includeOriginal = true)
        {
            var linqResults = new float[iterations];
            var words = TestUtilities.DictionaryWords;

            for (int i = 0; i < iterations; i++)
            {
                var stopWatch1 = new Stopwatch();
                stopWatch1.Start();

                var linqWay = words.Select(w => FuzzyMatcher.FuzzyMatch(w, "corvus", formatString, includeOriginal))
                                   .Where(r => r.DidMatch).ToList();

                stopWatch1.Stop();
                Debug.WriteLine($"Time for iteration {i + 1}: {stopWatch1.ElapsedMilliseconds}");

                linqResults[i] = stopWatch1.ElapsedMilliseconds;
            }

            return linqResults.Average();
        }

        [Test]
        public void InstanceBenchmarkTest()
        {
            var averageOfNoFormatting = InstanceBenchmark();
            var averageOfFormatting = InstanceBenchmark(formatString: true);

            Assert.AreNotSame(averageOfFormatting, averageOfNoFormatting);
            Assert.IsTrue(averageOfFormatting > averageOfNoFormatting);
        }

        private static float InstanceBenchmark(int iterations = 10, bool formatString = false, bool includeOriginal = true)
        {
            var instanceResult = new float[iterations];
            var words = TestUtilities.DictionaryWords;

            for (int i = 0; i < iterations; i++)
            {
                var stopWatch2 = new Stopwatch();
                stopWatch2.Start();

                var instance = new FuzzyMatcher(words);
                var results = instance.FuzzyMatch("corvus", formatString, includeOriginal);

                stopWatch2.Stop();
                Debug.WriteLine($"Time for iteration {i + 1}: {stopWatch2.ElapsedMilliseconds}");
                instanceResult[i] = stopWatch2.ElapsedMilliseconds;
            }

            return instanceResult.Average();
        }
    }
}