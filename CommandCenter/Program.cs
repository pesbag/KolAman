using MongoDB.Driver;
using CommandCenter.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var connectionString = "mongodb://localhost:27017";
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase("AlertsDb");
});

builder.Services.AddSingleton<SaveToMongoDbService>();
builder.Services.AddHostedService<RabbitConsumerService>();

var app = builder.Build();

await app.RunAsync();