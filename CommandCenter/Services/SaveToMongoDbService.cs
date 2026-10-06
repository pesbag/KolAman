using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace CommandCenter.Services;

public class SaveToMongoDbService
{
    private readonly IMongoCollection<BsonDocument> _collection;
    private readonly ILogger<SaveToMongoDbService> _logger;
    public SaveToMongoDbService(ILogger<SaveToMongoDbService> logger,
        IMongoDatabase database)
    {
        _logger = logger;
        _collection = database.GetCollection<BsonDocument>("Alert-Notifications");
    }
    public async Task SaveToMongoAsync(string message)
    {
        try
        {
            var document = BsonDocument.Parse(message);
            await _collection.InsertOneAsync(document);

            _logger.LogInformation("enter a docunetnt to mongodb");
            _logger.LogInformation(
            "inserted to database: '{DbName}', Collection: '{ColName}'",
            _collection.Database.DatabaseNamespace.DatabaseName,
            _collection.CollectionNamespace.CollectionName
            );
        }
        catch (FormatException jsonEx)
        {
            Console.WriteLine($"error in json: {jsonEx.Message}");
            _logger.LogError($"error in json: {jsonEx.Message}");
        }
        catch (MongoException mongoEx)
        {
            Console.WriteLine($"error in saving to MongoDb: {mongoEx.Message}");
            _logger.LogError($"error in saving to MongoDb: {mongoEx.Message}");
        }
    }
}