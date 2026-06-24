namespace ProductManagment_APIs.DTOs
{
    public class LocationResponseDto
    {
        public int Id { get; set; }
        public string? DeviceId { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Accuracy { get; set; }
        public double? Speed { get; set; }
        public double? Heading { get; set; }
        public DateTime? Timestamp { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
