using Edstem.Services.IdentityAPI.Models.Dto;
using Microsoft.AspNetCore.Identity;

namespace Edstem.Services.IdentityAPI.Services;

public interface ITokenCreationService
{
    public AuthenticationResponse CreateToken(IdentityUser user);
}