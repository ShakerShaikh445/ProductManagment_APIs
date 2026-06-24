using Microsoft.EntityFrameworkCore;
using ProductManagment_APIs.Data;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;
using ProductManagment_APIs.Model;

namespace ProductManagment_APIs.Repository
{
    public class LocationRepository : ILocationRepository
    {
        private readonly AppDbContext _context;

        public LocationRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Location> CreateLocationAsync(Location location)
        {
            await _context.Locations.AddAsync(location);
            await _context.SaveChangesAsync();
            return location;
        }

        public async Task<IEnumerable<Location>> CreateBatchLocationsAsync(IEnumerable<Location> locations)
        {
            await _context.Locations.AddRangeAsync(locations);
            await _context.SaveChangesAsync();
            return locations;
        }

        public async Task<IEnumerable<Location>> GetLocationsByDeviceAsync(
            string deviceId,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            int limit = 1000)
        {
            var query = _context.Locations
                .Where(l => l.DeviceId == deviceId);

            if (fromDate.HasValue)
                query = query.Where(l => l.Timestamp >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(l => l.Timestamp <= toDate.Value);

            return await query
                .OrderByDescending(l => l.Timestamp)
                .Take(limit)
                .ToListAsync();
        }

        public async Task<Location> GetRecentLocationByDeviceAsync(string deviceId)
        {
            return await _context.Locations
                .Where(l => l.DeviceId == deviceId)
                .OrderByDescending(l => l.Timestamp)
                .FirstOrDefaultAsync();
        }

        public async Task<LocationStats> GetDeviceStatsAsync(string deviceId)
        {
            var stats = await _context.Locations
                .Where(l => l.DeviceId == deviceId)
                .GroupBy(l => 1)
                .Select(g => new LocationStats
                {
                    TotalLocations = g.Count(),
                    FirstLocation = g.Min(l => l.Timestamp),
                    LastLocation = g.Max(l => l.Timestamp),
                    AverageAccuracy = g.Average(l => l.Accuracy ?? 0),
                    MaxSpeed = g.Max(l => l.Speed ?? 0)
                })
                .FirstOrDefaultAsync();

            return stats ?? new LocationStats();
        }

        public async Task<int> DeleteDeviceLocationsAsync(string deviceId)
        {
            var locations = await _context.Locations
                .Where(l => l.DeviceId == deviceId)
                .ToListAsync();

            if (!locations.Any())
                return 0;

            _context.Locations.RemoveRange(locations);
            await _context.SaveChangesAsync();
            return locations.Count;
        }

        public async Task<int> GetLocationCountByDeviceAsync(string deviceId)
        {
            return await _context.Locations
                .Where(l => l.DeviceId == deviceId)
                .CountAsync();
        }

        public async Task<bool> DeviceExistsAsync(string deviceId)
        {
            return await _context.Locations
                .AnyAsync(l => l.DeviceId == deviceId);
        }
    }
}