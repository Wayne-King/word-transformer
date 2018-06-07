using System;
using Microsoft.Extensions.DependencyInjection;
using WayneKing.Practice.Abstractions;
using WayneKing.Practice.Apps;
using WayneKing.Practice.Monitors;
using WayneKing.Practice.Trees;

namespace WayneKing.Practice
{
    class WordTransformerProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("<<hello>>");

            IServiceProvider services = ConfigureServices().BuildServiceProvider();
            var xformer = services.GetRequiredService<WordTransformer>();

            if (args.Length == 2)
                xformer.Transform(args[0], args[1]);
            else
                xformer.Transform("goat", "floats");

            Console.WriteLine("<<goodbye>>");
        }

        private static IServiceCollection ConfigureServices()
        {
            return new ServiceCollection()
                    .AddSingleton<ITransformationNodeFactory, TransformationNodeFactory>()
                    .AddSingleton<IMonitorFactory>(new MonitorFactory("Word Transformer"))

                    .AddSingleton<ITransformationTraversalWithMonitor, TransformationTraversalWithMonitor>()
                    .AddSingleton<ITransformationTraversal, FifoTraversal>()

                    .AddSingleton<IWordDictionaryWithMonitor, WordDictionaryWithMonitor>()
                    .AddSingleton<IWordDictionary>(GetLoadedDictionary())

                    .AddSingleton<IWordModulator, WordModulator>()
                    .AddSingleton<WordTransformer, WordTransformerWithMonitor>();
        }

        private static IWordDictionary GetLoadedDictionary()
        {
            var builder = new TrieTreeBuilder();
            builder.LoadWordsFromFile(@"C:\temp\wordlist.txt");
            return builder.ToTree();
        }
    }
}
