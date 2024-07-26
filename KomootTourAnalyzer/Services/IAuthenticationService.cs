namespace KomootTourAnalyzer.Services;
public interface IAuthenticationService
{
    Task AddAuthHeader(HttpRequestMessage message);
    Task<string> GetUserId();
}