using Microsoft.AspNetCore.Mvc;
using ProductManagment_APIs.DTOs;
using ProductManagment_APIs.Interface;

namespace YourApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        // POST: api/locations
        [HttpPost]
        public async Task<IActionResult> CreateLocation([FromBody] LocationCreateDto locationDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { success = false, message = "Invalid model state", errors = ModelState });

                var result = await _locationService.CreateLocationAsync(locationDto);
                return Ok(new { success = true, message = "Location saved successfully", data = result });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // POST: api/locations/batch
        [HttpPost("batch")]
        public async Task<IActionResult> CreateBatchLocations([FromBody] LocationBatchCreateDto batchDto)
        {
            try
            {
                if (batchDto?.Locations == null || !batchDto.Locations.Any())
                    return BadRequest(new { success = false, message = "No locations provided" });

                var result = await _locationService.CreateBatchLocationsAsync(batchDto);
                return Ok(new { success = true, message = result.Message, count = result.Count });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: api/locations/device/{deviceId}
        [HttpGet("device/{deviceId}")]
        public async Task<IActionResult> GetLocationsByDevice(
            string deviceId,
            [FromQuery] DateTime? fromDate,
            [FromQuery] DateTime? toDate)
        {
            try
            {
                var locations = await _locationService.GetLocationsByDeviceAsync(deviceId, fromDate, toDate);
                return Ok(new { success = true, data = locations, count = locations.Count() });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: api/locations/recent/{deviceId}
        [HttpGet("recent/{deviceId}")]
        public async Task<IActionResult> GetRecentLocation(string deviceId)
        {
            try
            {
                var location = await _locationService.GetRecentLocationByDeviceAsync(deviceId);
                return Ok(new { success = true, data = location });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // DELETE: api/locations/device/{deviceId}
        [HttpDelete("device/{deviceId}")]
        public async Task<IActionResult> DeleteDeviceLocations(string deviceId)
        {
            try
            {
                var count = await _locationService.DeleteDeviceLocationsAsync(deviceId);
                return Ok(new { success = true, message = $"Deleted {count} locations" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }

        // GET: api/locations/stats/{deviceId}
        [HttpGet("stats/{deviceId}")]
        public async Task<IActionResult> GetDeviceStats(string deviceId)
        {
            try
            {
                var stats = await _locationService.GetDeviceStatsAsync(deviceId);
                return Ok(new { success = true, data = stats });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = $"Error: {ex.Message}" });
            }
        }
    }
}