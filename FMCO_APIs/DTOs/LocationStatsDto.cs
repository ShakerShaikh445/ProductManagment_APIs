namespace ProductManagment_APIs.DTOs
{
    public class LocationStatsDto
    {
        public int TotalLocations { get; set; }
        public DateTime FirstLocation { get; set; }
        public DateTime LastLocation { get; set; }
        public double AverageAccuracy { get; set; }
        public double MaxSpeed { get; set; }
    }
}
