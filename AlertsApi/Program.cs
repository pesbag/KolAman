using AlertsApi.Models;
using AlertsApi.Repository;
using MongoDB.Bson;
using MongoDB.Driver;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var mongoSettings = builder.Configuration.GetSection("MongoSettings");
var connectionString = mongoSettings["ConnectionString"];
var databaseName = mongoSettings["DatabaseName"];
var southCollection = mongoSettings["CollectionNames:south"];
var northCollection = mongoSettings["CollectionNames:north"];
var overseasCollection = mongoSettings["CollectionNames:overseas"];
var centerCollection = mongoSettings["CollectionNames:center"];


builder.Services.AddSingleton<IMongoClient>(sp => new MongoClient(connectionString));

builder.Services.AddScoped<IMongoCollection<AlertModel>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(databaseName);
    return database.GetCollection<AlertModel>(southCollection);
});
builder.Services.AddScoped<IMongoCollection<AlertModel>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(databaseName);
    return database.GetCollection<AlertModel>(northCollection);
});
builder.Services.AddScoped<IMongoCollection<AlertModel>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(databaseName);
    return database.GetCollection<AlertModel>(overseasCollection);
});
builder.Services.AddScoped<IMongoCollection<AlertModel>>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    var database = client.GetDatabase(databaseName);
    return database.GetCollection<AlertModel>(centerCollection);
});

builder.Services.AddScoped<IAlertRepository, AlertRepository>();

var app = builder.Build();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
