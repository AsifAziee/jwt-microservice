using Edstem.Services.IdentityAPI.Models.Dto;
using Edstem.Services.IdentityAPI.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Edstem.Services.IdentityAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenCreationService _tokenCreationService;

    public UserController(UserManager<IdentityUser> userManager, ITokenCreationService tokenCreationService)
    {
        _userManager = userManager;
        _tokenCreationService = tokenCreationService;
    }

    [HttpPost]
    public async Task<ActionResult<UserDto>> PostUser([FromBody] UserDto user)
    {
        var result = await _userManager.CreateAsync(
            new IdentityUser() { UserName = user.UserName, Email = user.Email },
            user.Password
        );

        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }

        user.Password = null;
        return Created("", user);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<UserDto>> GetUser([FromRoute] string username)
    {
        var user = await _userManager.FindByNameAsync(username);

        if (user == null)
        {
            return NotFound();
        }

        return new UserDto()
        {
            UserName = user.UserName,
            Email = user.Email
        };
    }

    [HttpPost("token")]
    public async Task<ActionResult<AuthenticationResponse>> CreateBearerToken([FromBody] AuthenticationRequest request)
    {
        var user = await _userManager.FindByNameAsync(request.UserName);
        if (user == null)
        {
            return BadRequest("Bad credentials");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            return BadRequest("Bad credentials");
        }

        var token = _tokenCreationService.CreateToken(user);
        return Ok(token);
    }
}