using System;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Monitors.Tests
{
    internal class MockWordDictionary : IWordDictionary
    {
        public Func<string, bool> IsWordStringCallback { private get; set; }
        public Func<char[], bool> IsWordCharArrayCallback { private get; set; }

        public bool IsWord(string word)
        {
            return this.IsWordStringCallback(word);
        }

        public bool IsWord(char[] word)
        {
            return this.IsWordCharArrayCallback(word);
        }
    }

    [TestClass]
    public class WordDictionaryWithMonitorTests
    {
        private class MockMonitorFactory : IMonitorFactory
        {
            public PerformanceCounter RatioCounter { get; private set; }
            public PerformanceCounter BaseCounter { get; private set; }

            public PerformanceCounter OpenPerformanceCounter(string name)
            {
                // unfortunately, we use "real" perf counters here, so if they are not already created/installed
                //  on the target machine, these ctor calls will succeed but the resulting PerfCounter objects
                //  will not be updateable and tests will fail when they observe the counters do not update.
                //  (.net core allows 'non existent' perf counters to be used)
                switch (name)
                {
                case "Word Hit Ratio":
                    this.RatioCounter = new PerformanceCounter("Word Transformer", name, false);
                    return this.RatioCounter;

                case "Word Hit Ratio - Base":
                    this.BaseCounter = new PerformanceCounter("Word Transformer", name, false);
                    return this.BaseCounter;

                default:
                    Assert.Fail("System under test specified an unrecognized perf counter name.");
                    return null;
                }
            }
        }

        private const string WordAsString = "word";
        private readonly static char[] WordAsChars = WordAsString.ToCharArray();

        private readonly static MockWordDictionary decoratedDictionary = new MockWordDictionary();
        private readonly static MockMonitorFactory monitorFactory = new MockMonitorFactory();

        // sut: system under test
        private WordDictionaryWithMonitor sut;

        public WordDictionaryWithMonitorTests()
        {
            this.sut = new WordDictionaryWithMonitor(decoratedDictionary, monitorFactory);
        }

        [TestMethod]
        public void CtorThrowsOnNullParam()
        {
            Assert.ThrowsException<ArgumentNullException>(() =>
                    new WordDictionaryWithMonitor(null, monitorFactory), "core");
            Assert.ThrowsException<ArgumentNullException>(() =>
                    new WordDictionaryWithMonitor(decoratedDictionary, null), "monitorFactory");
        }

        [TestMethod]
        public void CtorOpensPerfCounters()
        {
            // note ctor called above by this.ctor
            Assert.IsNotNull(monitorFactory.RatioCounter, "hit ratio counter");
            Assert.IsNotNull(monitorFactory.BaseCounter, "base counter");
        }

        [TestMethod]
        public void IsWordStringPassesParamToCore()
        {
            decoratedDictionary.IsWordStringCallback = word => 
                    {
                        Assert.AreEqual(WordAsString, word);
                        return true;
                    };
            sut.IsWord(WordAsString);
        }

        [TestMethod]
        public void IsWordStringPassesBackReturnValue()
        {
            decoratedDictionary.IsWordStringCallback = word => true;
            Assert.IsTrue(sut.IsWord(WordAsString), "returning true");

            decoratedDictionary.IsWordStringCallback = word => false;
            Assert.IsFalse(sut.IsWord(WordAsString), "returning false");
        }

        [TestMethod]
        public void IsWordStringCalledOnlyOnce()
        {
            int count = 0;
            decoratedDictionary.IsWordStringCallback = word => 
                    {
                        Assert.AreEqual(1, ++count);
                        return true;
                    };
            sut.IsWord(WordAsString);
        }

        [TestMethod]
        public void IsWordStringAlwaysIncrementsBaseCounter()
        {
            monitorFactory.BaseCounter.RawValue = 0;

            decoratedDictionary.IsWordStringCallback = word => true;
            sut.IsWord(WordAsString);
            decoratedDictionary.IsWordStringCallback = word => false;
            sut.IsWord(WordAsString);

            Assert.AreEqual(2, monitorFactory.BaseCounter.RawValue);
        }

        [TestMethod]
        public void IsWordStringIncrementsRatioCounterOnlyWhenIsWord()
        {
            monitorFactory.RatioCounter.RawValue = 0;

            decoratedDictionary.IsWordStringCallback = word => true;
            sut.IsWord(WordAsString);
            decoratedDictionary.IsWordStringCallback = word => false;
            sut.IsWord(WordAsString);

            Assert.AreEqual(1, monitorFactory.RatioCounter.RawValue);
        }

    //
    #region Copy-paste of IsWordString* tests, as IsWordCharArray*

         [TestMethod]
        public void IsWordCharArrayPassesParamToCore()
        {
            decoratedDictionary.IsWordCharArrayCallback = word => 
                    {
                        Assert.AreEqual(WordAsChars, word);
                        return true;
                    };
            sut.IsWord(WordAsChars);
        }

        [TestMethod]
        public void IsWordCharArrayPassesBackReturnValue()
        {
            decoratedDictionary.IsWordCharArrayCallback = word => true;
            Assert.IsTrue(sut.IsWord(WordAsChars), "returning true");

            decoratedDictionary.IsWordCharArrayCallback = word => false;
            Assert.IsFalse(sut.IsWord(WordAsChars), "returning false");
        }

        [TestMethod]
        public void IsWordCharArrayCalledOnlyOnce()
        {
            int count = 0;
            decoratedDictionary.IsWordCharArrayCallback = word => 
                    {
                        Assert.AreEqual(1, ++count);
                        return true;
                    };
            sut.IsWord(WordAsChars);
        }

        [TestMethod]
        public void IsWordCharArrayAlwaysIncrementsBaseCounter()
        {
            monitorFactory.BaseCounter.RawValue = 0;

            decoratedDictionary.IsWordCharArrayCallback = word => true;
            sut.IsWord(WordAsChars);
            decoratedDictionary.IsWordCharArrayCallback = word => false;
            sut.IsWord(WordAsChars);

            Assert.AreEqual(2, monitorFactory.BaseCounter.RawValue);
        }

        [TestMethod]
        public void IsWordCharArrayIncrementsRatioCounterOnlyWhenIsWord()
        {
            monitorFactory.RatioCounter.RawValue = 0;

            decoratedDictionary.IsWordCharArrayCallback = word => true;
            sut.IsWord(WordAsChars);
            decoratedDictionary.IsWordCharArrayCallback = word => false;
            sut.IsWord(WordAsChars);

            Assert.AreEqual(1, monitorFactory.RatioCounter.RawValue);
        }
       
    #endregion
    }
}