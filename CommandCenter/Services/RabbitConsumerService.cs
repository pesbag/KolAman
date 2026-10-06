using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            //await channel.QueueDeclareAsync(
            //    queue: region,
            //    durable: true,
            //    exclusive: false,
            //    autoDelete: false);
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                _logger.LogInformation($"message number {counter} received: {message}");
                Console.WriteLine($"message number {counter} received: {message}");
                await _mongo.SaveToMongoAsync(message);
                counter += 1;
                await Task.CompletedTask;
            };

            await channel.BasicConsumeAsync(region, autoAck: true, consumer: consumer);
        }
        await Task.Delay(Timeout.Infinite, stoppingToken);

    }
}
