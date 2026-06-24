using ProductManagment_APIs.DTOs;

namespace ProductManagment_APIs.Interface
{
    public interface ILocationService
    {
        Task<LocationResponseDto> CreateLocationAsync(LocationCreateDto locationDto);
        Task<BatchResultDto> CreateBatchLocationsAsync(LocationBatchCreateDto batchDto);
        Task<IEnumerable<LocationResponseDto>> GetLocationsByDeviceAsync(string deviceId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<LocationResponseDto> GetRecentLocationByDeviceAsync(string deviceId);
        Task<LocationStatsDto> GetDeviceStatsAsync(string deviceId);
        Task<int> DeleteDeviceLocationsAsync(string deviceId);
        Task<bool> ValidateLocationAsync(LocationCreateDto locationDto);
    }
}
