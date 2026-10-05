using cs_notificationGate.FileSystemWatcherProgram;
using System.IO;
using System;
string pathToAman = @"C:\Users\bgdps\OneDrive\שולחן העבודה\FinalProjects\KolAman\alert-simulator\alerts\aman";
string pathToMossad = "C:\\Users\\bgdps\\OneDrive\\שולחן העבודה\\FinalProjects\\KolAman\\alert-simulator\\alerts\\mossad";
//string pathTo
var watch = new FileSystemWatcherProgram(pathToMossad);
//watch.InternalBufferSize = 65536;
watch.CheckForChangesInFile(pathToMossad);
watch.CheckForChangesInFile(pathToAman);


