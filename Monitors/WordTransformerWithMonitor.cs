using System;
using System.Diagnostics;
using WayneKing.Practice.Abstractions;
using WayneKing.Practice.Apps;

namespace WayneKing.Practice.Monitors
{
    public class WordTransformerWithMonitor : WordTransformer
    {
        private readonly PerformanceCounter generatedWordsCounter, processedNodesCounter;

        public WordTransformerWithMonitor(
                ITransformationTraversalWithMonitor traversal,
                ITransformationNodeFactory nodeFactory,
                IWordModulator wordModulator,
                IMonitorFactory monitorFactory)
                : base(traversal, nodeFactory, wordModulator)
        {
            if (monitorFactory == null)
                throw new ArgumentNullException(nameof(monitorFactory));

            this.generatedWordsCounter = monitorFactory.OpenPerformanceCounter("New Words per Word");
            this.processedNodesCounter = monitorFactory.OpenPerformanceCounter("New Words per Word - Base");
        }

        protected override void OnEmbarkedModulationsOf(string word, int count)
        {
            generatedWordsCounter.RawValue += count;
            processedNodesCounter.RawValue++;
        }
    }
}