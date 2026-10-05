using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using cs_notificationGate.Services;
using Microsoft.Extensions.Logging;
namespace cs_notificationGate.FileSystemWatcherProgram;

public class FileSystemWatcherProgram : System.ComponentModel.Component
{
    private readonly string _path;
    private readonly ILogger<FileSystemWatcherProgram> _logger;
    //KafkaProducerService _kafka;
    public FileSystemWatcherProgram(string path
        //, ILogger<FileSystemWatcherProgram> logger
        )
    {
        _path = path;
        //_logger = logger;
        //_kafka = kafka;
    }
    public void CheckForChangesInFile(string path) {
        using var watcher = new FileSystemWatcher(path);
        Console.WriteLine($"enter to file in path:{path}");
        watcher.NotifyFilter = NotifyFilters.Attributes
                                         | NotifyFilters.CreationTime
                                         | NotifyFilters.DirectoryName
                                         | NotifyFilters.FileName
                                         | NotifyFilters.LastAccess
                                         | NotifyFilters.LastWrite
                                         | NotifyFilters.Security
                                         | NotifyFilters.Size;
        watcher.Changed += OnCreated;
        watcher.Created += OnCreated;
        watcher.Error += OnError;

        watcher.Filter = "*.ready";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("Press enter to exit");
        Console.ReadLine();
    }
    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);
        Console.WriteLine(GetJsonContent(e.FullPath));
        //string messageContent = GetJsonContent(value);
        //Console.WriteLine(messageContent);
    }
    private static void OnError(object sender, ErrorEventArgs e) =>
           PrintException(e.GetException());

    private static void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            PrintException(ex.InnerException);
        }
    }
    private static string GetJsonContent(string pathToFolder)
    {
        string directoryPath = Path.GetDirectoryName(pathToFolder);
        if (string.IsNullOrWhiteSpace(directoryPath))
        {
            Console.WriteLine($"error to get directory path: {directoryPath}");
        }
        string[] filePath = Directory.GetFiles(directoryPath, "*.json");
        if (filePath.Length == 0)
        {
            Console.WriteLine($"no json files was found in {filePath}");
        }
        string jsonFileFullPath = filePath[0];
        return File.ReadAllText(jsonFileFullPath);
    }
}

