using NUnit.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NUnit.Framework.Internal;

namespace FuzzyMatch.Tests
{
    [TestFixture]
    public class FuzzyMatcherTests
    {
        [Test]
        public void FuzzyMatcherInstanceTest()
        {
            var newInstance = new FuzzyMatcher(TestUtilities.DictionaryWords);

            var stopWatch = new Stopwatch();
            stopWatch.Start();

            var results = newInstance.FuzzyMatch("corvus", true);

            stopWatch.Stop();
            Debug.WriteLine($"Time taken: {stopWatch.ElapsedMilliseconds}");

            Assert.IsNotEmpty(results);
            Assert.IsTrue(results.Count == 12);
        }

        [Test]
        public void BenchmarkTest()
        {
            var linqResults = new float[10];
            var instanceResult = new float[10];

            for (int i = 0; i < 10; i++)
            {
                var stopWatch1 = new Stopwatch();
                stopWatch1.Start();
            
                var linqWay = TestUtilities.DictionaryWords.Select(w => FuzzyMatcher.FuzzyMatch(w, "corvus", true))
                                           .Where(r => r.DidMatch).ToList();

                stopWatch1.Stop();
                Debug.WriteLine($"Time: {stopWatch1.ElapsedMilliseconds}");

                linqResults[i] = stopWatch1.ElapsedMilliseconds;
            }

            for (int i = 0; i < 10; i++)
            {
                var stopWatch2 = new Stopwatch();
                stopWatch2.Start();
            
                var instance = new FuzzyMatcher(TestUtilities.DictionaryWords);
                var results = instance.FuzzyMatch("corvus", true);

                stopWatch2.Stop();
                Debug.WriteLine($"Time: {stopWatch2.ElapsedMilliseconds}");
                instanceResult[i] = stopWatch2.ElapsedMilliseconds;
            }
        }
    }
}