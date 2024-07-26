using System.Net;
using System.Text;
using System.Text.Json;
using KomootTourAnalyzer.DTOs;
using Microsoft.Extensions.Configuration;

namespace KomootTourAnalyzer.Services;
public class AuthenticationService : IAuthenticationService
{
	private readonly CookieContainer _cookieContainer = new();
	private readonly HttpClientHandler _handler;
	private readonly IConfiguration _config;
	private LoginResponseDto? loginResponse;
	private string? authValue;

	public AuthenticationService(IConfiguration configuration)
	{
		_handler = new() { CookieContainer = _cookieContainer };
		_config = configuration;
		var komootUrl = configuration["KomootLoginUrl"] ?? throw new KeyNotFoundException("KomootLoginUrl is required");
	}

	public async Task<LoginResponseDto?> Login()
	{
		using var client = new HttpClient()
		{
			BaseAddress = new Uri("https://api.komoot.de")
		};
		var request = new HttpRequestMessage(HttpMethod.Get, "v006/account/email/" + _config["KomootUsername"] + "/");

		var email = _config["KomootUsername"] ?? throw new KeyNotFoundException("KomootUsername is required");
		var password = _config["KomootPassword"] ?? throw new KeyNotFoundException("KomootPassword is required");
		string authValue = Convert.ToBase64String(Encoding.ASCII.GetBytes(email + ":" + password));
		request.Headers.Add("Authorization", "Basic " + authValue); 

		var response = await client.SendAsync(request);
		var content = await response.Content.ReadAsStringAsync();
		return JsonSerializer.Deserialize<LoginResponseDto>(content);
	}

	public async Task AddAuthHeader(HttpRequestMessage message)
	{
		loginResponse ??= await Login() ?? throw new Exception("Login failed");
		authValue ??= "Basic " + Convert.ToBase64String(Encoding.ASCII.GetBytes(loginResponse.Username + ":" + loginResponse.Password));
		message.Headers.Add("Authorization", authValue);
	}

	public async Task<string> GetUserId()
	{
		loginResponse ??= await Login() ?? throw new Exception("Login failed");
		return loginResponse.Username;
	}
}
