using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Apps.Tests
{
    [TestClass]
    public class WordModulatorTests
    {
        private class MockWordDictionary : IWordDictionary, IWordDictionaryWithMonitor
        {
            private readonly bool response;

            private MockWordDictionary(bool response)
            {
                this.response = response;
            }

            public static IWordDictionaryWithMonitor AcceptsAnyWord = new MockWordDictionary(true);
            public static IWordDictionaryWithMonitor AcceptsNoWord = new MockWordDictionary(false);

            public bool IsWord(string word) { return response; }
            public bool IsWord(char[] word) { return response; }
        }

        private const string alphabet = "abcdefghijklmnopqrstuvwxyz";

        private WordModulator wm;

        public WordModulatorTests()
        {
            // set a default instance under test
            this.wm = new WordModulator(MockWordDictionary.AcceptsAnyWord);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CtorThrowsOnNullWordDictionary()
        {
            new WordModulator(null);
        }

        [TestMethod]
        public void AlphabetIsAthroughZ()
        {
            var letters = string.Concat(wm.EnumerateInsertedChars(string.Empty).SelectMany(word => word));

            Assert.AreEqual(alphabet, letters);
        }

        [TestMethod]
        public void CharsInsertedAtFrontMiddleEnd()
        {
            var words = wm.EnumerateInsertedChars("ab").ToArray();

            int expectCount =
                    alphabet.Length * 3  // each letter @ begin, middle, end
                    - 1   // except duplicate "aab"
                    - 1;  // except duplicate "abb"
            Assert.AreEqual(expectCount, words.Length, "total count");

            Assert.AreEqual("bab", words.First(), "first word when insert at begin");
            Assert.AreEqual("aab", words[alphabet.Length - 1], "first word when insert in middle");
            Assert.AreEqual("abz", words.Last(), "last word");
        }

        [TestMethod]
        public void RedundantModulationsCausedByDuplicateLetterPairsAreAvoided()
        {
            var words = wm.EnumerateInsertedChars("ab");
            Assert.AreEqual(1, words.Count(word => word == "aab"), "count('aab')");
            Assert.AreEqual(1, words.Count(word => word == "abb"), "count('abb')");
            
            words = wm.EnumerateInsertedChars("aa");
            Assert.AreEqual(1, words.Count(word => word == "aaa"), "count('aaa')");
        }

        [TestMethod]
        public void CharsDeletedAtFrontMiddleEnd()
        {
            Assert.IsTrue(wm.EnumerateDeletedChars("ab")
                    .SequenceEqual(new[] { "b", "a" }), "of 'ab'");
            Assert.IsTrue(wm.EnumerateDeletedChars("abc")
                    .SequenceEqual(new[] { "bc", "ac", "ab" }), "of 'abc'");
        }

        [TestMethod]
        public void DeleteFromEmptyIsEmptySet()
        {
            Assert.AreEqual(0, wm.EnumerateDeletedChars(string.Empty).Count(), "count");
        }

        [TestMethod]
        public void DeleteFromLoneCharIsEmptySet()
        {
            Assert.AreEqual(0, wm.EnumerateDeletedChars("a").Count(), "count");
        }

        [TestMethod]
        public void ChangeOfEmptySetIsEmptySet()
        {
            Assert.AreEqual(0, wm.EnumerateChangedChars(string.Empty).Count(), "count");
        }

        [TestMethod]
        public void ChangeChars()
        {
            const string origin = "ab";
            var words = wm.EnumerateChangedChars(origin).ToArray();

            int expectCount = alphabet.Length * origin.Length  // each letter changed through alphabet
                    - origin.Length;  // except when the changed letter would be same as original
            Assert.AreEqual(expectCount, words.Length, "count");

            Assert.AreEqual("bb", words.First(), "first word");
            Assert.AreEqual("zb", words[alphabet.Length - 2], "last word of first letter's changes");
            Assert.AreEqual("aa", words[alphabet.Length - 1], "first word of second letter's changes");
            Assert.AreEqual("ac", words[alphabet.Length], "second word of second letter's changes");
            Assert.AreEqual("az", words.Last(), "last word");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetModulationsThrowsOnNullWord()
        {
            wm.GetModulationsOf(null);
        }

        [TestMethod]
        public void GetModulationsReturnsInsertedDeletedChanged()
        {
            var words = wm.GetModulationsOf("ab").ToArray();

            Assert.IsTrue(words.Any(word => word.Length == 3), "any word with inserted letter");
            Assert.IsTrue(words.Any(word => word.Length == 1), "any word with deleted letter");
            Assert.IsTrue(words.Any(word => word.Length == 2 && word != "ab"), "any word with changed letter");
        }

        [TestMethod]
        public void GetModulationsFiltersNonWords()
        {
            wm = new WordModulator(MockWordDictionary.AcceptsNoWord);
            Assert.IsFalse(wm.GetModulationsOf("a").Any());
        }
    }
}