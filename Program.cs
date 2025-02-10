// List, SortedList, Dictionary, SortedDictionary, Binary Search Tree

using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Security.AccessControl;
using System.Timers;

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


    public void Initialize(string predefinedTemplate = "")
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

        string templateContent = string.Empty;
        if (predefinedTemplate == "")
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

            for (int i = 0; i < csvTemplate.Count; i++)
            {
                string fieldName = csvTemplate[i];
                templateContent += fieldName;

                if (i < csvTemplate.Count - 1)
                    templateContent += ";";
            }
        }
        else
        {
            templateContent = predefinedTemplate;
        }

        try
        {
            File.WriteAllText(Path, templateContent);
            DebugFunctions.Log("Template has been created.", DebugFunctions.EDebugType.Success);
        }
        catch (Exception ex)
        {
            DebugFunctions.Log("Could not write to file.", DebugFunctions.EDebugType.Error);
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

    public void Append(string content)
    {
        if (File.Exists(Path))
        {
            try
            {
                File.AppendAllText(Path, $"{Environment.NewLine}{content}");
            }
            catch (Exception ex)
            {
                DebugFunctions.Log($"Could not write to file.", DebugFunctions.EDebugType.Error);
            }
        }
        else
            DebugFunctions.Log($"{Path} does not exist.", DebugFunctions.EDebugType.Error);
    }

    public void OutputContentsToTerminal(bool clear = false)
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

        public void Start()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                DebugFunctions.Log("Already running and got reset, this could be the result of an interrupted function.", DebugFunctions.EDebugType.Stopwatch);
            }

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

        public double GetElapsedMilliseconds() => _stopwatch.ElapsedMilliseconds;
    }

    public class CPU
    {
        private double storedTime;
        private double timeElapsedMilliseconds;

        public void Start()
        {
            storedTime = Process.GetCurrentProcess().UserProcessorTime.TotalMilliseconds;
        }

        public void Stop()
        {
            timeElapsedMilliseconds = Process.GetCurrentProcess().UserProcessorTime.TotalMilliseconds - storedTime;   
        }
        
        public double GetElapsedMilliseconds() => timeElapsedMilliseconds;
    }
}

public class Application
{
    private string _sampleTextName = string.Empty;
    private string _sampleText = string.Empty;
    private string[] _sampleTextWords;
    private int _sampleTextWordCount = 0;
    private int _wordIncrement = 10000;
    Timers.Stopwatch _stopwatch = new Timers.Stopwatch();
    Timers.CPU _cpuTime = new Timers.CPU();
    string[] GetWords(string text) => text.Split(' ');

    public class TestResults
    {
        public string DataType;
        public string SampleName;
        public KeyValuePair<string, int> MostFrequent;
        public int UniqueWords;
        public int WordLimit;
        public int WordCount;

        public double StopwatchElapsedMilliseconds;
        public double CpuElapsedMilliseconds;
    }
    
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
                _sampleTextName = Path.GetFileName(path);
                _stopwatch.Stop();
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

    TestResults ListCounter(string[] words, int wordLimit = 0)
    {
        _stopwatch.Start();
        _cpuTime.Start();
        
        List<KeyValuePair<string, int>> kvPairs = new List<KeyValuePair<string, int>>();

        if (wordLimit == 0)
            wordLimit = words.Length;

        int uniqueWords = 0;
        
        for (int i = 0; i < wordLimit; i++)
        {
            string word = words[i];

            int index = kvPairs.FindIndex(kv => kv.Key == word); // -1 if word wasn't stored
            if (index != -1) // word was found
            {
                kvPairs[index] =  new KeyValuePair<string, int>(word,  kvPairs[index].Value + 1); // already exists, so we update it by getting the last int value and going +1
            }
            else // not found
            {
                kvPairs.Add(new KeyValuePair<string, int>(word, 1));
                uniqueWords++;
            }
        }

        var mostFrequent = kvPairs.OrderByDescending(kv => kv.Value).First();
        _stopwatch.Stop(false);
        _cpuTime.Stop();

        return CreateTestResults("List", mostFrequent, uniqueWords, wordLimit, _stopwatch.GetElapsedMilliseconds(), _cpuTime.GetElapsedMilliseconds());;
    }

    public TestResults CreateTestResults(string dataType, KeyValuePair<string, int> mostFrequent, int uniqueWords, int wordLimit, double stopwatchElapsedMilliseconds, double cpuElapsedMilliseconds)
    {
        TestResults results = new TestResults();
        results.DataType = "List";
        results.MostFrequent = mostFrequent;
        results.UniqueWords = uniqueWords;
        results.WordLimit = wordLimit;
        results.StopwatchElapsedMilliseconds = _stopwatch.GetElapsedMilliseconds();
        results.CpuElapsedMilliseconds = _cpuTime.GetElapsedMilliseconds();
        
        return results;
    }

    public void Execute()
    {
        AssignFileContentsToString(ref _sampleText);
        _sampleTextWords = GetWords(_sampleText);
        _sampleTextWordCount = _sampleTextWords.Length;
        DebugFunctions.Log($"The word count of the provided sample is {_sampleTextWordCount}.");

        List<TestResults> results = CheckWords();
        string executionPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
        string executionDirectory = Path.GetDirectoryName(executionPath); //  $"{executionDirectory}\\AbstractDatatypeComparison.csv";
        string fullPath = $"{executionDirectory}\\AbstractDatatypeComparison.csv";
        
        CsvFile file = WriteResultsToCsv(fullPath, results);
        file.Read();
        file.OutputContentsToTerminal();
    }

    public CsvFile WriteResultsToCsv(string path, List<TestResults> results)
    {
        CsvFile file = new CsvFile();
        file.Path = path;
        if(!File.Exists(path))
        file.Initialize("Datatype;Sample filename;Words tested;Time (stopwatch);Time (cpu);Unique words;Most frequent");

        foreach (TestResults res in results)
        {
            file.Append($"{res.DataType};{res.SampleName};{res.WordLimit}/{res.WordCount};{res.StopwatchElapsedMilliseconds} ms;{res.CpuElapsedMilliseconds} ms;{res.UniqueWords};{res.MostFrequent}");
        }

        return file;
    }
    
    public List<TestResults> CheckWords()
    {
        List<TestResults> resultsSaved = new List<TestResults>();

        int wordsToCheck = 0;
        int increments = 0;
        while (_sampleTextWordCount > wordsToCheck)
        {
            increments++;
            wordsToCheck += _wordIncrement;

            if (wordsToCheck > _sampleTextWordCount)
                wordsToCheck = _sampleTextWordCount;

            DebugFunctions.Log($"-- ({increments}) Reading {wordsToCheck}/{_sampleTextWordCount} words using List --");
            TestResults res = ListCounter(_sampleTextWords, wordsToCheck);
            res.WordCount = _sampleTextWordCount;
            res.SampleName = _sampleTextName;
            resultsSaved.Add(res);
        }

        Console.WriteLine($"Executed a total of {increments} times.");

        return resultsSaved;
    }
}