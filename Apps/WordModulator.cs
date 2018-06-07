using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WayneKing.Practice.Abstractions;
using WayneKing.Practice.Trees;

namespace WayneKing.Practice.Apps
{
    internal class WordModulator : IWordModulator
    {
        private const string letters = "abcdefghijklmnopqrstuvwxyz";
        private readonly IWordDictionary words;

        public WordModulator(IWordDictionaryWithMonitor words)
        {
            if (words == null)
                throw new ArgumentNullException(nameof(words));

            this.words = words;
        }

        public IEnumerable<string> GetModulationsOf(string word)
        {
            if (word == null)
                throw new ArgumentNullException(nameof(word));

            return EnumerateInsertedChars(word)
                    .Concat(EnumerateDeletedChars(word))
                    .Concat(EnumerateChangedChars(word))
                    .Where(w => words.IsWord(w));
        }

        internal IEnumerable<string> EnumerateInsertedChars(string origin)
        {
            var modula = new char[origin.Length + 1];

            for (int insertAt = 0; insertAt < modula.Length; insertAt++)
            {
                int modulaIndex = 0, originIndex = 0;

                // letters before insertion point
                while (modulaIndex < insertAt)
                {
                    modula[modulaIndex++] = origin[originIndex++];
                }

                // skip insert point
                modulaIndex++;

                char? nextLetter = originIndex < origin.Length ? origin[originIndex] : (char?)null;

                // letters after insertion point
                while (modulaIndex < modula.Length)
                {
                    modula[modulaIndex++] = origin[originIndex++];
                }

                // letter at insertion point
                foreach (char letter in letters)
                {
                    // skip the modulation if it would be a duplicate
                    // for example, given origin of "ab", skip modula 'a'+"ab" but keep "a"+'a'+"b"
                    if (letter != nextLetter)
                    {
                        modula[insertAt] = letter;
                        yield return new string(modula);
                    }
                }
            }
        }

        internal IEnumerable<string> EnumerateDeletedChars(string origin)
        {
            if (origin.Length <= 1)
                yield break;

            var modula = new char[origin.Length - 1];

            for (int deleteAt = 0; deleteAt < origin.Length; deleteAt++)
            {
                // lay out all chars after the deleted one
                for (int modulaIndex = deleteAt, originIndex = deleteAt + 1;
                        modulaIndex < modula.Length;
                        modulaIndex++, originIndex++)
                {
                    modula[modulaIndex] = origin[originIndex];
                }

                yield return new string(modula);

                // place the deleted one, then iterate to the next char to be deleted
                if (deleteAt < modula.Length)
                    modula[deleteAt] = origin[deleteAt];
            }
        }

        internal IEnumerable<string> EnumerateChangedChars(string origin)
        {
            var modula = origin.ToCharArray();

            for (int changeAt = 0; changeAt < origin.Length; changeAt++)
            {
                char original = origin[changeAt];

                foreach (char letter in letters)
                {
                    if (letter != original)
                    {
                        modula[changeAt] = letter;
                        yield return new string(modula);
                    }
                }

                modula[changeAt] = original;
            }
        }
    }
}