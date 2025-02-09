// List, SortedList, Dictionary, SortedDictionary, Binary Search Tree

using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;

Application app = new Application();
app.Execute();

public class CsvFile
{
    private struct ContentEntry
    {
        public string content;
        public int index;
    }

    public string Path = string.Empty;

    private string _content = string.Empty;

    //private Dictionary<string, int> _template = new Dictionary<string, int>();
    private List<ContentEntry> _entries = new List<ContentEntry>();
    private int _templateCount;


    public void Initialize(bool defineTemplate = true)
    {
        if (!File.Exists(Path))
            try
            {
                File.Create(Path).Close();
            }
            catch (Exception ex)
            {
                DebugFunctions.Log($"Could not write to {Path}. ({ex.Message})", DebugFunctions.EDebugType.Error);
            }

        if (defineTemplate)
        {
            bool inputCancelled = false;
            List<string> csvTemplate = new List<string>();

            while (!inputCancelled)
            {
                DebugFunctions.Log("Please enter the next field name for the csv template. Enter nothing to cancel.",
                    DebugFunctions.EDebugType.Input);
                string input = Console.ReadLine();

                if (input.Contains(";") || input.Length == 0 || input == null)
                    DebugFunctions.Log("Invalid string.", DebugFunctions.EDebugType.Error);
                else
                    csvTemplate.Add(input);

                DebugFunctions.Log("Do you want to continue? (y/n)", DebugFunctions.EDebugType.Input);
                inputCancelled = Console.ReadLine()[0] == 'n';
            }

            string templateContent = string.Empty;
            for (int i = 0; i < csvTemplate.Count; i++)
            {
                string fieldName = csvTemplate[i];
                templateContent += fieldName;

                if (i < csvTemplate.Count - 1)
                    templateContent += ";";
            }

            File.WriteAllText(Path, templateContent);
            DebugFunctions.Log("Template has been created.", DebugFunctions.EDebugType.Success);
        }
    }

