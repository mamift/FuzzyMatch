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
            var words = TestUtilities.DictionaryWords;
            
            Assert.IsNotEmpty(words);
            Assert.IsTrue(words.Count == 466547);
        }
    }
}