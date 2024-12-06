namespace NextStopEndPoints.DTOs
{
    public class LoginResponseDTO
    {
        public int UserId { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
