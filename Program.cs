// List, SortedList, Dictionary, SortedDictionary, Binary Search Tree

using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;

Application app = new Application();
app.Execute();

public class DebugFunctions
{
    public enum EDebugType
    {
        success,
        error,
        information,
        warning
    }
    
    public static void Log(string message, EDebugType debugType = EDebugType.information)
    {
        ConsoleColor backup = Console.ForegroundColor;

        switch (debugType)
        {
            case EDebugType.error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("(error) ");
                break;
            case EDebugType.success:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("(success) ");
                break;
            case EDebugType.information:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("(information) ");
                break;
            case EDebugType.warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("(warning) ");
                break;

        }
        
        Console.ForegroundColor = backup;
        Console.WriteLine(message);
    }

    public static string GetCallerMethodName()
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame callerFrame = stackTrace.GetFrame(2); // get the second last caller aside from the function that sent us here
        
        return callerFrame.GetMethod().Name;
    }
}

public class Timers
{
    public class Stopwatch
    {
        private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        public void Start(bool resetStopwatch = true)
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                _stopwatch.Reset();
                DebugFunctions.Log("The stopwatch was already running and got reset, this could be the result of an interrupted function.", DebugFunctions.EDebugType.warning);
            }
                
            if (resetStopwatch)
                _stopwatch.Reset();

            _stopwatch.Start();
        }

        public void Stop(bool returnElapsedTime = false)
        {
            _stopwatch.Stop();

            if (returnElapsedTime)
                DebugFunctions.Log($"{DebugFunctions.GetCallerMethodName()} took {GetElapsedMilliseconds()} ms to execute (stopwatch)");
        }
            
        public long GetElapsedMilliseconds() => _stopwatch.ElapsedMilliseconds;
        public TimeSpan GetElapsed() => _stopwatch.Elapsed;
    }

    public class CPU
    {
            
    }
}

public class Application
{
    private string SampleText = string.Empty;
    private string[] SampleTextWords;
    private int SampleTextWordCount = 0;
    private int wordIncrement = 10000;
    Timers.Stopwatch _stopwatch = new Timers.Stopwatch();
    string[] GetWords(string Text) => Text.Split(' ');
    
    private void AssignFileContentsToString(ref string str)
    {
        Console.WriteLine("Please enter the path to the file you want to read from.");
        string path = Console.ReadLine();
        DebugFunctions.Log("Reading file", DebugFunctions.EDebugType.information);

        bool completed = false;
        
        if (File.Exists(path) && path.EndsWith(".txt"))
        {
            try
            {
                _stopwatch.Start();
                str = File.ReadAllText(path);
                DebugFunctions.Log("Assigned text!", DebugFunctions.EDebugType.success);
                _stopwatch.Stop(true);
                completed = true;
            }
            catch (Exception ex)
            {
                DebugFunctions.Log($"Could not read file. ({ex.Message})", DebugFunctions.EDebugType.error);
            }
        }
        else
        {
            DebugFunctions.Log("Invalid file.", DebugFunctions.EDebugType.error);
        }
        
        if(!completed)
            AssignFileContentsToString(ref str);
    }
    
    static IEnumerable<KeyValuePair<string, int>> ListCounter(string[] words, int wordLimit)
    {
        List<KeyValuePair<string, int>> kvPairs = new List<KeyValuePair<string, int>>();

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];
            
            kvPairs.Add(new KeyValuePair<string, int>(word, 1));
        }

        return kvPairs;
    }
    
    public void Execute()
    {
        AssignFileContentsToString(ref SampleText);
        SampleTextWords = GetWords(SampleText);
        SampleTextWordCount = SampleTextWords.Length;

        DebugFunctions.Log($"The word count of the provided sample is {SampleTextWordCount}.");
        int wordsToCheck = 0;
        while (wordsToCheck > SampleTextWordCount)
        {
            wordsToCheck += wordIncrement;
            
            if (wordsToCheck > SampleTextWordCount)
                wordsToCheck = SampleTextWordCount;
            
            //ListCounter(SampleTextWords, wordsToCheck);
            Console.WriteLine(wordsToCheck);
        }
    }
}

/*
    static IEnumerable<KeyValuePair<string, int>> ListCounter(string[] words, ref List<KeyValuePair<string, int>> kvPairs)
    {
        foreach (string word in words)
        {
            
        }

        return kvPairs;
    }
*/