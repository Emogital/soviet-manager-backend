namespace SovietManager.AuthService.Services
{
    public interface IJwtTokenService
    {
        string GenerateJwtToken(string userId);
    }
}
