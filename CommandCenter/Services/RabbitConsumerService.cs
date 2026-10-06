using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CommandCenter.Models;
namespace CommandCenter.Services;

public class RabbitConsumerService:BackgroundService
{
    private readonly SaveToMongoDbService _mongo;
    private readonly ILogger<RabbitConsumerService> _logger;
    private readonly string[] _commandCenters = ["NORTH", "CENTER", "SOUTH", "OVERSEAS"];

    public RabbitConsumerService(SaveToMongoDbService mongo,
        ILogger<RabbitConsumerService> logger)
    {
        _logger = logger;
        _mongo = mongo;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        int counter = 1;
        var factory = new ConnectionFactory { HostName = "localhost" };
        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();
        foreach (var region in _commandCenters)
        {
            var consumer = new AsyncEventingBasicConsumer(channel);

            try
            {
                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var Event = JsonSerializer.Deserialize<AlertModel>(message);

                    _logger.LogInformation($"message number {counter} received: {message}");
                    Console.WriteLine($"message number {counter} received: {message}");
                    if (region == "CENTER" && Event!.Priority!="LOW")
                    {
                    await _mongo.SaveToMongoCenterAsync(message);
                    counter += 1;
                    await Task.CompletedTask;
                    }
                    else if(region == "OVERSEAS" && Event!.Priority != "LOW")
                    {
                        await _mongo.SaveToMongoOverseasAsync(message);
                        counter += 1;
                        await Task.CompletedTask;
                    }
                    else if(region== "NORTH" && Event!.Priority != "LOW")
                    {
                        await _mongo.SaveToMongoNorthAsync(message);
                        counter += 1;
                        await Task.CompletedTask;
                    }
                    else if(Event!.Priority != "LOW")
                    {
                        await _mongo.SaveToMongoSouthAsync(message);
                        counter += 1;
                        await Task.CompletedTask;
                    }
                };
            }
            catch(JsonException ex)
            {
                Console.WriteLine($"error: {ex.Message}");
            }
            await channel.BasicConsumeAsync(region, autoAck: true, consumer: consumer);
        }
        await Task.Delay(Timeout.Infinite, stoppingToken);

    }
}
