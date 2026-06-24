using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;
using ProductManagment_APIs.Model;


namespace ProductManagment_APIs.Repository
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;

        public LocationService(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<LocationResponseDto> CreateLocationAsync(LocationCreateDto locationDto)
        {
            // Validate
            if (!await ValidateLocationAsync(locationDto))
                throw new ArgumentException("Invalid location data");

            var location = new Location
            {
                DeviceId = locationDto.DeviceId,
                Latitude = locationDto.Latitude,
                Longitude = locationDto.Longitude,
                Altitude = locationDto.Altitude,
                Accuracy = locationDto.Accuracy,
                Speed = locationDto.Speed,
                Heading = locationDto.Heading,
                Timestamp = locationDto.Timestamp,
                CreatedAt = DateTime.UtcNow
            };

            var created = await _locationRepository.CreateLocationAsync(location);
            return MapToResponseDto(created);
        }

        public async Task<BatchResultDto> CreateBatchLocationsAsync(LocationBatchCreateDto batchDto)
        {
            if (batchDto?.Locations == null || !batchDto.Locations.Any())
                throw new ArgumentException("No locations provided");

            var locations = new List<Location>();
            foreach (var dto in batchDto.Locations)
            {
                if (await ValidateLocationAsync(dto))
                {
                    locations.Add(new Location
                    {
                        DeviceId = dto.DeviceId,
                        Latitude = dto.Latitude,
                        Longitude = dto.Longitude,
                        Altitude = dto.Altitude,
                        Accuracy = dto.Accuracy,
                        Speed = dto.Speed,
                        Heading = dto.Heading,
                        Timestamp = dto.Timestamp,
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }

            if (!locations.Any())
                throw new ArgumentException("No valid locations to save");

            var created = await _locationRepository.CreateBatchLocationsAsync(locations);

            return new BatchResultDto
            {
                Success = true,
                Message = $"Successfully saved {created.Count()} locations",
                Count = created.Count()
            };
        }

        public async Task<IEnumerable<LocationResponseDto>> GetLocationsByDeviceAsync(
            string deviceId,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var locations = await _locationRepository.GetLocationsByDeviceAsync(
                deviceId, fromDate, toDate);

            return locations.Select(MapToResponseDto);
        }

        public async Task<LocationResponseDto> GetRecentLocationByDeviceAsync(string deviceId)
        {
            var location = await _locationRepository.GetRecentLocationByDeviceAsync(deviceId);
            if (location == null)
                throw new KeyNotFoundException($"No location found for device: {deviceId}");

            return MapToResponseDto(location);
        }

        public async Task<LocationStatsDto> GetDeviceStatsAsync(string deviceId)
        {
            var stats = await _locationRepository.GetDeviceStatsAsync(deviceId);

            return new LocationStatsDto
            {
                TotalLocations = stats.TotalLocations,
                FirstLocation = stats.FirstLocation,
                LastLocation = stats.LastLocation,
                AverageAccuracy = stats.AverageAccuracy,
                MaxSpeed = stats.MaxSpeed
            };
        }

        public async Task<int> DeleteDeviceLocationsAsync(string deviceId)
        {
            var count = await _locationRepository.DeleteDeviceLocationsAsync(deviceId);
            if (count == 0)
                throw new KeyNotFoundException($"No locations found for device: {deviceId}");

            return count;
        }

        public async Task<bool> ValidateLocationAsync(LocationCreateDto locationDto)
        {
            if (string.IsNullOrWhiteSpace(locationDto.DeviceId))
                return false;

            if (locationDto.Latitude < -90 || locationDto.Latitude > 90)
                return false;

            if (locationDto.Longitude < -180 || locationDto.Longitude > 180)
                return false;

            if (locationDto.Timestamp == default)
                return false;

            return true;
        }

        private LocationResponseDto MapToResponseDto(Location location)
        {
            return new LocationResponseDto
            {
                Id = location.Id,
                DeviceId = location.DeviceId,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Altitude = location.Altitude,
                Accuracy = location.Accuracy,
                Speed = location.Speed,
                Heading = location.Heading,
                Timestamp = location.Timestamp,
                CreatedAt = location.CreatedAt
            };
        }
    }
}