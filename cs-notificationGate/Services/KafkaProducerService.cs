using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace cs_notificationGate.Services;

public class KafkaProducerService
{
    private readonly string _bootstrapServices;
    //private readonly ILogger<KafkaProducerService> _logger;
    private readonly IProducer<Null, string> _producer;
    public KafkaProducerService(string bootstrapServices
        //,ILogger<KafkaProducerService> logger
        )
    {
        _bootstrapServices = bootstrapServices;
        var config = new ProducerConfig { BootstrapServers = bootstrapServices };
        _producer = new ProducerBuilder<Null, string>(config).Build();
        //_logger = logger;
    }

    public async Task SendToKafkaAsync(string topicName, Message<Null, string> content)
    {
        try
        {
            var result = await _producer.ProduceAsync(topicName, content);
            Console.WriteLine($"delivered report {content.Key} to partition {result.Partition.Value} at offset {result.Offset.Value}");
        }
        catch (ProduceException<Null, string> ex)
        {
            Console.WriteLine($"kafka Error for station {content.Key}: {ex.Error.Reason}");
        }
    }
}