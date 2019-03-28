using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace FuzzyMatch.Tests
{
    [TestFixture]
    public class ReflectionAnalysis
    {
        [Test]
        public void MembersOfTheFuzzyMatcherClassHaveAnAssociatedTestMethodTest()
        {
            var fuzzyMatcherClass = typeof(FuzzyMatcher);
            var methodsInFuzzyMatcherClass = fuzzyMatcherClass.GetTypeInfo().DeclaredMethods
                                                              .Where(m => !m.IsSpecialName && m.IsPublic);

            var thisTestAssembly = Assembly.GetExecutingAssembly();
            var testTypes = thisTestAssembly.DefinedTypes.ToList();

            var testClassTypes = testTypes.Where(t => !t.Name.Contains("<>"));
            var allTestClassMethods = (from types in testClassTypes
                                       let methods = types.DeclaredMethods
                                       from method in methods
                                       where method.IsPublic &&
                                             method.CustomAttributes.Any(ca => ca.AttributeType == typeof(TestAttribute))
                                       select method);

            var fuzzyMatcherMethodNames = methodsInFuzzyMatcherClass.Select(m => m.Name).ToArray();

            const string test = "Test";

            var testClassMethodNames = allTestClassMethods
                                       .Select(m => m.Name).Where(n =>
                                           n.EndsWith(test, StringComparison.CurrentCultureIgnoreCase)).Select(rn =>
                                       {
                                           var lastPartBeginningWithTest = rn.LastIndexOf(test, StringComparison.Ordinal);
                                           return rn.Remove(lastPartBeginningWithTest, test.Length);
                                       }).ToArray();

            foreach (var fuzzyMatcherMethodName in fuzzyMatcherMethodNames)
            {
                var hasACorrespondingTestName = testClassMethodNames.Any(tcmn =>
                    tcmn.Contains(fuzzyMatcherMethodName) || fuzzyMatcherMethodName.Contains(tcmn));

                Assert.IsTrue(hasACorrespondingTestName,
                    $"There is no corresponding test for this method: {fuzzyMatcherMethodName}");
            }

            Assert.IsTrue(fuzzyMatcherMethodNames.Length < testClassMethodNames.Length);
        }
    }
}