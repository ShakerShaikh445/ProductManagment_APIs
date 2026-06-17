namespace ProductManagment_APIs.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public int? UserId { get; set; }
        public string Name { get; set; }
        public List<string> Roles { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiry { get; set; }
    }
}
