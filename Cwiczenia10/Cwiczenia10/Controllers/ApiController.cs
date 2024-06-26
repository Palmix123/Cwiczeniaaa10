using Cwiczenia10.DTOs;
using Cwiczenia10.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cwiczenia10.Controllers;

[ApiController]
[Route("user")]
public class ApiController : ControllerBase
{
    private readonly IConfiguration Configuration;
    private readonly IDbService Service;

    public ApiController(IConfiguration configuration, IDbService service)
    {
        Configuration = configuration;
        Service = service;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> RegisterStudent(RegisterModel model)
    {
        await Service.RegisterUser(model);
        return Ok("User registered");
    }

    [Authorize(Roles = "admin")]
    [HttpGet("students")]
    public IActionResult GetStudents()
    {
        var claims = User.Claims;
        return Ok("Dziala");
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest loginRequest)
    {
        var token = await Service.LoginUser(loginRequest, Configuration);

        return Ok(new
        {
            accessToken = token.Item1,
            refreshToken = token.Item2
        });
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(string refreshToken)
    {
        var result = await Service.RefreshAccessToken(refreshToken, Configuration);
        return Ok(new
        {
            accessToken = result.Item1,
            refreshToken = result.Item2
        });
    }

}