using System.Diagnostics;

Application app = new Application();
app.Execute();

public class CsvFile // todo: move to own repo and link here
    // todo: EditContent(field, value)
{
    private struct ContentEntry
    {
        public string Content;
        public int Index;
    }

    public string Path = string.Empty; // file path
    private string _content = string.Empty; // file text content to string
    private string _outputSeparator = " | ";
    
    private List<ContentEntry> _entries = new List<ContentEntry>(); // adding it to list for index
    private int _templateCount; // template count (x;y;z;etc at the top of the file)
    
    public void Initialize(string predefinedTemplate = "")
    {
        if (!File.Exists(Path)) // create file if it doesn't exist
            try
            {
                File.Create(Path).Close();
            }
            catch (Exception ex)
            {
                DebugFunctions.Log($"Could not write to {Path}. ({ex.Message})", DebugFunctions.DebugType.Error);
            }

        string templateContent = string.Empty;
        if (predefinedTemplate == "") // if there's no template, use the one provided
        {
            bool inputCancelled = false;
            List<string> csvTemplate = new List<string>();

            while (!inputCancelled)
            {
                DebugFunctions.Log("Please enter the next field name for the csv template. Enter nothing to cancel.",
                    DebugFunctions.DebugType.Input);
                string input = Console.ReadLine();

                if (input.Contains(";") || input.Length == 0 || input == null)
                    DebugFunctions.Log("Invalid string.", DebugFunctions.DebugType.Error);
                else
                    csvTemplate.Add(input);

                DebugFunctions.Log("Do you want to continue? (y/n)", DebugFunctions.DebugType.Input);
                inputCancelled = Console.ReadLine()[0] == 'n';
            }

            for (int i = 0; i < csvTemplate.Count; i++) // since we are creating the file here for further use, we write the individual template entries one by one
            {
                string fieldName = csvTemplate[i];
                templateContent += fieldName;

                if (i < csvTemplate.Count - 1) // we do not add a closing semi-colon at the end unless we want an invalid format
                    templateContent += ";";
            }
        }
        else
            templateContent = predefinedTemplate; // if template was provided, we do not need to ask for any kind of input and we can jump straight to writing it to file as it's expected to be in the correct format

        try
        {
            File.WriteAllText(Path, templateContent);
            DebugFunctions.Log("Template has been created.", DebugFunctions.DebugType.Success);
        }
        catch (Exception ex)
        {
            DebugFunctions.Log("Could not write to file.", DebugFunctions.DebugType.Error);
        }
    }

    public void Read() // assign the file entries to _entries by reading to _content and validating contents
    {
        if (File.Exists(Path)) // only try to read if the file actually exists, a wrong path could've been provided
        {
            try
            {
                _content = File.ReadAllText(Path);
                if (_content == null || _content.Length == 0)
                    throw new ArgumentException(nameof(_content), "Could not read from file.");

                if (_entries.Count > 0)
                    _entries.Clear();

                string[] lines = _content.Split(Environment.NewLine); // split by each new line as thats how we expect entries to be stored, with the template bein at the top
                for (int y = 0; y < lines.Length; y++) // y = entries, x = fields
                {
                    string[] fields = lines[y].Split(';');

                    if (y == 0)
                        _templateCount = fields.Length; // since we are on the first line, only the csv template should be here and we can just count it in the fields

                    for (int x = 0; x < fields.Length; x++)
                    {
                        string field = fields[x];

                        ContentEntry entry = new ContentEntry(); // using this structure, we can assign the template index to the entry and easily access it later (for editing, accessing field name, etc)
                        entry.Content = field;
                        entry.Index = x;

                        _entries.Add(entry);
                    }
                }
            }
            catch (Exception ex)
            {
                DebugFunctions.Log(ex.Message, DebugFunctions.DebugType.Error);
            }
        }
        else
            DebugFunctions.Log($"{Path} does not exist.", DebugFunctions.DebugType.Error);
    }

    public void Append(string content) // writes to last line of csv file
    {
        if (File.Exists(Path))
        {
            try
            {
                File.AppendAllText(Path, $"{Environment.NewLine}{content}");
            }
            catch (Exception ex)
            {
                DebugFunctions.Log($"Could not write to file.", DebugFunctions.DebugType.Error);
            }
        }
        else
            DebugFunctions.Log($"{Path} does not exist.", DebugFunctions.DebugType.Error);
    }

    public void OutputContentsToTerminal(bool clear = false) // loops through all _entries and gets largest string length for readable outputs with clear lines
    {
        if (clear)
            Console.Clear();

        List<string> largestEntries = new List<string>();
        int leftPos = 0;
        
        for (int i = 0; i < _templateCount; i++) // while we are adding the entries that should be defining the spacing between fields, we also make the leftpos the maximum size it can be
        {
            string largestEntry = GetLargestEntry(_entries, i);
            largestEntries.Add(largestEntry);
            leftPos += largestEntry.Length + _outputSeparator.Length;
        }

        try // now that we have the maximum leftPos, we can attempt to set it. if the terminal window is too small compared to the output, we don't continue because then we would crash and the output wouldn't be good
        {
            Console.SetCursorPosition(leftPos, Console.GetCursorPosition().Top);
            Console.SetCursorPosition(0, Console.GetCursorPosition().Top);
            leftPos = 0;
        }
        catch (Exception ex) // this is only hit if setcursorposition failed
        {
            DebugFunctions.Log("The terminal window is too small, cancelling CSV output.", DebugFunctions.DebugType.Error);
            return;
        }
        
        for (int i = 0; i < _entries.Count; i++) // read all entries, output and then add a separator
        {
            int topPosition = Console.GetCursorPosition().Top;
            ContentEntry entry = _entries[i];

            Console.Write(entry.Content);
            leftPos += largestEntries[entry.Index].Length;
            Console.SetCursorPosition(leftPos, topPosition);

            if (entry.Index < _templateCount - 1)
            {
                Console.Write(_outputSeparator);
                leftPos += _outputSeparator.Length;
            }
            else
            {
                Console.Write(Environment.NewLine);
                leftPos = 0;
            }
        }
    }

    private string GetLargestEntry(List<ContentEntry> entries, int index) // check all strings, replace if larger and return
    {
        string largestString = string.Empty;

        foreach (var entry in _entries.Where(e => e.Index == index))
        {
            if (entry.Content.Length > largestString.Length)
                largestString = entry.Content;
        }

        return largestString;
    }
}

