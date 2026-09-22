namespace IdentityService.Business.Dtos.Requests
{
    public class LogoutRequest
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