    public void Read()
    {
        if (File.Exists(Path))
        {
            try
            {
                _content = File.ReadAllText(Path);
                if (_content == null || _content.Length == 0)
                    throw new ArgumentException(nameof(_content), "Could not read from file.");

                if (_entries.Count > 0)
                    _entries.Clear();

                string[] lines = _content.Split(Environment.NewLine);
                for (int y = 0; y < lines.Length; y++) // y = entries, x = fields
                {
                    string[] fields = lines[y].Split(';');

                    if (y == 0)
                        _templateCount = fields.Length;

                    for (int x = 0; x < fields.Length; x++)
                    {
                        string field = fields[x];

                        ContentEntry entry = new ContentEntry();
                        entry.content = field;
                        entry.index = x;

                        _entries.Add(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugFunctions.Log(ex.Message, DebugFunctions.EDebugType.Error);
            }
        }
        else
            DebugFunctions.Log($"{Path} does not exist.", DebugFunctions.EDebugType.Error);
    }

    public void Append(string content = "")
    {
    }

    public void OutputContents(bool clear = false)
    {
        if (clear)
            Console.Clear();

        List<string> largestEntries = new List<string>();

        for (int i = 0; i < _templateCount; i++)
            largestEntries.Add(GetLargestEntry(_entries, i));

        int leftPos = 0;
        for (int i = 0; i < _entries.Count; i++)
        {
            int topPosition = Console.GetCursorPosition().Top;
            ContentEntry entry = _entries[i];

            Console.Write(entry.content);
            leftPos += largestEntries[entry.index].Length;
            Console.SetCursorPosition(leftPos, topPosition);

            if (entry.index < _templateCount - 1)
            {
                string separator = " | ";
                Console.Write(separator);
                leftPos += separator.Length;
            }
            else
            {
                Console.WriteLine("");
                leftPos = 0;
            }
        }
    }

    private string GetLargestEntry(List<ContentEntry> entries, int index)
    {
        string largestString = string.Empty;

        foreach (var entry in _entries.Where(e => e.index == index))
        {
            if (entry.content.Length > largestString.Length)
                largestString = entry.content;
        }

        return largestString;
    }
}

public class DebugFunctions
{
    public enum EDebugType
    {
        Success,
        Error,
        Information,
        Warning,
        Stopwatch,
        CPUTime,
        Input
    }

    public static void Log(string message, EDebugType debugType = EDebugType.Information)
    {
        ConsoleColor backup = Console.ForegroundColor;

        switch (debugType)
        {
            case EDebugType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("(error) ");
                break;
            case EDebugType.Success:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("(success) ");
                break;
            case EDebugType.Information:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("(information) ");
                break;
            case EDebugType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("(warning) ");
                break;
            case EDebugType.Stopwatch:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("(stopwatch) ");
                break;
            case EDebugType.CPUTime:
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("(cpu) ");
                break;
            case EDebugType.Input:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("(input) ");
                break;
        }

        Console.ForegroundColor = backup;
        Console.WriteLine(message);
    }

    public static string GetCallerMethodName()
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame
            callerFrame =
                stackTrace.GetFrame(2); // get the second last caller aside from the function that sent us here

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
                DebugFunctions.Log(
                    "Already running and got reset, this could be the result of an interrupted function.",
                    DebugFunctions.EDebugType.Stopwatch);
            }

            if (resetStopwatch)
                _stopwatch.Reset();

            _stopwatch.Start();
        }

        public void Stop(bool returnElapsedTime = false)
        {
            _stopwatch.Stop();

            if (returnElapsedTime)
                DebugFunctions.Log(
                    $"{DebugFunctions.GetCallerMethodName()} took {GetElapsedMilliseconds()} ms to execute",
                    DebugFunctions.EDebugType.Stopwatch);
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
    string[] GetWords(string text) => text.Split(' ');

    private void AssignFileContentsToString(ref string str)
    {
        Console.WriteLine("Please enter the path to the file you want to read from.");
        string path = Console.ReadLine();
        DebugFunctions.Log("Reading file", DebugFunctions.EDebugType.Information);

        bool completed = false;

        if (File.Exists(path) && path.EndsWith(".txt"))
        {
            try
            {
                _stopwatch.Start();
                str = File.ReadAllText(path);
                DebugFunctions.Log("Assigned text!", DebugFunctions.EDebugType.Success);
                _stopwatch.Stop(true);
                completed = true;
            }
            catch (Exception ex)
            {
                DebugFunctions.Log($"Could not read file. ({ex.Message})", DebugFunctions.EDebugType.Error);
            }
        }
        else
        {
            DebugFunctions.Log("Invalid file.", DebugFunctions.EDebugType.Error);
        }

        if (!completed)
            AssignFileContentsToString(ref str);
    }

    IEnumerable<KeyValuePair<string, int>> ListCounter(string[] words, int wordLimit = 0)
    {
        _stopwatch.Start();
        List<KeyValuePair<string, int>> kvPairs = new List<KeyValuePair<string, int>>();

        if (wordLimit == 0)
            wordLimit = words.Length;

        for (int i = 0; i < wordLimit; i++)
        {
            string word = words[i];
            int index = kvPairs.FindIndex(kv =>
                kv.Key == word); // will result -1 if the word does not exist, in that case we just add it as kvp(word, 1)

            if (index != -1)
            {
                kvPairs[index] =
                    new KeyValuePair<string, int>(word,
                        kvPairs[index].Value +
                        1); // already exists, so we update it by getting the last int value and going +1
            }
            else
            {
                kvPairs.Add(new KeyValuePair<string, int>(word, 1));
            }
        }

        var mostFrequent = kvPairs.OrderByDescending(kv => kv.Value).FirstOrDefault();
        DebugFunctions.Log($"Most frequent word: {mostFrequent}");
        _stopwatch.Stop(true);
        return kvPairs;
    }

    public void Execute()
    {
        CsvFile file = new CsvFile();
        file.Path = "C:\\Users\\e\\Downloads\\labb1\\Texts\\CSVFile.csv";
        //file.Initialize();
        file.Read();
        file.OutputContents();
    }

    public void Execute2()
    {
        AssignFileContentsToString(ref SampleText);
        SampleTextWords = GetWords(SampleText);
        SampleTextWordCount = SampleTextWords.Length;
        DebugFunctions.Log($"The word count of the provided sample is {SampleTextWordCount}.");

        int wordsToCheck = 0;
        int increments = 0;
        while (SampleTextWordCount > wordsToCheck)
        {
            increments++;
            wordsToCheck += wordIncrement;

            if (wordsToCheck > SampleTextWordCount)
                wordsToCheck = SampleTextWordCount;

            DebugFunctions.Log($"-- ({increments}) Reading {wordsToCheck}/{SampleTextWordCount} words --");
            ListCounter(SampleTextWords, wordsToCheck);
        }

        Console.WriteLine($"Executed {increments} times.");
    }
}