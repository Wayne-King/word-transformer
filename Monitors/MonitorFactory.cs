using System;
using System.Diagnostics;
using WayneKing.Practice.Abstractions;

namespace WayneKing.Practice.Monitors
{
    public class MonitorFactory : IMonitorFactory
    {
        private readonly string category;

        public MonitorFactory(string categoryName)
        {
            if (string.IsNullOrEmpty(categoryName))
                throw new ArgumentNullException(nameof(categoryName));

            this.category = categoryName;
        }

        public PerformanceCounter OpenPerformanceCounter(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException(nameof(name));

            return new PerformanceCounter(category, name, false);
        }
    }
}