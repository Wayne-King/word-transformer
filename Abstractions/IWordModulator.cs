using System.Collections.Generic;

namespace WayneKing.Practice.Abstractions
{
    public interface IWordModulator
    {
        IEnumerable<string> GetModulationsOf(string word);
    }
}