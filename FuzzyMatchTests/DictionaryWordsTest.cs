using NUnit.Framework;

namespace FuzzyMatch.Tests
{
    [TestFixture]
    public class DictionaryWordsTests
    {
        [Test]
        public void DictionaryWordsDeserialisedTest()
        {
            Assert.DoesNotThrow(() =>
            {
                var words = TestUtilities.DeserialiseDictionaryWords();
            });
        }

        [Test]
        public void DictionaryWordsAreCorrectCountTest()
        {
            Assert.IsNotEmpty(TestUtilities.DictionaryWords);
            Assert.IsTrue(TestUtilities.DictionaryWords.Count == 466547);
        }
    }
}