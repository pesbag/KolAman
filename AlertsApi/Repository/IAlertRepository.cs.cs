using AlertsApi.Dtos;

namespace AlertsApi.Repository;

public interface IAlertRepository
{
    Task<RegionTotalAlertsDto> GetNumberOfAlertsInUnitAsync();
}
