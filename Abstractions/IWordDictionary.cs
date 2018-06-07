namespace WayneKing.Practice.Abstractions
{
    public interface IWordDictionary
    {
        bool IsWord(string word);
        bool IsWord(char[] word);
    }
}