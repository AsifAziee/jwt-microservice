namespace Edstem.Services.IdentityAPI.Models.Dto;

public class AuthenticationResponse
{
    public string Token { get; set; }
    public DateTime Expiration { get; set; }
}