public class DebugFunctions // simple debug outputs with categories that have different colors to make it easier on the eyes
{
    public enum DebugType
    {
        Success,
        Error,
        Information,
        Warning,
        Stopwatch,
        CPUTime,
        Input
    }

    public static void Log(string message, DebugType debugType = DebugType.Information)
    {
        ConsoleColor backup = Console.ForegroundColor;

        switch (debugType)
        {
            case DebugType.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("(error) ");
                break;
            case DebugType.Success:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("(success) ");
                break;
            case DebugType.Information:
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write("(information) ");
                break;
            case DebugType.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("(warning) ");
                break;
            case DebugType.Stopwatch:
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("(stopwatch) ");
                break;
            case DebugType.CPUTime:
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.Write("(cpu) ");
                break;
            case DebugType.Input:
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.Write("(input) ");
                break;
        }

        Console.ForegroundColor = backup;
        Console.WriteLine(message);
    }

    public static string GetCallerMethodName() // just for debug functions, gets the name of the calling function
    {
        StackTrace stackTrace = new StackTrace();
        StackFrame
            callerFrame =
                stackTrace.GetFrame(2); // get the second last caller aside from the function that sent us here

        return callerFrame.GetMethod().Name;
    }
}

public class Timers // stopwatch & cpu time can be measured with these classes and their functions
{
    public class Stopwatch
    {
        private System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();

        public void Start()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                DebugFunctions.Log(
                    "Already running and reset, this could be the result of an interrupted function.",
                    DebugFunctions.DebugType.Stopwatch);
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
                    DebugFunctions.DebugType.Stopwatch);
        }

        public double GetElapsedMilliseconds() => _stopwatch.ElapsedMilliseconds;
    }

    public class CPU
    {
        private double _storedTime;
        private double _timeElapsedMilliseconds;
        
        public void Start() => _storedTime = Process.GetCurrentProcess().UserProcessorTime.TotalMilliseconds;
        public void Stop(bool returnElapsedTime = false)
        {
            _timeElapsedMilliseconds = Process.GetCurrentProcess().UserProcessorTime.TotalMilliseconds - _storedTime;
            
            if (returnElapsedTime)
                DebugFunctions.Log(
                    $"{DebugFunctions.GetCallerMethodName()} took {GetElapsedMilliseconds()} ms to execute",
                    DebugFunctions.DebugType.Stopwatch);
        }

        public double GetElapsedMilliseconds() => _timeElapsedMilliseconds;
    }
}

