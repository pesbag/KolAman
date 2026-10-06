//using AlertsApi.Dtos;
//using System.Text.Json;
//using StackExchange.Redis;
//namespace AlertsApi.Repository;

//public class CasheAlertRepository : IAlertRepository
//{
//    private readonly IAlertRepository _innerRepo;
//    private IDatabase _redis;
//    public CasheAlertRepository(IAlertRepository innerRepo,IConnectionMultiplexer muxer)
//    {
//        _innerRepo = innerRepo;
//        _redis = muxer.GetDatabase();
//    }
//    //public async Task<RegionTotalAlertsDto> GetNumberOfAlertsInUnitAsync()
//    //{
//    //    string cashKey="Total:Alerts";
//}
//}