using AlertsApi.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using AlertsApi.Dtos;
namespace AlertsApi.Repository;

public class AlertRepository:IAlertRepository
{
    private readonly IMongoCollection<AlertModel> _southCollection;
    private readonly IMongoCollection<AlertModel> _overseasCollection;
    private readonly IMongoCollection<AlertModel> _centerCollection;
    private readonly IMongoCollection<AlertModel> _northCollection;
    public AlertRepository(IMongoCollection<AlertModel> collection)
    {
        _southCollection = collection;
        _overseasCollection = collection;
        _centerCollection = collection;
        _northCollection = collection;
    }
    public async Task<RegionTotalAlertsDto> GetNumberOfAlertsInUnitAsync()
    {
        long totalSouth = await _southCollection.CountDocumentsAsync(FilterDefinition<AlertModel>.Empty);
        long totalOverseas = await _overseasCollection.CountDocumentsAsync(FilterDefinition<AlertModel>.Empty);
        long totalCenter = await _centerCollection.CountDocumentsAsync(FilterDefinition<AlertModel>.Empty);
        long totalNorth = await _northCollection.CountDocumentsAsync(FilterDefinition<AlertModel>.Empty);

        return new RegionTotalAlertsDto
        {
            South = totalSouth,
            Center=totalCenter,
            OverSeas=totalOverseas,
            North=totalNorth
        };
    }
}