public class Application // this is where the comparison application itself starts, the top code will get moved to other files
{
    private string _sampleTextName = string.Empty; // sample filename
    private string _sampleText = string.Empty; // text contents will be read here
    private string[] _sampleTextWords; // words will be split and assigned here
    private int _sampleTextWordCount = 0; // for debug purposes, we want to know how what the max amount of words are that we can loop through
    private int _wordIncrement = 10000; // each test will increase by 10000, then we clamp it to _sampleTextWordCount
    Timers.Stopwatch _stopwatch = new Timers.Stopwatch(); // to measure time using stopwatch and to compare it to time spent since saving the processor time
    Timers.CPU _cpuTime = new Timers.CPU();
    string[] GetWords(string text) => text.Split(' '); // could be done using multiple ways, but this is the simplest and the purpose of the test is not test the efficiency of split

    public class TestResults // each result will be returned using this class, so that we can add it to a list and save it to csv for later analysis
    {
        public string DataType;
        public string SampleName;
        public KeyValuePair<string, int> MostFrequentWord;
        public int UniqueWords;
        public int WordLimit;
        public int WordCount;

        public double StopwatchElapsedMilliseconds;
        public double CpuElapsedMilliseconds;
    }

    private void AssignFileContentsToString(ref string str) // read sample file, assign to _sampleText and _sampleTextName then get the words for _sampleTextWords
    {
        Console.WriteLine("Please enter the path to the file you want to read from.");
        string path = Console.ReadLine();
        while (path == null || path.Length <= 0 || !File.Exists(path) ||
               !path.EndsWith(".txt")) // don't allow the function to continue unless we have a valid file
        {
            Console.WriteLine("Invalid input.");
            path = Console.ReadLine();
        }

        DebugFunctions.Log("Reading file", DebugFunctions.DebugType.Information);

        bool completed = false;
        
        // no need for file validation here since we already did it above
        try
        {
            _stopwatch.Start(); // measure time it takes to read the file
            str = File.ReadAllText(path);
            DebugFunctions.Log("Assigned text!", DebugFunctions.DebugType.Success);
            _sampleTextName = Path.GetFileName(path);
            _stopwatch.Stop();
            completed = true;
        }
        catch (Exception ex)
        {
            DebugFunctions.Log($"Could not read file. ({ex.Message})", DebugFunctions.DebugType.Error);
        }

        if (!completed)
            AssignFileContentsToString(ref str); // if for some reason the above didn't set completed to true we re-run the entire function
    }

    TestResults ListCounter(string[] words, int wordLimit = 0) // from now on, these tests will use very similar code adapted to their data type
    {
        _stopwatch.Start(); // start both timers
        _cpuTime.Start();

        List<KeyValuePair<string, int>> kvPairs = new List<KeyValuePair<string, int>>(); // save data to KeyValuePair in a List

        if (wordLimit == 0) // if we didn't give it a wordLimit, we just use the maximum amount of words
            wordLimit = words.Length;

        int uniqueWords = 0; // we want to know how many unique words there are in the text, since this also helps us measure the datatype by forcing us to search in it

        for (int i = 0; i < wordLimit; i++) // loop through the allowed wrods
        {
            string word = words[i]; // not using a foreach loop so we store it like this (for indexes)

            int index = kvPairs.FindIndex(kv => kv.Key == word); // -1 if word wasn't stored
            if (index != -1) // word was found
            {
                kvPairs[index] =
                    new KeyValuePair<string, int>(word,
                        kvPairs[index].Value +
                        1); // already exists, so we update it by getting the last int value and going +1
            }
            else // not found
            {
                kvPairs.Add(new KeyValuePair<string, int>(word, 1));
                uniqueWords++; // also means its a unique word, so add it there
            }
        }

        var mostFrequent = kvPairs.OrderByDescending(kv => kv.Value).First(); // get the most frequent / occuring word
        _stopwatch.Stop(false); // now stop the timers and we save the test results by providing it to the CreateTestResults function, which returns a TestResults object
        _cpuTime.Stop();

        return CreateTestResults("List", mostFrequent, uniqueWords, wordLimit, _stopwatch.GetElapsedMilliseconds(),
            _cpuTime.GetElapsedMilliseconds());
        ;
    }

