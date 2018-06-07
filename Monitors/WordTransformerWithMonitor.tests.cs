using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WayneKing.Practice.Abstractions;
using WayneKing.Practice.Monitors;

namespace WayneKing.Practice.Monitors.Tests
{
    [TestClass]
    public class WordTransformerWithMonitorTests
    {
        private class TraversalStub : ITransformationTraversalWithMonitor
        {
            public bool IsEmpty { get; }            
            public void Embark(TransformationNode node) {}
            public TransformationNode Disembark() { return null; }
        }

        private class NodeFactoryStub : ITransformationNodeFactory
        {
            public TransformationNode CreateNode(string word) { return null; }
            public TransformationNode CreateNode(TransformationNode precursor, string word) { return null; }
        }

        private class WordModulatorStub : IWordModulator
        {
            public IEnumerable<string> GetModulationsOf(string word) { return null; }
        }

        private class MockMonitorFactory : IMonitorFactory
        {
            public PerformanceCounter NewWordsCounter { get; private set; }
            public PerformanceCounter BaseCounter { get; private set; }

            public PerformanceCounter OpenPerformanceCounter(string name)
            {
                // unfortunately, we use "real" perf counters here, so if they are not already created/installed
                //  on the target machine, these ctor calls will succeed but the resulting PerfCounter objects
                //  will not be updateable and tests will fail when they observe the counters do not update.
                //  (.net core allows 'non existent' perf counters to be used)
                switch (name)
                {
                case "New Words per Word":
                    this.NewWordsCounter = new PerformanceCounter("Word Transformer", name, false);
                    return this.NewWordsCounter;

                case "New Words per Word - Base":
                    this.BaseCounter = new PerformanceCounter("Word Transformer", name, false);
                    return this.BaseCounter;

                default:
                    Assert.Fail("System under test specified an unrecognized perf counter name.");
                    return null;
                }
            }
        }

        private class WordTransformerWithMonitorUnderTest : WordTransformerWithMonitor
        {
            public WordTransformerWithMonitorUnderTest(
                    ITransformationTraversalWithMonitor traversal,
                    ITransformationNodeFactory nodeFactory,
                    IWordModulator wordModulator,
                    IMonitorFactory monitorFactory)
                    : base(traversal, nodeFactory, wordModulator, monitorFactory)
            {
            }

            public void InvokeOnEmbarkedModulationsOf(string word, int count)
            {
                OnEmbarkedModulationsOf(word, count);
            }
        }

        private static readonly TraversalStub traversalStub = new TraversalStub();
        private static readonly NodeFactoryStub nodeFactoryStub = new NodeFactoryStub();
        private static readonly WordModulatorStub wordModulatorStub = new WordModulatorStub();
        private static readonly MockMonitorFactory monitorFactory = new MockMonitorFactory();

        // sut: system under test
        private WordTransformerWithMonitorUnderTest sut;

        public WordTransformerWithMonitorTests()
        {
            this.sut = new WordTransformerWithMonitorUnderTest(
                    traversalStub, nodeFactoryStub, wordModulatorStub, monitorFactory);
        }


        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CtorThrowsOnNullMonitorFactory()
        {
            new WordTransformerWithMonitor(traversalStub, nodeFactoryStub, wordModulatorStub, null);
        }

        [TestMethod]
        public void CtorOpensPerfCounters()
        {
            // note ctor called above by this.ctor
            Assert.IsNotNull(monitorFactory.NewWordsCounter, "new words counter");
            Assert.IsNotNull(monitorFactory.BaseCounter, "base counter");
        }

        [TestMethod]
        public void OnEmbarkModulationsOf_IncrementsCounters()
        {
            monitorFactory.NewWordsCounter.RawValue = 0;
            monitorFactory.BaseCounter.RawValue = 0;

            sut.InvokeOnEmbarkedModulationsOf("word", 20);

            Assert.AreEqual(20, monitorFactory.NewWordsCounter.RawValue, "new words counter");
            Assert.AreEqual(1, monitorFactory.BaseCounter.RawValue, "base counter");
        }
    }
}