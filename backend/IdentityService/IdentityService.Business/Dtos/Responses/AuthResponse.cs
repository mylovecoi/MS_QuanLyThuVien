namespace IdentityService.Business.Dtos.Responses
{
    public class AuthResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public int ExpiresIn { get; set; }
        public UserResponse User { get; set; } = null!;
    }
}
