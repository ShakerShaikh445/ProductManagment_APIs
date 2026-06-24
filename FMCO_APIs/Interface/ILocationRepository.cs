using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Model;

namespace ProductManagment_APIs.Interface
{
    public interface ILocationRepository
    {
        Task<Location> CreateLocationAsync(Location location);
        Task<IEnumerable<Location>> CreateBatchLocationsAsync(IEnumerable<Location> locations);
        Task<IEnumerable<Location>> GetLocationsByDeviceAsync(string deviceId, DateTime? fromDate = null, DateTime? toDate = null, int limit = 1000);
        Task<Location> GetRecentLocationByDeviceAsync(string deviceId);
        Task<LocationStats> GetDeviceStatsAsync(string deviceId);
        Task<int> DeleteDeviceLocationsAsync(string deviceId);
        Task<int> GetLocationCountByDeviceAsync(string deviceId);
        Task<bool> DeviceExistsAsync(string deviceId);
    }
}
