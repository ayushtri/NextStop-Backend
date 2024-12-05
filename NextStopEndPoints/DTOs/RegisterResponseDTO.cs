namespace NextStopEndPoints.DTOs
{
    public class RegisterResponseDTO
    {
        public UserDTO User { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
