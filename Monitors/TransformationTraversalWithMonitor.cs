using System;
using System.Diagnostics;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Monitors
{
    public class TransformationTraversalWithMonitor : ITransformationTraversalWithMonitor
    {
        private readonly ITransformationTraversal core;
        private readonly PerformanceCounter backlogCount;
        private readonly PerformanceCounter rateOfEmbark, rateOfDisembark;

        public TransformationTraversalWithMonitor(ITransformationTraversal core, IMonitorFactory monitorFactory)
        {
            if (core == null)
                throw new ArgumentNullException(nameof(core));
            if (monitorFactory == null)
                throw new ArgumentNullException(nameof(monitorFactory));

            this.core = core;

            this.backlogCount = monitorFactory.OpenPerformanceCounter("Traversal Backlog");
            this.rateOfEmbark = monitorFactory.OpenPerformanceCounter("Traversal Embark per Second");
            this.rateOfDisembark = monitorFactory.OpenPerformanceCounter("Traversal Disembark per Second");
        }

        public bool IsEmpty
        {
            get { return core.IsEmpty; }
        }

        public void Embark(TransformationNode node)
        {
            core.Embark(node);

            // note directly manipulate the RawValue because we are *NOT* targeting thread safety
            backlogCount.RawValue++;
            rateOfEmbark.RawValue++;
        }

        public TransformationNode Disembark()
        {
            var node = core.Disembark();

            backlogCount.RawValue--;
            rateOfDisembark.RawValue++;
            return node;
        }
    }
}