    TestResults SortedListCounter(string[] words, int wordLimit = 0) // everything is the same as above, and from now on the comments will only be to the new parts of the functions
    {
        _stopwatch.Start();
        _cpuTime.Start();

        SortedList<string, int> kvPairs = new SortedList<string, int>();

        if (wordLimit == 0)
            wordLimit = words.Length;

        int uniqueWords = 0;

        for (int i = 0; i < wordLimit; i++)
        {
            string word = words[i];

            if (!kvPairs.TryAdd(word, 1)) // we can use TryAdd here, it either tells us the word already exists or it creates it for us. we don't have to check then add manually, and is a provided part of SortedList that doesn't exist in normal List
                kvPairs[word]++; // already exists, so we update it by getting the last int value and going +1
            else // not found
                uniqueWords++; // we successfully ran TryAdd, which means we added a new entry and that it's a unique word
        }

        var mostFrequent = kvPairs.OrderByDescending(kv => kv.Value).First();
        _stopwatch.Stop(false);
        _cpuTime.Stop();

        return CreateTestResults("SortedList", mostFrequent, uniqueWords, wordLimit,
            _stopwatch.GetElapsedMilliseconds(), _cpuTime.GetElapsedMilliseconds());
    }

    TestResults DictionaryCounter(string[] words, int wordLimit = 0)
    {
        _stopwatch.Start();
        _cpuTime.Start();
        Dictionary<string, int> kvPairs = new Dictionary<string, int>();

        if (wordLimit == 0)
            wordLimit = words.Length;

        int uniqueWords = 0;

        for (int i = 0; i < wordLimit; i++)
        {
            string word = words[i];

            if (!kvPairs.TryAdd(word, 1)) 
                kvPairs[word]++; 
            else
                uniqueWords++; 
        }

        var mostFrequent = kvPairs.OrderByDescending(kv => kv.Value).First();
        _stopwatch.Stop(false);
        _cpuTime.Stop();

        return CreateTestResults("Dictionary", mostFrequent, uniqueWords, wordLimit,
            _stopwatch.GetElapsedMilliseconds(), _cpuTime.GetElapsedMilliseconds());
    }

    TestResults SortedDictionaryCounter(string[] words, int wordLimit = 0)
    {
        _stopwatch.Start();
        _cpuTime.Start();
        SortedDictionary<string, int> kvPairs = new SortedDictionary<string, int>();

        if (wordLimit == 0)
            wordLimit = words.Length;

        int uniqueWords = 0;

        for (int i = 0; i < wordLimit; i++)
        {
            string word = words[i];

            if (!kvPairs.TryAdd(word, 1)) 
                kvPairs[word]++; 
            else
                uniqueWords++; 
        }

        var mostFrequent = kvPairs.OrderByDescending(kv => kv.Value).First();
        _stopwatch.Stop(false);
        _cpuTime.Stop();

        return CreateTestResults("SortedDictionary", mostFrequent, uniqueWords, wordLimit,
            _stopwatch.GetElapsedMilliseconds(), _cpuTime.GetElapsedMilliseconds());
    }

    TestResults BinarySearchTreeCounter(string[] words, int wordLimit = 0)
    {
        _stopwatch.Start();
        _cpuTime.Start();
        AbstractDataTypeComparison.Utilities.BinarySearchTree<string, int> kvPairs =
            new AbstractDataTypeComparison.Utilities.BinarySearchTree<string, int>();

        if (wordLimit == 0)
            wordLimit = words.Length;

        int uniqueWords = 0;

        for (int i = 0; i < wordLimit; i++)
        {
            string word = words[i];

            if (kvPairs.Contains(word)) // word was found
            {
                kvPairs[word]++; // already exists, so we update it by getting the last int value and going +1
            }
            else // not found
            {
                kvPairs.Add(word, 1);
                uniqueWords++;
            }
        }

        var mostFrequent = kvPairs.OrderByDescending(kv => kv.Value).First();
        _stopwatch.Stop(false);
        _cpuTime.Stop();

        return CreateTestResults("BinarySearchTree", mostFrequent, uniqueWords, wordLimit,
            _stopwatch.GetElapsedMilliseconds(), _cpuTime.GetElapsedMilliseconds());
    }


