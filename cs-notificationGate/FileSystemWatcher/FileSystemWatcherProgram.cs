using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace cs_notificationGate.FileSystemWatcherProgram;

public class FileSystemWatcherProgram : System.ComponentModel.Component
{
    private readonly string _path;
    public FileSystemWatcherProgram(string path)
    {
        _path = path;
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
        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Error += OnError;

        //watcher.Filter = "*.json";
        watcher.IncludeSubdirectories = true;
        watcher.EnableRaisingEvents = true;
        Console.WriteLine("Press enter to exit");
        Console.ReadLine();
    }
    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        Console.WriteLine("check for changes");
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        Console.WriteLine($"Changed: {e.FullPath}");
    }
    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);
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
}

