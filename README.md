# Word Transformer

Given an origin word and a target word, attempts to discover real words that can form a transformation sequence between the words, where each step of the transformation sequence is a word that differs from the prior word by a single letter, where the letter is added, removed, or changed.

For example, given an origin word of 'goat' and target word 'floats', a valid transformation sequence is:
> goat -> goats -> gloats -> floats


## How to Use

### to build:
    dotnet build

### to run the unit tests:
    dotnet test

### to run it:

1. Before running it the first time, register the perf counter categories.  
From an elevated PowerShell prompt:

        .\Monitors\CreateCounters.ps1

2. Before running it the first time, acquire a dictionary of words (see below) and provide it to the code. See the `GetLoadedDictionary()` method in the WordTransformerProgram.cs file. The dictionary can be loaded from a local file, or downloaded on-the-fly from a URL. See the public `Load*` methods of the `TrieTreeBuilder` class.

3. Subsequently:

        dotnet run <origin word> <target word>


## Some Aspects of the Code

 * .NET Core
 * Console app
 * Unit tests
 * Dependency Injection
 * Inversion of Control Container
 * Performance counters
 * Trie tree

### About unit tests

The unit test code files are "in-line", adjacent to each code file that is tested. This makes each test file conspicuously easy to locate, and allows the tests to be a more natural companion and extension of the functional code. A caveat is that the tests are built into the "shipped" assembly. That can be addressed in the future when needed by adding some conditional build-target or build-type logic into the project file.

### About the word dictionary

A potential word is considered "real" if it is found in a dictionary. A dictionary is not included here in the code and must be provided. A potential list of 350k words is Prof. Foster's at http://www.math.sjsu.edu/~foster/dictionary.txt.

### About the perf counter creation script

.NET Core supports reading and updating perf counters but not the creation/registration of them, which is a platform-specific capability. Therefore, they need to be created a priori by some other (i.e., not .NET Core) mechanism. On the Windows platform, run the CreateCounters script from a "normal" but elevated PowerShell prompt. The script won't succeed from a PowerShell *Core* prompt, since, well, that's running upon .NET Core.
