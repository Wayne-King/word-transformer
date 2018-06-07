using System.Diagnostics;

namespace WayneKing.Practice.Abstractions
{
    public interface IMonitorFactory
    {
        PerformanceCounter OpenPerformanceCounter(string name);
    }
}