    public TestResults CreateTestResults(string dataType, KeyValuePair<string, int> mostFrequent, int uniqueWords,
        int wordLimit, double stopwatchElapsedMilliseconds, double cpuElapsedMilliseconds)
    {
        TestResults results = new TestResults(); // create TestResults object, assign results and pass it back to the caller
        results.DataType = dataType;
        results.MostFrequentWord = mostFrequent;
        results.UniqueWords = uniqueWords;
        results.WordLimit = wordLimit;
        results.StopwatchElapsedMilliseconds = _stopwatch.GetElapsedMilliseconds();
        results.CpuElapsedMilliseconds = _cpuTime.GetElapsedMilliseconds();

        return results;
    }

    public void Execute() // runs all relevant functions for the application to work (reads sample file, gets words, saves word count and checks in increments of _wordIncrement for each datatype)
    {
        AssignFileContentsToString(ref _sampleText);
        _sampleTextWords = GetWords(_sampleText);
        _sampleTextWordCount = _sampleTextWords.Length;
        DebugFunctions.Log($"The word count of the provided sample is {_sampleTextWordCount}.");

        List<TestResults> results = CheckWords();
        string executionPath = System.Reflection.Assembly.GetExecutingAssembly().Location; // default path is where the executing assembly is located
        string executionDirectory = Path.GetDirectoryName(executionPath); // we don't want the assembly itself, we want the path of the assembly
        
        Console.WriteLine("Please enter the file name to save the csv content to.");
        string fileName = Console.ReadLine();
        while (fileName == null || fileName.Length <= 0) // don't allow the function to continue unless we have a valid filename
        {
            Console.WriteLine("Invalid input.");
            fileName = Console.ReadLine();
        }
        
        if(!fileName.EndsWith(".csv"))
            fileName += ".csv"; // if it doesn't end in csv make it end with csv
        
        string fullPath = $"{executionDirectory}\\{fileName}";

        CsvFile file = WriteResultsToCsv(fullPath, results); // create a csv file by making a CsvFile object, then read from it and output to terminal
        file.Read();
        file.OutputContentsToTerminal();
    }

    public CsvFile WriteResultsToCsv(string path, List<TestResults> results) // if the file already exists, we just append the entries. else, we initialize it using the predefined template
    {
        CsvFile file = new CsvFile();
        file.Path = path;
        if (!File.Exists(path))
            file.Initialize(
                "Datatype;Sample filename;Words tested;Time (stopwatch);Time (cpu);Unique words;Most frequent");

        foreach (TestResults res in results)
        {
            file.Append(
                $"{res.DataType};{res.SampleName};{res.WordLimit}/{res.WordCount};{res.StopwatchElapsedMilliseconds} ms;{res.CpuElapsedMilliseconds} ms;{res.UniqueWords};{res.MostFrequentWord}");
        }

        return file;
    }

    public List<TestResults> CheckWords() // loop 4 times as that's the amount of datatypes we are testing, increase words by wordsToCheck (increases by 10000 until we reach max wordCount and output all to a csv file which is then read)
    {
        List<TestResults> resultsSaved = new List<TestResults>();

        for (int i = 0; i < 5; i++)
        {
            int wordsToCheck = 0;
            int increments = 0;
            while (_sampleTextWordCount > wordsToCheck)
            {
                increments++; // so we can know how many times we've executed the function / datatype
                wordsToCheck += _wordIncrement;

                if (wordsToCheck > _sampleTextWordCount)
                    wordsToCheck = _sampleTextWordCount; // clamp to max words if we reached above it
                
                TestResults res = new TestResults();

                switch (i)
                {
                    case 0:
                        res = ListCounter(_sampleTextWords, wordsToCheck);
                        break;
                    case 1:
                        res = SortedListCounter(_sampleTextWords, wordsToCheck);
                        break;
                    case 2:
                        res = DictionaryCounter(_sampleTextWords, wordsToCheck);
                        break;
                    case 3:
                        res = SortedDictionaryCounter(_sampleTextWords, wordsToCheck);
                        break;
                    case 4:
                        res = BinarySearchTreeCounter(_sampleTextWords, wordsToCheck);
                        break;
                }
                
                DebugFunctions.Log($"-- ({increments}) {wordsToCheck}/{_sampleTextWordCount} words have been read ({res.DataType}) --");
                res.WordCount = _sampleTextWordCount; // we add this here because the functions don't have access to these variables
                res.SampleName = _sampleTextName;
                resultsSaved.Add(res);
            }

            Console.WriteLine($"** Executed a total of {increments} times. **");
        }

        return resultsSaved;
    }
}