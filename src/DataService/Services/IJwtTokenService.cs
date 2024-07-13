namespace DataService.Services
{
    public interface IJwtTokenService
    {
        string ValidateTokenAndGetUserId(string token);
    }
}
