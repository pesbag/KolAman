using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using CommandCenter.Models;

namespace CommandCenter.Services;

public class SaveToMongoDbService
{
    private readonly IMongoCollection<BsonDocument> _northCollection;
    private readonly IMongoCollection<BsonDocument> _centerCollection;
    private readonly IMongoCollection<BsonDocument> _overseasCollection;
    private readonly IMongoCollection<BsonDocument> _southCollection;
    private readonly ILogger<SaveToMongoDbService> _logger;
    public SaveToMongoDbService(ILogger<SaveToMongoDbService> logger,
        IMongoDatabase database)
    {
        _logger = logger;
        _northCollection = database.GetCollection<BsonDocument>("Alert-Notifications-north");
        _centerCollection = database.GetCollection<BsonDocument>("Alert-Notifications-center");
        _southCollection = database.GetCollection<BsonDocument>("Alert-Notifications-south");
        _overseasCollection = database.GetCollection<BsonDocument>("Alert-Notifications-overseas");
    }
    public async Task SaveToMongoCenterAsync(string message)
    {
        try
        {
            var document = BsonDocument.Parse(message);
            await _centerCollection.InsertOneAsync(document);
            _logger.LogInformation("enter a docunetnt to mongodb");
            _logger.LogInformation(
            "inserted to database: '{DbName}', Collection: '{ColName}'",
            _northCollection.Database.DatabaseNamespace.DatabaseName,
            _northCollection.CollectionNamespace.CollectionName
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
    public async Task SaveToMongoSouthAsync(string message)
    {
        try
        {
            var document = BsonDocument.Parse(message);
            await _overseasCollection.InsertOneAsync(document);
            _logger.LogInformation("enter a docunetnt to mongodb");
            _logger.LogInformation(
            "inserted to database: '{DbName}', Collection: '{ColName}'",
            _northCollection.Database.DatabaseNamespace.DatabaseName,
            _northCollection.CollectionNamespace.CollectionName
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
    public async Task SaveToMongoOverseasAsync(string message)
    {
        try
        {
            var document = BsonDocument.Parse(message);
            await _southCollection.InsertOneAsync(document);
            _logger.LogInformation("enter a docunetnt to mongodb");
            _logger.LogInformation(
            "inserted to database: '{DbName}', Collection: '{ColName}'",
            _northCollection.Database.DatabaseNamespace.DatabaseName,
            _northCollection.CollectionNamespace.CollectionName
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

    public async Task SaveToMongoNorthAsync(string message)
    {
        try
        {
            var document = BsonDocument.Parse(message);
            await _northCollection.InsertOneAsync(document);
            _logger.LogInformation("enter a docunetnt to mongodb");
            _logger.LogInformation(
            "inserted to database: '{DbName}', Collection: '{ColName}'",
            _northCollection.Database.DatabaseNamespace.DatabaseName,
            _northCollection.CollectionNamespace.CollectionName
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