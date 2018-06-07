using System;
using System.Diagnostics;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Monitors
{
    public class WordDictionaryWithMonitor : IWordDictionaryWithMonitor
    {
        private readonly IWordDictionary core;
        private readonly PerformanceCounter wordTestCount, wordHitCount;

        public WordDictionaryWithMonitor(IWordDictionary core, IMonitorFactory monitorFactory)
        {
            if (core == null)
                throw new ArgumentNullException(nameof(core));
            if (monitorFactory == null)
                throw new ArgumentNullException(nameof(monitorFactory));

            this.core = core;

            this.wordTestCount = monitorFactory.OpenPerformanceCounter("Word Hit Ratio - Base");
            this.wordHitCount = monitorFactory.OpenPerformanceCounter("Word Hit Ratio");
        }

        public bool IsWord(string word)
        {
            bool isWord = core.IsWord(word);
            IncrementCounters(isWord);
            return isWord;
        }

        public bool IsWord(char[] word)
        {
            bool isWord = core.IsWord(word);
            IncrementCounters(isWord);
            return isWord;
        }

        private void IncrementCounters(bool isWord)
        {
            wordTestCount.RawValue++;
            if (isWord)
            {
                wordHitCount.RawValue++;
            }
        }
    }
}