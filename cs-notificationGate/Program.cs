using cs_notificationGate.FileSystemWatcherProgram;
using cs_notificationGate.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        path: "logs/watcher-.log",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 10_000_000,
        rollOnFileSizeLimit: true
    )
    .CreateLogger();


var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSerilog();

string kafkaServers = builder.Configuration.GetValue<string>("Kafka:BootstrapService") ?? "localhost:9092";

string watchPathMossad = builder.Configuration.GetValue<string>("Watcher:Path:mossad")!;
string watchPathAman = builder.Configuration.GetValue<string>("Watcher:Path:aman")!;
string watchPathPikudHaoref = builder.Configuration.GetValue<string>("Watcher:Path:pikud-haoref")!;
string watchPathShabak = builder.Configuration.GetValue<string>("Watcher:Path:shabak")!;

builder.Services.AddSingleton(new KafkaProducerService(kafkaServers));
var app = builder.Build();
var kafkaProducer = app.Services.GetRequiredService<KafkaProducerService>();
var logger = app.Services.GetRequiredService<ILogger<FileSystemWatcherProgram>>();

var watcherMossad = new FileSystemWatcherProgram(watchPathMossad, kafkaProducer, logger);
var watcherShabak = new FileSystemWatcherProgram(watchPathShabak, kafkaProducer, logger);
var watcherAman = new FileSystemWatcherProgram(watchPathAman, kafkaProducer, logger);
var watcherPikudHaoref = new FileSystemWatcherProgram(watchPathPikudHaoref, kafkaProducer, logger);

Task.Run(() => watcherMossad.CheckForChangesInFile(watchPathMossad));
Task.Run(() => watcherShabak.CheckForChangesInFile(watchPathShabak));
Task.Run(() => watcherAman.CheckForChangesInFile(watchPathAman));
Task.Run(() => watcherPikudHaoref.CheckForChangesInFile(watchPathPikudHaoref));

Console.WriteLine("all watchers was start press enter to stop");
Console.ReadLine();